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
        : BaseCommandService<Membro, MembroCommandDto, IMembroRepository>(service), IMembroComandoServices
    {
        protected override Membro MapToEntity(MembroCommandDto entity) => entity.MapToEntity();

        public async Task InsertAsync(MembroCommandDto membroDto)
        {
            if (Validator(membroDto))
                return;

            membroDto.Telefone = FormatFone(membroDto.Telefone);

            if (await _repository.ExisteAsync(membroDto.Nome) != null)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    string.Format(Message.RegistroExistente, "O membro", membroDto.Nome)
                );
                return;
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
                return;
            }

        }

        public async Task UpdateAsync(Guid code, MembroCommandDto membroDto)
        {
            if (Validator(membroDto))
                return;

            var membro = await _repository.GetByCodigoAsync(code);

            if (membro is null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.IdNaoEncontrado, "O membro", code)
                );
                return;
            }

            if (_repository.ValidaMembroParaAcao(membro.Code))
            {
                Notificar(EnumTipoNotificacao.ClientError, Message.AvisoMembroImutavel);
                return;
            }
            if (await _repository.ExisteAsync(membroDto.Nome) is Membro membroExiste)
            {
                if (membro.Code != membroExiste.Code)
                {
                    Notificar(
                        EnumTipoNotificacao.Informacao,
                        string.Format(Message.RegistroExistente, "O membro", membroDto.Nome)
                    );
                    return;
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
                return;
            }


        }

        public async Task<bool> DeleteAsync(Guid code)
        {
            var membro = await _repository.GetByCodigoAsync(code);

            if (membro == null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.IdNaoEncontrado, "O membro", code)
                );
                return false;
            }

            if (_repository.ValidaMembroParaAcao(membro.Code))
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
