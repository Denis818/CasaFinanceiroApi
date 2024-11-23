using Application.Configurations.MappingsApp;
using Application.Helpers;
using Application.Queries.Dtos;
using Application.Queries.Interfaces.Telas;
using Application.Queries.Services.Base;
using Application.Resources.Messages;
using Domain.Dtos;
using Domain.Dtos.Despesas;
using Domain.Enumeradores;
using Domain.Extensions.Help;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Repositories.GrupoFaturas;
using Domain.Interfaces.Services.Despesa;
using Domain.Models.Despesas;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Application.Queries.Services.Telas
{
    public class DashboardQueryServices(
        IServiceProvider service,
        IGrupoFaturaRepository _grupoFaturaRepository,
        IDespesaDomainServices _despesaDomainServices
    ) : BaseQueryService<Despesa, DespesaDto, IDespesaRepository>(service), IDashboardQueryServices
    {
        private readonly PdfTableHelper _pdfTableCasa =
            new(new PdfTableStyle(textAlignmentColumnKey: TextAlignment.LEFT));

        private readonly PdfTableHelper _pdfTableMoradia =
            new(new PdfTableStyle(textAlignmentColumnKey: TextAlignment.LEFT));

        protected override DespesaDto MapToDTO(Despesa entity) => entity.MapToDTO();

        #region Dashboard
        public async Task<IEnumerable<DespesasPorGrupoQueryDto>> GetDespesaGrupoParaGraficoAsync(
            string ano
        )
        {
            var monthOrder = new Dictionary<string, int>
            {
                { "Janeiro", 1 },
                { "Fevereiro", 2 },
                { "Março", 3 },
                { "Abril", 4 },
                { "Maio", 5 },
                { "Junho", 6 },
                { "Julho", 7 },
                { "Agosto", 8 },
                { "Setembro", 9 },
                { "Outubro", 10 },
                { "Novembro", 11 },
                { "Dezembro", 12 }
            };

            var despesasPorGrupo = await _repository
                .Get(despesa => despesa.GrupoFatura.Ano == ano)
                .GroupBy(d => d.GrupoFatura.Nome)
                .Select(group => new DespesasPorGrupoQueryDto
                {
                    GrupoNome = group.Key,
                    Total = group.Sum(d => d.Total)
                })
                .ToListAsync();

            var result = despesasPorGrupo
                .OrderBy(dto =>
                {
                    var monthName = dto.GrupoNome.Split(' ')[2];
                    return monthOrder[monthName];
                })
                .ToList();

            return result;
        }

        public async Task<DespesasDivididasMensalQueryDto> GetDespesasDivididasMensal()
        {
            var membersByDate = await GetMembersByBate();
            //Aluguel + Condomínio + Conta de Luz
            var distribuicaoCustosMoradia = CalcularDistribuicaoCustosMoradia(membersByDate);

            //Despesas de casa como almoço, Limpeza, higiene etc...
            var distribuicaoCustosCasa = CalcularDistribuicaoCustosCasa(membersByDate);

            var despesasPorMembro = DistribuirDespesasEntreMembros(
                membersByDate,
                distribuicaoCustosCasa.DespesaGeraisMaisAlmocoDividioPorMembro,
                distribuicaoCustosCasa.TotalAlmocoParteDoJhon,
                distribuicaoCustosMoradia.DistribuicaoCustos.ValorParaMembrosForaPeu,
                distribuicaoCustosMoradia.DistribuicaoCustos.ValorParaDoPeu
            );

            return new DespesasDivididasMensalQueryDto { DespesasPorMembro = despesasPorMembro };
        }

        public async Task<RelatorioGastosDoGrupoQueryDto> GetRelatorioDeGastosDoGrupoAsync()
        {
            var grupoNome = await _grupoFaturaRepository
                .Get(g => g.Code == _grupoCode)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (grupoNome.Nome.IsNullOrEmpty())
            {
                Notificar(EnumTipoNotificacao.Informacao, Message.SelecioneUmGrupoDesesa);
                return new();
            }

            double totalGastoMoradia = ListDespesasPorGrupo
                .Where(d =>
                    d.Categoria.Code == CategoriaCods.CodAluguel
                    || d.Categoria.Code == CategoriaCods.CodCondominio
                    || d.Categoria.Code == CategoriaCods.CodContaDeLuz
                )
                .Sum(d => d.Total);

            double totalGastosCasa = ListDespesasPorGrupo
                .Where(d =>
                    d.Categoria.Code != CategoriaCods.CodAluguel
                    && d.Categoria.Code != CategoriaCods.CodCondominio
                    && d.Categoria.Code != CategoriaCods.CodContaDeLuz
                )
                .Sum(d => d.Total);

            var totalGeral = totalGastoMoradia + totalGastosCasa;

            return new RelatorioGastosDoGrupoQueryDto
            {
                GrupoFaturaNome = grupoNome.Nome,
                TotalGeral = totalGeral.RoundTo(2),
                TotalGastosCasa = totalGastosCasa.RoundTo(2),
                TotalGastosMoradia = totalGastoMoradia.RoundTo(2),
            };
        }

        public async Task<byte[]> ExportarPdfRelatorioDeDespesaCasa()
        {
            var members = await GetMembersByBate();
            var custosCasaDto = CalcularDistribuicaoCustosCasa(members);

            return GerarRelatorioDespesaCasaPdf(custosCasaDto);
        }

        public async Task<byte[]> ExportarPdfRelatorioDeDespesaMoradia()
        {
            var members = await GetMembersByBate();

            var custosMoradiaDto = CalcularDistribuicaoCustosMoradia(members);

            return GerarRelatorioDespesaMoradiaPdf(custosMoradiaDto);
        }

        #endregion

        #region Calculo Despesas

        private DespesasDistribuicaoCustosCasaDto CalcularDistribuicaoCustosCasa(
            List<MembroDto> todosMembros
        )
        {
            int membrosForaJhonCount = todosMembros
                .Where(m => m.Code != MembroCods.CodJhon)
                .Count();

            // Despesas gerais Limpesa, Higiêne etc... (Fora Almoço)
            double totalDespesaGeraisForaAlmoco = ListDespesasPorGrupo
                .Where(d =>
                    d.Categoria.Code != CategoriaCods.CodAluguel
                    && d.Categoria.Code != CategoriaCods.CodCondominio
                    && d.Categoria.Code != CategoriaCods.CodContaDeLuz
                    && d.Categoria.Code != CategoriaCods.CodAlmoco
                )
                .Sum(d => d.Total);

            //Total somente do almoço
            double valorTotalAlmoco = ListDespesasPorGrupo
                .Where(despesa => despesa.Categoria.Code == CategoriaCods.CodAlmoco)
                .Sum(despesa => despesa.Total);

            var custosDespesasCasa = new DespesasCustosDespesasCasaDto
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

        private DetalhamentoDespesasMoradiaDto CalcularDistribuicaoCustosMoradia(
            List<MembroDto> todosMembros
        )
        {
            var grupoListMembrosDespesa = GetGrupoListMembrosDespesa(todosMembros);

            var custosDespesasMoradiaDto = GetCustosDespesasMoradiaAsync();

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

                return new DetalhamentoDespesasMoradiaDto
                {
                    GrupoListMembrosDespesa = grupoListMembrosDespesa,
                    CustosDespesasMoradia = custosDespesasMoradiaDto,
                    DistribuicaoCustos = new DespesasDistribuicaoCustosMoradiaDto
                    {
                        ValorParaDoPeu = 300, // 300 reais do aluguel é fixo.
                    }
                };
            }

            var custosMoradiaDto = new DespesasCustosMoradiaDto
            {
                ParcelaApartamento = custosDespesasMoradiaDto.ParcelaApartamento,
                ParcelaCaixa = custosDespesasMoradiaDto.ParcelaCaixa,
                ContaDeLuz = custosDespesasMoradiaDto.ContaDeLuz,
                Condominio = custosDespesasMoradiaDto.Condominio,
                MembrosForaJhonPeuCount = grupoListMembrosDespesa.ListMembroForaJhonPeu.Count,
                MembrosForaJhonCount = grupoListMembrosDespesa.ListMembroForaJhon.Count
            };

            return new DetalhamentoDespesasMoradiaDto()
            {
                GrupoListMembrosDespesa = grupoListMembrosDespesa,

                CustosDespesasMoradia = custosDespesasMoradiaDto,

                DistribuicaoCustos = _despesaDomainServices.CalcularDistribuicaoCustosMoradia(
                    custosMoradiaDto
                )
            };
        }

        private DespesasCustosMoradiaDto GetCustosDespesasMoradiaAsync()
        {
            var listAluguel = ListDespesasPorGrupo.Where(d =>
                d.Categoria.Code == CategoriaCods.CodAluguel
            );

            var parcelaApartamento = listAluguel
                .Where(aluguel => aluguel.Item.ToLower().Contains("ap ponto"))
                .FirstOrDefault();

            var parcelaCaixa = listAluguel
                .Where(aluguel => aluguel.Item.ToLower().Contains("caixa"))
                .FirstOrDefault();

            var contaDeLuz = ListDespesasPorGrupo
                .Where(despesa => despesa.Categoria.Code == CategoriaCods.CodContaDeLuz)
                .FirstOrDefault();

            var condominio = ListDespesasPorGrupo
                .Where(despesa => despesa.Categoria.Code == CategoriaCods.CodCondominio)
                .FirstOrDefault();

            return new DespesasCustosMoradiaDto()
            {
                Condominio = condominio?.Preco ?? 0,
                ContaDeLuz = contaDeLuz?.Preco ?? 0,
                ParcelaCaixa = parcelaCaixa?.Preco ?? 0,
                ParcelaApartamento = parcelaApartamento?.Preco ?? 0,
            };
        }

        private GrupoListMembrosDespesaDto GetGrupoListMembrosDespesa(List<MembroDto> todosMembros)
        {
            var listMembroForaJhonLaila = todosMembros
                .Where(m => m.Code != MembroCods.CodJhon && m.Code != MembroCods.CodLaila)
                .ToList();

            var listMembroForaJhonPeu = listMembroForaJhonLaila
                .Where(m => m.Code != MembroCods.CodPeu)
                .ToList();

            var listAluguel = ListDespesasPorGrupo
                .Where(d => d.Categoria.Code == CategoriaCods.CodAluguel)
                .Select(m => m.MapToDTO())
                .ToList();

            return new GrupoListMembrosDespesaDto()
            {
                ListAluguel = listAluguel,
                ListMembroForaJhon = listMembroForaJhonLaila,
                ListMembroForaJhonPeu = listMembroForaJhonPeu
            };
        }

        #endregion

        #region Gerar Relatório PDF Casa

        private byte[] GerarRelatorioDespesaCasaPdf(DespesasDistribuicaoCustosCasaDto custosCasaDto)
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
                    "Total das despesas gerais somado com a parte do almço que foi divido com Jhon",
                    $"R$ {totalDespesasGeraisMaisAlmocoDividido.ToFormatPriceBr()}"
                },
                { "Total das Despesas", $"R$ {despesaGeraisMaisAlmoco.ToFormatPriceBr()}" },
            };

            _pdfTableCasa.CreateTable(doc, "Despesas somente da Casa", columnsValoresCalculados);
        }

        private void CreateTableValoresParaCada(
            Document doc,
            List<MembroDto> membros,
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

        private byte[] GerarRelatorioDespesaMoradiaPdf(DetalhamentoDespesasMoradiaDto custosMoradia)
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
            IList<MembroDto> listMembroForaJhon,
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
            IList<MembroDto> listMembroForaJhon,
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
            IList<MembroDto> listMembroForaJhon,
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

        private IEnumerable<DespesaPorMembroQueryDto> DistribuirDespesasEntreMembros(
            List<MembroDto> todosMembros,
            double despesaGeraisMaisAlmocoDividioPorMembro,
            double almocoParteDoJhon,
            double aluguelCondominioContaLuzPorMembroForaPeu,
            double aluguelCondominioContaLuzParaPeu
        )
        {
            double ValorMoradia(MembroDto membro)
            {
                if (membro.Code == MembroCods.CodPeu)
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
                    member.Code == MembroCods.CodJhon
                        ? Math.Max(almocoParteDoJhon.RoundTo(2), 0)
                        : Math.Max(despesaGeraisMaisAlmocoDividioPorMembro.RoundTo(2), 0),

                ValorDespesaMoradia =
                    member.Code == MembroCods.CodJhon || member.Code == MembroCods.CodLaila
                        ? -1
                        : ValorMoradia(member).RoundTo(2)
            });

            return valoresPorMembro;
        }

        public async Task<List<MembroDto>> GetMembersByBate()
        {
            var grupoFatura = await _grupoFaturaRepository.GetByCodigoAsync(_grupoCode);

            return await _membroRepository
                .Get(m => m.DataInicio <= grupoFatura.DataCriacao)
                .Select(m => m.MapToDTO())
                .ToListAsync();
        }

        #endregion
    }
}
