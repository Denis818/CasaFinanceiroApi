﻿using Application.Commands.Dtos;
using Application.Commands.Interfaces;
using Application.Commands.Services.Base;
using Application.Configurations.MappingsApp;
using Application.Resources.Messages;
using Domain.Converters.DatesTimes;
using Domain.Dtos;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories.Membros;
using Domain.Models.Membros;
using System.Text.RegularExpressions;

namespace Application.Commands.Services
{
    public class MembroCommandServices
        : BaseCommandService<Membro, MembroCommandDto, IMembroRepository>, IMembroComandoServices
    {
        public MembroIdDto MembroCods => _lazyMembroIds.Value;
        private readonly Lazy<MembroIdDto> _lazyMembroIds;

        public MembroCommandServices(IServiceProvider service) : base(service)
        {
            _lazyMembroIds = _repository.GetMembroCods();
        }
        protected override Membro MapToEntity(MembroCommandDto entity) => entity.MapToEntity();

        public async Task<bool> InsertAsync(MembroCommandDto membroDto)
        {
            if (Validator(membroDto))
                return false;

            membroDto.Telefone = FormatFone(membroDto.Telefone);

            if (await _repository.ExisteAsync(membroDto.Nome) != null)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    string.Format(Message.RegistroExistente, "O membro", membroDto.Nome)
                );
                return false;
            }

            var membro = membroDto.MapToEntity();

            membro.DataInicio = DateTimeZoneConverterPtBR.GetBrasiliaDateTimeZone();

            await _repository.InsertAsync(membro);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Inserir")
                );
                return false;
            }

            return true;

        }

        public async Task<bool> UpdateAsync(Guid code, MembroCommandDto membroDto)
        {
            if (membroDto is null)
            {
                Notificar(
                     EnumTipoNotificacao.NotFount,
                    "Nulo não é valido"
                 );
                return false;

            }
            if (Validator(membroDto))
                return false;

            var membro = await _repository.GetByCodigoAsync(code);

            if (membro is null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.NaoEncontrado, "O membro")
                );
                return false;
            }

            if (ValidaMembroParaAcao(membro.Code))
            {
                Notificar(EnumTipoNotificacao.ClientError, Message.AvisoMembroImutavel);
                return false;
            }
            if (await _repository.ExisteAsync(membroDto.Nome) is Membro membroExiste)
            {
                if (membro.Code != membroExiste.Code)
                {
                    Notificar(
                        EnumTipoNotificacao.Informacao,
                        string.Format(Message.RegistroExistente, "O membro", membroDto.Nome)
                    );
                    return false;
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
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteAsync(Guid code)
        {
            var membro = await _repository.GetByCodigoAsync(code);

            if (membro == null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.NaoEncontrado, "O membro")
                );
                return false;
            }

            if (ValidaMembroParaAcao(membro.Code))
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

        public bool ValidaMembroParaAcao(Guid codeMembro)
        {
            var ehAlteravel =
                codeMembro == MembroCods.CodJhon
                || codeMembro == MembroCods.CodPeu
                || codeMembro == MembroCods.CodLaila;

            return ehAlteravel;
        }

        #endregion
    }
}
