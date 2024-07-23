using Application.Commands.Dtos;
using Application.Commands.Interfaces;
using Application.Commands.Services.Base;
using Application.Configurations.MappingsApp;
using Application.Resources.Messages;
using Domain.Converters.DatesTimes;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Application.Commands.Services
{
    public class GrupoFaturaCommandService(
        IServiceProvider service,
        IStatusFaturaRepository _statusFaturaRepository
    ) : BaseCommandService<GrupoFatura, IGrupoFaturaRepository>(service), IGrupoFaturaCommandService
    {
        public async Task<GrupoFatura> InsertAsync(GrupoFaturaCommandDto grupoDto)
        {
            if (Validator(grupoDto))
                return null;

            grupoDto.Nome = FormatNomeGrupo(grupoDto);

            if (!NomeGrupoIsCorretFormat(grupoDto.Nome))
                return null;

            var existingGrupo = await _repository
                .Get(grupo => grupo.Nome == grupoDto.Nome)
                .FirstOrDefaultAsync();

            if (existingGrupo != null)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    string.Format(Message.RegistroExistente, "o Grupo", grupoDto.Nome)
                );
                return null;
            }

            var grupoFatura = grupoDto.MapToEntity();
            grupoFatura.DataCriacao = DateTimeZoneProvider.GetBrasiliaDateTimeZone();

            grupoFatura.StatusFaturas =
            [
                new()
                {
                    Estado = EnumStatusFatura.CasaAberto.ToString(),
                    FaturaNome = EnumFaturaTipo.Casa.ToString()
                },
                new()
                {
                    Estado = EnumStatusFatura.MoradiaAberto.ToString(),
                    FaturaNome = EnumFaturaTipo.Moradia.ToString()
                }
            ];

            await _repository.InsertAsync(grupoFatura);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Inserir")
                );
                return null;
            }

            return grupoFatura;
        }

        public async Task<GrupoFatura> UpdateAsync(int id, GrupoFaturaCommandDto grupoDto)
        {
            if (Validator(grupoDto))
                return null;

            grupoDto.Nome = FormatNomeGrupo(grupoDto);

            if (!NomeGrupoIsCorretFormat(grupoDto.Nome))
                return null;

            var grupoFatura = await _repository.GetByIdAsync(id);

            if (grupoFatura is null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.IdNaoEncontrado, "Grupo Despesa", id)
                );
                return null;
            }

            if (grupoFatura.Nome == grupoDto.Nome)
                return grupoFatura;

            if (await _repository.ExisteAsync(nome: grupoDto.Nome) is GrupoFatura GrupoFaturaExiste)
            {
                if (grupoFatura.Id != GrupoFaturaExiste.Id)
                {
                    Notificar(
                        EnumTipoNotificacao.Informacao,
                        string.Format(
                            Message.RegistroExistente,
                            "O Grupo de Despesa",
                            grupoDto.Nome
                        )
                    );
                    return null;
                }
            }

            grupoFatura.MapUpdateEntity(grupoDto);

            _repository.Update(grupoFatura);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Atualizar")
                );
                return null;
            }

            return grupoFatura;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var GrupoFatura = await _repository.GetByIdAsync(id);

            if (GrupoFatura == null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.IdNaoEncontrado, "Grupo Despesa", id)
                );
                return false;
            }

            var IsUnicGroup = await _repository.Get().AsNoTracking().ToListAsync();
            if (IsUnicGroup.Count == 1)
            {
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    string.Format(Message.DeletarUnicoGrupoFatura)
                );
                return false;
            }

            _repository.Delete(GrupoFatura);

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

        public async Task<StatusFatura> UpdateStatusFaturaAsync(
            EnumFaturaTipo faturaNome,
            EnumStatusFatura status
        )
        {
            var grupoFaturaId = (int)(_httpContext.Items["GrupoFaturaId"] ?? 0);

            var statusFatura = await _statusFaturaRepository
                .Get(s => s.GrupoFaturaId == grupoFaturaId)
                .FirstOrDefaultAsync(s => s.FaturaNome == faturaNome.ToString());

            statusFatura.Estado = status.ToString();

            _statusFaturaRepository.Update(statusFatura);

            if (!await _statusFaturaRepository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Atualizar")
                );
                return null;
            }

            return statusFatura;
        }

        #region Metodos de Suporte
        private string FormatNomeGrupo(GrupoFaturaCommandDto grupoDto)
        {
            grupoDto.Nome = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(grupoDto.Nome.ToLower());

            return $"Fatura de {grupoDto.Nome} {grupoDto.Ano}";
        }

        private bool NomeGrupoIsCorretFormat(string nomeGrupo)
        {
            var regex = new Regex(
                @"^Fatura de (Janeiro|Fevereiro|Março|Abril|Maio|Junho|Julho|Agosto|Setembro|Outubro|Novembro|Dezembro) \d{4}$",
                RegexOptions.IgnoreCase
            );

            if (!regex.IsMatch(nomeGrupo))
            {
                Notificar(EnumTipoNotificacao.Informacao, Message.NomeGrupoForaDoPadrao);
                return false;
            }

            return true;
        }

        #endregion
    }
}
