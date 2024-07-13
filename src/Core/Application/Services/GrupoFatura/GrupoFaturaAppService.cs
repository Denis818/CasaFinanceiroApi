using Application.Interfaces.Services.Despesas;
using Application.Resources.Messages;
using Application.Services.Base;
using Domain.Converters.DatesTimes;
using Domain.Dtos.Despesas.Criacao;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Application.Services.GrupoFaturas
{
    public class GrupoFaturaAppService(IServiceProvider service)
        : BaseAppService<GrupoFatura, IGrupoFaturaRepository>(service),
            IGrupoFaturaAppService
    {
        #region CRUD
        public async Task<IEnumerable<GrupoFatura>> GetAllAsync() =>
            await _repository
                .Get()
                .Include(s => s.StatusFaturas)
                .OrderBy(c => c.Nome)
                .ToListAsync();

        public async Task<string> GetNameFatura(int id)
        {
            var fatura = await _repository.Get(fatura => fatura.Id == id).FirstOrDefaultAsync();

            return fatura.Nome;
        }

        public async Task<GrupoFatura> InsertAsync(GrupoFaturaDto grupoDto)
        {
            if (Validator(grupoDto))
                return null;

            grupoDto.Nome = FormatNomeGrupo(grupoDto.Nome);

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

            var grupoFatura = _mapper.Map<GrupoFatura>(grupoDto);
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

        public async Task<GrupoFatura> UpdateAsync(int id, GrupoFaturaDto grupoDto)
        {
            if (Validator(grupoDto))
                return null;

            grupoDto.Nome = FormatNomeGrupo(grupoDto.Nome);

            if (!NomeGrupoIsCorretFormat(grupoDto.Nome))
                return null;

            var GrupoFatura = await _repository.GetByIdAsync(id);

            if (GrupoFatura is null)
            {
                Notificar(
                    EnumTipoNotificacao.NotFount,
                    string.Format(Message.IdNaoEncontrado, "Grupo Despesa", id)
                );
                return null;
            }

            if (GrupoFatura.Nome == grupoDto.Nome)
                return GrupoFatura;

            if (await _repository.ExisteAsync(nome: grupoDto.Nome) is GrupoFatura GrupoFaturaExiste)
            {
                if (GrupoFatura.Id != GrupoFaturaExiste.Id)
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

            _mapper.Map(grupoDto, GrupoFatura);

            _repository.Update(GrupoFatura);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Atualizar")
                );
                return null;
            }

            return GrupoFatura;
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

            var IsUnicGroup = await _repository.Get().ToListAsync();
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
        #endregion

        #region Metodos de Suporte
        public string FormatNomeGrupo(string nomeGrupo)
        {
            nomeGrupo = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nomeGrupo.ToLower());

            return $"Fatura de {nomeGrupo} {DateTime.Now.Year}";
        }

        public bool NomeGrupoIsCorretFormat(string nomeGrupo)
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
