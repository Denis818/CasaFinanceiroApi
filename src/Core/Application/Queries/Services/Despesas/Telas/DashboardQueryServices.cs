﻿using Application.Configurations.MappingsApp;
using Application.Helpers;
using Application.Queries.Dtos;
using Application.Queries.Interfaces;
using Application.Queries.Services.Base;
using Application.Resources.Messages;
using Domain.Dtos;
using Domain.Dtos.Despesas;
using Domain.Dtos.QueryResults.Despesas;
using Domain.Enumeradores;
using Domain.Extensions.Help;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services.Despesa;
using Domain.Models.Despesas;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Application.Queries.Services.Telas
{
    public class DashboardQueryServices
        : BaseQueryService<Despesa, DespesaQueryDto, IDespesaRepository>,
            IDashboardQueryServices
    {
        private readonly PdfTableHelper _pdfTableCasa =
            new(new PdfTableStyle(textAlignmentColumnKey: TextAlignment.LEFT));

        private readonly PdfTableHelper _pdfTableMoradia =
            new(new PdfTableStyle(textAlignmentColumnKey: TextAlignment.LEFT));

        private readonly IGrupoFaturaRepository _grupoFaturaRepository;
        private readonly IMembroRepository _membroRepository;
        private readonly IDespesaDomainServices _despesaDomainServices;

        private readonly MembroIdDto _membroId;
        private readonly GrupoFaturaQueryDto _grupoFatura;

        public DashboardQueryServices(
            IServiceProvider service,
            IGrupoFaturaRepository grupoFaturaRepository,
            IMembroRepository membroRepository,
            IDespesaDomainServices despesaDomainServices
        )
            : base(service)
        {
            _grupoFaturaRepository = grupoFaturaRepository;
            _membroRepository = membroRepository;
            _despesaDomainServices = despesaDomainServices;

            _membroId = _membroRepository.GetMembersIds();
            _grupoFatura = _grupoFaturaRepository.GetByCodigoAsync(_grupoCode).Result?.MapToDTO();
        }

        protected override DespesaQueryDto MapToDTO(Despesa entity) => entity.MapToDTO();

        #region Dashboard
        public async Task<IEnumerable<DespesasPorGrupoQueryResult>> GetDespesaGrupoParaGraficoAsync(
            string ano
        )
        {
            var list = await _repository.GetDespesaGrupoParaGraficoAsync(ano);

            if (list.IsNullOrEmpty())
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    "Não há despesa em nenhum grupo de fatura"
                );

                return [];
            }

            return list;
        }

        public async Task<DespesasDivididasMensalQueryDto> GetDespesasDivididasMensalAsync()
        {
            //Aluguel + Condomínio + Conta de Luz
            var distribuicaoCustosMoradia = await CalcularDistribuicaoCustosMoradiaAsync();

            //Despesas de casa como almoço, Limpeza, higiene etc...
            var distribuicaoCustosCasa = await CalcularDistribuicaoCustosCasaAsync();

            var despesasPorMembro = await DistribuirDespesasEntreMembrosAsync(
                distribuicaoCustosCasa.DespesaGeraisMaisAlmocoDividioPorMembro,
                distribuicaoCustosCasa.TotalAlmocoParteDoJhon,
                distribuicaoCustosMoradia.DistribuicaoCustos.ValorParaMembrosForaPeu,
                distribuicaoCustosMoradia.DistribuicaoCustos.ValorParaDoPeu
            );

            return new DespesasDivididasMensalQueryDto { DespesasPorMembro = despesasPorMembro };
        }

        public async Task<RelatorioGastosDoGrupoQueryResult> GetRelatorioDeGastosDoGrupoAsync() =>
            await _repository.GetRelatorioDeGastosDoGrupoAsync(_grupoCode, _categoriaIds);

        public async Task<byte[]> ExportarPdfRelatorioDeDespesaCasa()
        {
            var custosCasaDto = await CalcularDistribuicaoCustosCasaAsync();

            return GerarRelatorioDespesaCasaPdf(custosCasaDto);
        }

        public async Task<byte[]> ExportarPdfRelatorioDeDespesaMoradia()
        {
            var custosMoradiaDto = await CalcularDistribuicaoCustosMoradiaAsync();

            return GerarRelatorioDespesaMoradiaPdf(custosMoradiaDto);
        }

        #endregion

        #region Calculo Despesas

        private async Task<DespesasDistribuicaoCustosCasaQueryDto> CalcularDistribuicaoCustosCasaAsync()
        {
            var todosMembros = await _membroRepository
                .Get(m => m.DataInicio.Date.Month <= _grupoFatura.DataCriacao.Date.Month)
                .AsNoTracking()
                .Select(m => m.MapToDTO())
                .ToListAsync();

            int membrosForaJhonCount = todosMembros.Where(m => m.Code != _membroId.CodJhon).Count();

            // Despesas gerais Limpesa, Higiêne etc... (Fora Almoço)
            double totalDespesaGeraisForaAlmoco = await _queryDespesasPorGrupo
                .Where(d =>
                    d.Categoria.Code != _categoriaIds.CodAluguel
                    && d.Categoria.Code != _categoriaIds.CodCondominio
                    && d.Categoria.Code != _categoriaIds.CodContaDeLuz
                    && d.Categoria.Code != _categoriaIds.CodAlmoco
                )
                .SumAsync(d => d.Total);

            //Total somente do almoço
            double valorTotalAlmoco = await _queryDespesasPorGrupo
                .Where(despesa => despesa.Categoria.Code == _categoriaIds.CodAlmoco)
                .SumAsync(despesa => despesa.Total);

            var custosDespesasCasa = new DespesasCustosDespesasCasaQueryDto
            {
                TodosMembros = todosMembros,
                ValorTotalAlmoco = valorTotalAlmoco,
                TotalDespesaGeraisForaAlmoco = totalDespesaGeraisForaAlmoco,
                MembrosForaJhonCount = membrosForaJhonCount
            };

            var distribuicaoCustosCasa = _despesaDomainServices.CalcularDistribuicaoCustosCasa(
                custosDespesasCasa
            );

            return distribuicaoCustosCasa;
        }

        private async Task<DetalhamentoDespesasMoradiaQueryDto> CalcularDistribuicaoCustosMoradiaAsync()
        {
            var grupoListMembrosDespesa = await GetGrupoListMembrosDespesa();

            var custosDespesasMoradiaDto = await GetCustosDespesasMoradiaAsync();

            if (
                custosDespesasMoradiaDto.ContaDeLuz == 0
                && custosDespesasMoradiaDto.ParcelaApartamento == 0
                && grupoListMembrosDespesa.ListAluguel.Count <= 0
            )
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    string.Format(Message.DespesasNaoEncontradas, "de Moradia")
                );

                return new DetalhamentoDespesasMoradiaQueryDto
                {
                    GrupoListMembrosDespesa = grupoListMembrosDespesa,
                    CustosDespesasMoradia = custosDespesasMoradiaDto,
                    DistribuicaoCustos = new DespesasDistribuicaoCustosMoradiaDto
                    {
                        ValorParaDoPeu = 300, // 300 reais do aluguel é fixo.
                    }
                };
            }

            var custosMoradiaDto = new DespesasCustosMoradiaQueryDto
            {
                ParcelaApartamento = custosDespesasMoradiaDto.ParcelaApartamento,
                ParcelaCaixa = custosDespesasMoradiaDto.ParcelaCaixa,
                ContaDeLuz = custosDespesasMoradiaDto.ContaDeLuz,
                Condominio = custosDespesasMoradiaDto.Condominio,
                MembrosForaJhonPeuCount = grupoListMembrosDespesa.ListMembroForaJhonPeu.Count,
                MembrosForaJhonCount = grupoListMembrosDespesa.ListMembroForaJhon.Count
            };

            return new DetalhamentoDespesasMoradiaQueryDto()
            {
                GrupoListMembrosDespesa = grupoListMembrosDespesa,

                CustosDespesasMoradia = custosDespesasMoradiaDto,

                DistribuicaoCustos = _despesaDomainServices.CalcularDistribuicaoCustosMoradia(
                    custosMoradiaDto
                )
            };
        }

        private async Task<DespesasCustosMoradiaQueryDto> GetCustosDespesasMoradiaAsync()
        {
            var listAluguel = _queryDespesasPorGrupo.Where(d =>
                d.Categoria.Code == _categoriaIds.CodAluguel
            );

            var parcelaApartamento = await listAluguel
                .Where(aluguel => aluguel.Item.ToLower().Contains("ap ponto"))
                .FirstOrDefaultAsync();

            var parcelaCaixa = await listAluguel
                .Where(aluguel => aluguel.Item.ToLower().Contains("caixa"))
                .FirstOrDefaultAsync();

            var contaDeLuz = await _queryDespesasPorGrupo
                .Where(despesa => despesa.Categoria.Code == _categoriaIds.CodContaDeLuz)
                .FirstOrDefaultAsync();

            var condominio = await _queryDespesasPorGrupo
                .Where(despesa => despesa.Categoria.Code == _categoriaIds.CodCondominio)
                .FirstOrDefaultAsync();

            return new DespesasCustosMoradiaQueryDto()
            {
                Condominio = condominio?.Preco ?? 0,
                ContaDeLuz = contaDeLuz?.Preco ?? 0,
                ParcelaCaixa = parcelaCaixa?.Preco ?? 0,
                ParcelaApartamento = parcelaApartamento?.Preco ?? 0,
            };
        }

        private async Task<GrupoListMembrosDespesaQueryDto> GetGrupoListMembrosDespesa()
        {
            List<MembroQueryDto> todosMembros = await _membroRepository
                .Get(m => m.DataInicio <= _grupoFatura.DataCriacao)
                .AsNoTracking()
                .Select(m => m.MapToDTO())
                .ToListAsync();

            List<MembroQueryDto> listMembroForaJhonLaila = todosMembros
                .Where(m => m.Code != _membroId.CodJhon && m.Code != _membroId.CodLaila)
                .ToList();

            List<MembroQueryDto> listMembroForaJhonPeu = listMembroForaJhonLaila
                .Where(m => m.Code != _membroId.CodPeu)
                .ToList();

            List<DespesaQueryDto> listAluguel = await _queryDespesasPorGrupo
                .Where(d => d.Categoria.Code == _categoriaIds.CodAluguel)
                .Select(m => m.MapToDTO())
                .ToListAsync();

            return new GrupoListMembrosDespesaQueryDto()
            {
                ListAluguel = listAluguel,
                ListMembroForaJhon = listMembroForaJhonLaila,
                ListMembroForaJhonPeu = listMembroForaJhonPeu
            };
        }

        #endregion

        #region Gerar Relatório PDF Casa

        private byte[] GerarRelatorioDespesaCasaPdf(
            DespesasDistribuicaoCustosCasaQueryDto custosCasaDto
        )
        {
            using var memoryStream = new MemoryStream();
            using var writer = new PdfWriter(memoryStream);
            using var mergedDocument = new PdfDocument(writer);
            using var doc = new Document(mergedDocument);

            _pdfTableCasa.CreateTitleDocument(doc, "Relatório detalhado dos valores divididos");

            CreateTableCalculos(
                doc,
                custosCasaDto.TotalDespesaGeraisForaAlmoco,
                custosCasaDto.TotalAlmocoDividioComJhon,
                custosCasaDto.TotalDespesasGeraisMaisAlmocoDividido,
                custosCasaDto.TotalSomenteAlmoco,
                custosCasaDto.DespesaGeraisMaisAlmoco,
                custosCasaDto.Membros.Count
            );

            CreateTableValoresParaCada(
                doc,
                custosCasaDto.Membros,
                custosCasaDto.DespesaGeraisMaisAlmocoDividioPorMembro,
                custosCasaDto.TotalAlmocoParteDoJhon
            );

            doc.Close();

            return memoryStream.ToArray();
        }

        private void CreateTableCalculos(
            Document doc,
            double totalDespesaGeraisForaAlmoco,
            double totalAlmocoDividioComJhon,
            double totalDespesasGeraisMaisAlmocoDividido,
            double totalSomenteAlmoco,
            double despesaGeraisMaisAlmoco,
            int countMembros
        )
        {
            var columnsValoresCalculados = new Dictionary<string, string>
            {
                { "Total de membros", $"{countMembros}" },
                { "Despesas fora almoço", $"R$ {totalDespesaGeraisForaAlmoco.ToFormatPriceBr()}" },
                { "Despesas somente almoço", $"R$ {totalSomenteAlmoco.ToFormatPriceBr()}" },
                {
                    "Almoço fora parte do Jhon",
                    $"R$ {totalAlmocoDividioComJhon.ToFormatPriceBr()}"
                },
                {
                    "Total com almoco/janta dividido com Jhon",
                    $"R$ {totalDespesasGeraisMaisAlmocoDividido.ToFormatPriceBr()}"
                },
                { "Total das Despesas", $"R$ {despesaGeraisMaisAlmoco.ToFormatPriceBr()}" },
            };

            _pdfTableCasa.CreateTable(doc, "Despesas somente da Casa", columnsValoresCalculados);
        }

        private void CreateTableValoresParaCada(
            Document doc,
            List<MembroQueryDto> membros,
            double despesaGeraisMaisAlmocoDividioPorMembro,
            double totalAlmocoParteDoJhon
        )
        {
            Dictionary<string, string> columnsTotalParaCada = [];
            foreach (var membro in membros)
            {
                double valorParaCada = despesaGeraisMaisAlmocoDividioPorMembro;

                if (membro.Nome.Contains("Jhon", StringComparison.CurrentCultureIgnoreCase))
                {
                    valorParaCada = totalAlmocoParteDoJhon;
                }

                columnsTotalParaCada.Add(membro.Nome, $"R$ {valorParaCada.ToFormatPriceBr()}");
            }

            _pdfTableCasa.CreateTable(doc, "Valor que cada um deve pagar", columnsTotalParaCada);
        }
        #endregion

        #region Gerar Relatório PDF Moradia

        private byte[] GerarRelatorioDespesaMoradiaPdf(DetalhamentoDespesasMoradiaQueryDto custosMoradia)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new PdfWriter(memoryStream);
            using var mergedDocument = new PdfDocument(writer);
            using var doc = new Document(mergedDocument);

            _pdfTableMoradia.CreateTitleDocument(doc, "Relatório detalhado dos valores divididos");

            TableValoresIniciais(
                doc,
                custosMoradia.CustosDespesasMoradia.ParcelaApartamento,
                custosMoradia.CustosDespesasMoradia.ParcelaCaixa,
                custosMoradia.CustosDespesasMoradia.ContaDeLuz,
                custosMoradia.CustosDespesasMoradia.Condominio
            );

            TableCalculos(
                doc,
                custosMoradia.DistribuicaoCustos.TotalAptoMaisCaixa,
                custosMoradia.DistribuicaoCustos.TotalLuzMaisCondominio,
                custosMoradia.DistribuicaoCustos.TotalAptoMaisCaixaAbate300Peu100Estacionamento
            );

            TableParcelaCaixaApto(
                doc,
                custosMoradia.GrupoListMembrosDespesa.ListMembroForaJhon, //ListMembroForaJhon,
                custosMoradia.DistribuicaoCustos.ValorAptoMaisCaixaParaCadaMembro
            );

            TableContaLuzAndCondominio(
                doc,
                custosMoradia.GrupoListMembrosDespesa.ListMembroForaJhon,
                custosMoradia.DistribuicaoCustos.ValorLuzMaisCondominioParaCadaMembro
            );

            TableValoresParaCada(
                doc,
                custosMoradia.GrupoListMembrosDespesa.ListMembroForaJhon,
                custosMoradia.DistribuicaoCustos.ValorParaMembrosForaPeu,
                custosMoradia.DistribuicaoCustos.ValorParaDoPeu
            );

            doc.Close();

            return memoryStream.ToArray();
        }

        private void TableValoresIniciais(
            Document doc,
            double parcelaApartamento,
            double parcelaCaixa,
            double contaDeLuz,
            double condominio
        )
        {
            var columnsValoresIniciais = new Dictionary<string, string>
            {
                { "Parcela do Apartamento", $"R$ {parcelaApartamento.ToFormatPriceBr()}" },
                { "Parcela da Caixa", $"R$ {parcelaCaixa.ToFormatPriceBr()}" },
                { "Conta de Luz", $"R$ {contaDeLuz.ToFormatPriceBr()}" },
                { "Condomínio", $"R$ {condominio.ToFormatPriceBr()}" },
            };
            _pdfTableMoradia.CreateTable(doc, "Valores Iniciais", columnsValoresIniciais);
        }

        private void TableCalculos(
            Document doc,
            double totalAptoMaisCaixa,
            double totalLuzMaisCondominio,
            double totalAptoMaisCaixaAbate300Peu100Estacionamento
        )
        {
            var columnsCalculos = new Dictionary<string, string>
            {
                { "Parcela Apto mais Caixa", $"R$ {totalAptoMaisCaixa.ToFormatPriceBr()}" },
                {
                    "Conta de Luz mais Condomínio",
                    $"R$ {totalLuzMaisCondominio.ToFormatPriceBr()}"
                },
                {
                    "Apto mais Caixa menos R$ 300 do Peu e R$ 100 do estacionamento alugado",
                    $"R$ {totalAptoMaisCaixaAbate300Peu100Estacionamento.ToFormatPriceBr()}"
                },
            };
            _pdfTableMoradia.CreateTable(doc, "Cálculos", columnsCalculos);
        }

        private void TableParcelaCaixaApto(
            Document doc,
            IList<MembroQueryDto> listMembroForaJhon,
            double valorAptoMaisCaixaParaCadaMembro
        )
        {
            Dictionary<string, string> columnsAptoCaixaParaCada = [];

            foreach (var membro in listMembroForaJhon)
            {
                var valorAluguel = valorAptoMaisCaixaParaCadaMembro;

                if (membro.Nome.Contains("Peu", StringComparison.CurrentCultureIgnoreCase))
                {
                    valorAluguel = 300;
                }

                columnsAptoCaixaParaCada.Add(membro.Nome, $"R$ {valorAluguel.ToFormatPriceBr()}");
            }

            _pdfTableMoradia.CreateTable(
                doc,
                "Parcela do Apto e Caixa para cada",
                columnsAptoCaixaParaCada
            );
        }

        private void TableContaLuzAndCondominio(
            Document doc,
            IList<MembroQueryDto> listMembroForaJhon,
            double valorLuzMaisCondominioParaCadaMembro
        )
        {
            Dictionary<string, string> columnsLuzCondParaCada = [];

            foreach (var membro in listMembroForaJhon)
            {
                columnsLuzCondParaCada.Add(
                    membro.Nome,
                    $"R$ {valorLuzMaisCondominioParaCadaMembro.ToFormatPriceBr()}"
                );
            }

            _pdfTableMoradia.CreateTable(
                doc,
                "Conta de Luz e Condomínio para cada",
                columnsLuzCondParaCada
            );
        }

        private void TableValoresParaCada(
            Document doc,
            IList<MembroQueryDto> listMembroForaJhon,
            double valorParaMembrosForaPeu,
            double valorParaDoPeu
        )
        {
            Dictionary<string, string> columnsTotalParaCada = [];

            foreach (var membro in listMembroForaJhon)
            {
                var valorParaCada = valorParaMembrosForaPeu;

                if (membro.Nome.Contains("Peu", StringComparison.CurrentCultureIgnoreCase))
                {
                    valorParaCada = valorParaDoPeu;
                }

                columnsTotalParaCada.Add(membro.Nome, $"R$ {valorParaCada.ToFormatPriceBr()}");
            }
            _pdfTableMoradia.CreateTable(doc, "Valor que cada um deve pagar", columnsTotalParaCada);
        }
        #endregion

        #region Metodos de Suporte

        private async Task<
            IEnumerable<DespesaPorMembroQueryDto>
        > DistribuirDespesasEntreMembrosAsync(
            double despesaGeraisMaisAlmocoDividioPorMembro,
            double almocoParteDoJhon,
            double aluguelCondominioContaLuzPorMembroForaPeu,
            double aluguelCondominioContaLuzParaPeu
        )
        {
            var todosMembros = await _membroRepository
                .Get(m => m.DataInicio.Date.Month <= _grupoFatura.DataCriacao.Date.Month)
                .AsNoTracking()
                .Select(m => m.MapToDTO())
                .ToListAsync();

            double ValorMoradia(MembroQueryDto membro)
            {
                if (membro.Code == _membroId.CodPeu)
                {
                    return aluguelCondominioContaLuzParaPeu.RoundTo(2);
                }
                else
                {
                    return aluguelCondominioContaLuzPorMembroForaPeu.RoundTo(2);
                }
            }

            var valoresPorMembro = todosMembros.Select(member => new DespesaPorMembroQueryDto
            {
                Nome = member.Nome,

                ValorDespesaCasa =
                    member.Code == _membroId.CodJhon
                        ? Math.Max(almocoParteDoJhon.RoundTo(2), 0)
                        : Math.Max(despesaGeraisMaisAlmocoDividioPorMembro.RoundTo(2), 0),

                ValorDespesaMoradia =
                    member.Code == _membroId.CodJhon || member.Code == _membroId.CodLaila
                        ? -1
                        : ValorMoradia(member).RoundTo(2)
            });

            return valoresPorMembro;
        }

        #endregion
    }
}