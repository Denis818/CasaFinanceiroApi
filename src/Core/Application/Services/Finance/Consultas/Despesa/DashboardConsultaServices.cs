using Application.Helpers;
using Application.Resources.Messages;
using Application.Services.Base;
using Domain.Dtos.Categorias.Consultas;
using Domain.Dtos.Despesas.Consultas;
using Domain.Dtos.Despesas.Relatorios;
using Domain.Dtos.Despesas.Resumos;
using Domain.Dtos.Membros;
using Domain.Enumeradores;
using Domain.Extensions.Help;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services.Despesa;
using Domain.Models.Despesas;
using Domain.Models.Membros;
using iText.Kernel.Pdf;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using iText.Layout;
using Application.Interfaces.Services.Finance.Consultas;

namespace Application.Services.Finance.Consultas
{
    public class DashboardConsultaServices : BaseAppService<Despesa, IDespesaRepository>, IDashboardConsultaServices
    {
        private readonly PdfTableHelper _pdfTableCasa = new();
        private readonly PdfTableHelper _pdfTableMoradia = new();

        private readonly IGrupoFaturaRepository _grupoFaturaRepository;
        private readonly IMembroRepository _membroRepository;
        private readonly IDespesaDomainServices _despesaDomainServices;

        private readonly CategoriaIdsDto _categoriaIds;
        private readonly MembroIdDto _membroId;
        private readonly IQueryable<Despesa> _queryDespesasPorGrupo;
        private readonly GrupoFatura _grupoFatura;

        public DashboardConsultaServices(
            IServiceProvider service,
            IGrupoFaturaRepository grupoFaturaRepository,
            ICategoriaRepository categoriaRepository,
            IMembroRepository membroRepository,
            IDespesaDomainServices despesaDomainServices) : base(service)
        {
            _grupoFaturaRepository = grupoFaturaRepository;
            _membroRepository = membroRepository;
            _despesaDomainServices = despesaDomainServices;

            _membroId = _membroRepository.GetMembersIds();
            _categoriaIds = categoriaRepository.GetCategoriaIds();

           int grupoId = (int)(_httpContext.Items["GrupoFaturaId"] ?? 0);

            _grupoFatura = _grupoFaturaRepository.GetByIdAsync(grupoId).Result;

            _queryDespesasPorGrupo = _repository
                .Get(d => d.GrupoFaturaId == grupoId)
                .Include(c => c.Categoria)
                .Include(c => c.GrupoFatura);
        }

        #region Dashboard

        public async Task<IEnumerable<DespesasTotalPorCategoriaDto>> GetTotalPorCategoriaAsync()
        {
            var listDespesas = await _queryDespesasPorGrupo.ToListAsync();

            if (listDespesas.Count <= 0)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    string.Format(Message.DespesasNaoEncontradas, "")
                );
                return [];
            }

            var listAgrupada = listDespesas.GroupBy(despesa => despesa.Categoria.Descricao);

