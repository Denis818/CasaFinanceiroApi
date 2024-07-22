using Application.Commands.Dtos;
using Application.Commands.Interfaces;
using Application.Commands.Services.Base;
using Application.Configurations.MappingsApp;
using Application.Resources.Messages;
using Domain.Converters.DatesTimes;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Membros;
using System.Text.RegularExpressions;

namespace Application.Commands.Services
{
    public class MembroCommandServices(IServiceProvider service)
        : BaseCommandService<Membro, IMembroRepository>(service), IMembroComandoServices
    {
        public async Task<Membro> InsertAsync(MembroCommandDto membroDto)
        {
            if (Validator(membroDto))
                return null;

            membroDto.Telefone = FormatFone(membroDto.Telefone);

            if (await _repository.ExisteAsync(membroDto.Nome) != null)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    string.Format(Message.RegistroExistente, "O membro", membroDto.Nome)
                );
                return null;
            }

            var membro = membroDto.MapToEntity();

            membro.DataInicio = DateTimeZoneProvider.GetBrasiliaDateTimeZone();

            await _repository.InsertAsync(membro);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Inserir")
                );
                return null;
            }

            return membro;
        }

        public async Task<Membro> UpdateAsync(int id, MembroCommandDto membroDto)
        {
            if (Validator(membroDto))
                return null;

            var membro = await _repository.GetByIdAsync(id);

            if (membro is null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.IdNaoEncontrado, "O membro", id)
                );
                return null;
            }

            if (_repository.ValidaMembroParaAcao(membro.Id))
            {
                Notificar(EnumTipoNotificacao.ClientError, Message.AvisoMembroImutavel);
                return null;
            }

            if (await _repository.ExisteAsync(membroDto.Nome) is Membro membroExiste)
            {
                if (membro.Id != membroExiste.Id)
                {
                    Notificar(
                        EnumTipoNotificacao.Informacao,
                        string.Format(Message.RegistroExistente, "O membro", membroDto.Nome)
                    );
                    return null;
                }
            }

            membro.MapUpdateEntity(membroDto);

            _repository.Update(membro);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Atualizar")
                );
                return null;
            }

            return membro;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var membro = await _repository.GetByIdAsync(id);

            if (membro == null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.IdNaoEncontrado, "O membro", id)
                );
                return false;
            }

            if (_repository.ValidaMembroParaAcao(membro.Id))
            {
                Notificar(EnumTipoNotificacao.Informacao, Message.AvisoMembroImutavel);
                return false;
            }

            _repository.Delete(membro);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Deletar")
                );
                return false;
            }

            return true;
        }

        #region Metodos de Suporte

        private static string FormatFone(string telefone)
        {
            string numeros = Regex.Replace(telefone, "[^0-9]", "");

            if (numeros.Length == 10)
            {
                return $"({numeros.Substring(0, 2)}) {numeros.Substring(2, 4)}-{numeros.Substring(6)}";
            }
            else if (numeros.Length == 11)
            {
                return $"({numeros.Substring(0, 2)}) {numeros.Substring(2, 5)}-{numeros.Substring(7)}";
            }
            else
            {
                return telefone;
            }
        }

        #endregion
    }
}