            return listAgrupada.Select(list => new DespesasTotalPorCategoriaDto(
                list.Key,
                list.Sum(despesa => despesa.Total)
            ));
        }

        public async Task<IEnumerable<DespesasPorGrupoDto>> GetDespesaGrupoParaGraficoAsync()
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

            var despesasPorGrupo = _repository
                .Get()
                .GroupBy(d => d.GrupoFatura.Nome)
                .Select(group => new DespesasPorGrupoDto
                {
                    GrupoNome = group.Key,
                    Total = group.Sum(d => d.Total)
                })
                .AsEnumerable()
                .OrderBy(dto =>
                {
                    var monthName = dto.GrupoNome.Split(' ')[2];
                    return monthOrder[monthName];
                });

            return await Task.FromResult(despesasPorGrupo.ToList());
        }

        public async Task<DespesasDivididasMensalDto> GetAnaliseDesesasPorGrupoAsync()
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

            var relatorioGastosDoGrupo = await GetRelatorioDeGastosDoMesAsync();

            return new DespesasDivididasMensalDto
            {
                RelatorioGastosDoGrupo = relatorioGastosDoGrupo,

                DespesasPorMembro = despesasPorMembro
            };
        }

        public async Task<byte[]> DownloadPdfRelatorioDeDespesaCasa()
        {
            var custosCasaDto = await CalcularDistribuicaoCustosCasaAsync();

            return GerarRelatorioDespesaCasaPdf(custosCasaDto);
        }

        public async Task<byte[]> DownloadPdfRelatorioDeDespesaMoradia()
        {
            var custosMoradiaDto = await CalcularDistribuicaoCustosMoradiaAsync();

            return GerarRelatorioDespesaMoradiaPdf(custosMoradiaDto);
        }

        #endregion

        #region Calculo Despesas

        private async Task<DistribuicaoCustosCasaDto> CalcularDistribuicaoCustosCasaAsync()
        {
            List<Membro> todosMembros = await _membroRepository
                .Get(m => m.DataInicio.Date.Month <= _grupoFatura.DataCriacao.Date.Month)
                .ToListAsync();

            int membrosForaJhonCount = todosMembros.Where(m => m.Id != _membroId.IdJhon).Count();

            // Despesas gerais Limpesa, Higiêne etc... (Fora Almoço)
            double totalDespesaGeraisForaAlmoco = await _queryDespesasPorGrupo
                .Where(d =>
                    d.CategoriaId != _categoriaIds.IdAluguel
                    && d.CategoriaId != _categoriaIds.IdCondominio
                    && d.CategoriaId != _categoriaIds.IdContaDeLuz
                    && d.CategoriaId != _categoriaIds.IdAlmoco
                )
                .SumAsync(d => d.Total);

            //Total somente do almoço
            double valorTotalAlmoco = await _queryDespesasPorGrupo
                .Where(despesa => despesa.CategoriaId == _categoriaIds.IdAlmoco)
                .SumAsync(despesa => despesa.Total);

            var custosDespesasCasa = new CustosDespesasCasaDto
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

        private async Task<DetalhamentoDespesasMoradiaDto> CalcularDistribuicaoCustosMoradiaAsync()
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

                return new DetalhamentoDespesasMoradiaDto
                {
                    GrupoListMembrosDespesa = grupoListMembrosDespesa,
                    CustosDespesasMoradia = custosDespesasMoradiaDto,
                    DistribuicaoCustos = new DistribuicaoCustosMoradiaDto
                    {
                        ValorParaDoPeu = 300, // 300 reais do aluguel é fixo.
                    }
                };
            }

            var custosMoradiaDto = new CustosDespesasMoradiaDto
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

        private async Task<CustosDespesasMoradiaDto> GetCustosDespesasMoradiaAsync()
        {
            var listAluguel = _queryDespesasPorGrupo.Where(d =>
                d.CategoriaId == _categoriaIds.IdAluguel
            );

            Despesa parcelaApartamento = await listAluguel
                .Where(aluguel => aluguel.Item.ToLower().Contains("ap ponto"))
                .FirstOrDefaultAsync();

            Despesa parcelaCaixa = await listAluguel
                .Where(aluguel => aluguel.Item.ToLower().Contains("caixa"))
                .FirstOrDefaultAsync();

            Despesa contaDeLuz = await _queryDespesasPorGrupo
                .Where(despesa => despesa.CategoriaId == _categoriaIds.IdContaDeLuz)
                .FirstOrDefaultAsync();

            Despesa condominio = await _queryDespesasPorGrupo
                .Where(despesa => despesa.CategoriaId == _categoriaIds.IdCondominio)
                .FirstOrDefaultAsync();

            return new CustosDespesasMoradiaDto()
            {
                Condominio = condominio?.Preco ?? 0,
                ContaDeLuz = contaDeLuz?.Preco ?? 0,
                ParcelaCaixa = parcelaCaixa?.Preco ?? 0,
                ParcelaApartamento = parcelaApartamento?.Preco ?? 0,
            };
        }

        private async Task<GrupoListMembrosDespesaDto> GetGrupoListMembrosDespesa()
        {
            List<Membro> todosMembros = await _membroRepository
                .Get(m => m.DataInicio <= _grupoFatura.DataCriacao)
                .ToListAsync();

            List<Membro> listMembroForaJhonLaila = todosMembros
                .Where(m => m.Id != _membroId.IdJhon && m.Id != _membroId.IdLaila)
                .ToList();

            List<Membro> listMembroForaJhonPeu = listMembroForaJhonLaila
                .Where(m => m.Id != _membroId.IdPeu)
                .ToList();

            List<Despesa> listAluguel = await _queryDespesasPorGrupo
                .Where(d => d.CategoriaId == _categoriaIds.IdAluguel)
                .ToListAsync();

            return new GrupoListMembrosDespesaDto()
            {
                ListAluguel = listAluguel,
                ListMembroForaJhon = listMembroForaJhonLaila,
                ListMembroForaJhonPeu = listMembroForaJhonPeu
            };
        }

        #endregion

        #region Gerar Relatório PDF Casa

        private byte[] GerarRelatorioDespesaCasaPdf(DistribuicaoCustosCasaDto custosCasaDto)
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
            List<Membro> membros,
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
            IList<Membro> listMembroForaJhon,
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
            IList<Membro> listMembroForaJhon,
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
            IList<Membro> listMembroForaJhon,
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

        private async Task<RelatorioGastosDoGrupoDto> GetRelatorioDeGastosDoMesAsync()
        {
            string grupoNome = _grupoFaturaRepository
                .Get(g => g.Id == _grupoFatura.Id)
                .FirstOrDefault()
                ?.Nome;

            if (grupoNome.IsNullOrEmpty())
            {
                Notificar(EnumTipoNotificacao.Informacao, Message.SelecioneUmGrupoDesesa);
                return new();
            }

            double totalGastoMoradia = await _queryDespesasPorGrupo
                .Where(d =>
                    d.Categoria.Id == _categoriaIds.IdAluguel
                    || d.Categoria.Id == _categoriaIds.IdCondominio
                    || d.Categoria.Id == _categoriaIds.IdContaDeLuz
                )
                .SumAsync(d => d.Total);

            double totalGastosCasa = await _queryDespesasPorGrupo
                .Where(d =>
                    d.Categoria.Id != _categoriaIds.IdAluguel
                    && d.Categoria.Id != _categoriaIds.IdCondominio
                    && d.Categoria.Id != _categoriaIds.IdContaDeLuz
                )
                .SumAsync(d => d.Total);

            var totalGeral = totalGastoMoradia + totalGastosCasa;

            return new RelatorioGastosDoGrupoDto
            {
                GrupoFaturaNome = grupoNome,
                TotalGeral = totalGeral.RoundTo(2),
                TotalGastosCasa = totalGastosCasa.RoundTo(2),
                TotalGastosMoradia = totalGastoMoradia.RoundTo(2),
            };
        }

        private async Task<IEnumerable<DespesaPorMembroDto>> DistribuirDespesasEntreMembrosAsync(
            double despesaGeraisMaisAlmocoDividioPorMembro,
            double almocoParteDoJhon,
            double aluguelCondominioContaLuzPorMembroForaPeu,
            double aluguelCondominioContaLuzParaPeu
        )
        {
            var todosMembros = await _membroRepository
                .Get(m => m.DataInicio.Date.Month <= _grupoFatura.DataCriacao.Date.Month)
                .ToListAsync();

            double ValorMoradia(Membro membro)
            {
                if (membro.Id == _membroId.IdPeu)
                {
                    return aluguelCondominioContaLuzParaPeu.RoundTo(2);
                }
                else
                {
                    return aluguelCondominioContaLuzPorMembroForaPeu.RoundTo(2);
                }
            }

            var valoresPorMembro = todosMembros.Select(member => new DespesaPorMembroDto
            {
                Nome = member.Nome,

                ValorDespesaCasa =
                    member.Id == _membroId.IdJhon
                        ? Math.Max(almocoParteDoJhon.RoundTo(2), 0)
                        : Math.Max(despesaGeraisMaisAlmocoDividioPorMembro.RoundTo(2), 0),

                ValorDespesaMoradia =
                    member.Id == _membroId.IdJhon || member.Id == _membroId.IdLaila
                        ? -1
                        : ValorMoradia(member).RoundTo(2)
            });

            return valoresPorMembro;
        }

        #endregion
    }
}