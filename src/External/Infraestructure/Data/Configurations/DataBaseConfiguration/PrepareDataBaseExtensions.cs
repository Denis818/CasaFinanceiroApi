using Application.Commands.Dtos;
using Application.Commands.Interfaces;
using Application.Helpers;
using Domain.Converters.DatesTimes;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Repositories.Categorias;
using Domain.Interfaces.Repositories.GrupoFaturas;
using Domain.Interfaces.Repositories.ListaCompras;
using Domain.Interfaces.Repositories.Membros;
using Domain.Interfaces.Repositories.Users;
using Domain.Models.Categorias;
using Domain.Models.Despesas;
using Domain.Models.GrupoFaturas;
using Domain.Models.ListaCompras;
using Domain.Models.Membros;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace Infraestructure.Data.Configurations.DataBaseConfiguration
{
    public class PrepareDataBaseExtensions
    {
        public void PrepareDataBase(IServiceProvider service, string nomeDominio)
        {
            PrepareUserMaster(service, nomeDominio);
            PrepararVisitante(service);
            PrepararCategorias(service).Wait();
            PrepararMembros(service).Wait();
            PrepararGruposFaturas(service).Wait();
            PrepararParametroDeAlertaDeGastos(service).Wait();
            PrepararProdutoListaCompras(service).Wait();
        }

        private void PrepareUserMaster(IServiceProvider service, string nomeDominio)
        {
            var usuarioRepository = service.GetRequiredService<IUsuarioRepository>();
            var authService = service.GetRequiredService<IAuthCommandService>();

            string email = "master@gmail.com";
            string senha = "Master@123456";

            if (nomeDominio.Contains("dev") || nomeDominio.Contains("railway"))
            {
                email = "dev@gmail.com";
                senha = "dev@123";
            }

            if (usuarioRepository.Get(u => u.Email == email).FirstOrDefault() != null)
                return;

            var (Salt, PasswordHash) = new PasswordHasherHelper().CriarHashSenha(senha);

            var usuario = new Usuario
            {
                Email = email,
                Password = PasswordHash,
                Salt = Salt,
                Permissoes = []
            };

            var permissoes = PrepararPermissoes();

            usuarioRepository.InsertAsync(usuario).Wait();
            usuarioRepository.SaveChangesAsync().Wait();

            authService
                .AddPermissaoAsync(new UserPermissionCommandDto(usuario.Id, permissoes))
                .Wait();
        }

        private void PrepararVisitante(IServiceProvider service)
        {
            var usuarioRepository = service.GetRequiredService<IUsuarioRepository>();
            var authService = service.GetRequiredService<IAuthCommandService>();

            string email = "visitante";
            string senha = "123456";

            if (usuarioRepository.Get(u => u.Email == email).FirstOrDefault() != null)
                return;

            var (Salt, PasswordHash) = new PasswordHasherHelper().CriarHashSenha(senha);

            var usuario = new Usuario
            {
                Email = email,
                Password = PasswordHash,
                Salt = Salt,
                Permissoes = []
            };

            usuarioRepository.InsertAsync(usuario).Wait();
            usuarioRepository.SaveChangesAsync().Wait();
        }

        private async Task PrepararCategorias(IServiceProvider service)
        {
            var categoriaRepository = service.GetRequiredService<ICategoriaRepository>();

            var list = await categoriaRepository.Get().ToListAsync();

            if (list.Count < 1)
            {
                var listCategoria = new List<Categoria>
                {
                    new() { Descricao = "Almoço/Janta" },
                    new() { Descricao = "Condomínio" },
                    new() { Descricao = "Aluguel" },
                    new() { Descricao = "Limpeza" },
                    new() { Descricao = "Lanches" },
                    new() { Descricao = "Higiêne" },
                    new() { Descricao = "Conta de Luz" }
                };

                await categoriaRepository.InsertRangeAsync(listCategoria);
                await categoriaRepository.SaveChangesAsync();
            }
        }

        private async Task PrepararMembros(IServiceProvider service)
        {
            var memberRepository = service.GetRequiredService<IMembroRepository>();

            var list = await memberRepository.Get().ToListAsync();

            if (list.Count < 1)
            {
                var dataInicio = DateTimeZoneConverterPtBR.GetBrasiliaDateTimeZone();

                var listMember = new List<Membro>
                {
                    new()
                    {
                        Nome = "Bruno",
                        Telefone = "(38) 99805-5965",
                        DataInicio = dataInicio
                    },
                    new()
                    {
                        Nome = "Denis",
                        Telefone = "(38) 997282407",
                        DataInicio = dataInicio
                    },
                    new()
                    {
                        Nome = "Valdirene",
                        Telefone = "(31) 99797-7731",
                        DataInicio = dataInicio
                    },
                    new()
                    {
                        Nome = "Peu",
                        Telefone = "(38) 99995-4309",
                        DataInicio = dataInicio
                    },
                    new()
                    {
                        Nome = "Jhon Lenon",
                        Telefone = "(31) 99566-4815",
                        DataInicio = dataInicio
                    }
                };

                await memberRepository.InsertRangeAsync(listMember);
                await memberRepository.SaveChangesAsync();
            }
        }

        private async Task PrepararGruposFaturas(IServiceProvider service)
        {
            var grupoFaturaRepository = service.GetRequiredService<IGrupoFaturaRepository>();

            var list = await grupoFaturaRepository.Get().ToListAsync();

            if (list.Count < 1)
            {
                var dataCriacao = DateTimeZoneConverterPtBR.GetBrasiliaDateTimeZone();

                string mesAtualName = dataCriacao.ToString("MMMM", new CultureInfo("pt-BR"));

                mesAtualName = char.ToUpper(mesAtualName[0]) + mesAtualName[1..].ToLower();

                var grupoFatura = new GrupoFatura
                {
                    Nome = $"Fatura de {mesAtualName} {DateTime.Now.Year}",
                    DataCriacao = dataCriacao,
                    Ano = DateTime.Now.Year.ToString(),
                    StatusFaturas =
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
                    ]
                };

                await grupoFaturaRepository.InsertAsync(grupoFatura);
                await grupoFaturaRepository.SaveChangesAsync();
            }
        }

        private async Task PrepararParametroDeAlertaDeGastos(IServiceProvider service)
        {
            var parametroDeAlertaDeGastosRepository =
                service.GetRequiredService<IParametroDeAlertaDeGastosRepository>();

            var list = await parametroDeAlertaDeGastosRepository.Get().ToListAsync();

            if (list.Count < 1)
            {
                var listMetricasPersonalizadas = new List<ParametroDeAlertaDeGastos>
                {
                    new()
                    {
                        TipoMetrica = EnumTipoMetrica.Casa.ToString(),
                        LimiteVermelho = 3600,
                        LimiteAmarelo = 3240
                    },
                    new()
                    {
                        TipoMetrica = EnumTipoMetrica.Moradia.ToString(),
                        LimiteVermelho = 2900,
                        LimiteAmarelo = 2610
                    },
                    new()
                    {
                        TipoMetrica = EnumTipoMetrica.Geral.ToString(),
                        LimiteVermelho = 5700,
                        LimiteAmarelo = 5130
                    },
                };

                await parametroDeAlertaDeGastosRepository.InsertRangeAsync(
                    listMetricasPersonalizadas
                );
                await parametroDeAlertaDeGastosRepository.SaveChangesAsync();
            }
        }

        private async Task PrepararProdutoListaCompras(IServiceProvider service)
        {
            var ProdutoListaComprasRepository =
                service.GetRequiredService<IProdutoListaComprasRepository>();

            var list = await ProdutoListaComprasRepository.Get().ToListAsync();

            if (list.Count < 1)
            {
                var itensIniciais = new List<ProdutoListaCompras>
                {
                    new() { Item = "Arroz" },
                    new() { Item = "Feijão" },
                    new() { Item = "Sabonete" },
                    new() { Item = "Macarrão" },
                    new() { Item = "Molho de Tomate" },
                    new() { Item = "Sabão Líquido para Roupas" },
                    new() { Item = "Amaciante" },
                    new() { Item = "Bolacha" },
                    new() { Item = "Óleo" },
                    new() { Item = "Sal" },
                    new() { Item = "Papel Higiênico" },
                    new() { Item = "Ovos" },
                    new() { Item = "Trigo" },
                    new() { Item = "Fubá" },
                    new() { Item = "Leite" },
                    new() { Item = "Detergente" },
                    new() { Item = "Desinfetante" },
                    new() { Item = "Açúcar" },
                    new() { Item = "Pasta de Dente" },
                    new() { Item = "Café" },
                    new() { Item = "Caldo de Galinha" }
                };

                await ProdutoListaComprasRepository.InsertRangeAsync(itensIniciais);
                await ProdutoListaComprasRepository.SaveChangesAsync();
            }
        }

        private EnumPermissoes[] PrepararPermissoes()
        {
            return
            [
                // Categoria
                EnumPermissoes.CATEGORIA_000001,
                EnumPermissoes.CATEGORIA_000002,
                EnumPermissoes.CATEGORIA_000003,

                // Membro
                EnumPermissoes.MEMBRO_000001,
                EnumPermissoes.MEMBRO_000002,
                EnumPermissoes.MEMBRO_000003,

                // Despesa
                EnumPermissoes.DESPESA_000001,
                EnumPermissoes.DESPESA_000002,
                EnumPermissoes.DESPESA_000003,
                EnumPermissoes.DESPESA_000004,
                 
                // Grupo Fatura
                EnumPermissoes.GRUPOFATURA_000001,
                EnumPermissoes.GRUPOFATURA_000002,
                EnumPermissoes.GRUPOFATURA_000003,

                // Status Fatura
                EnumPermissoes.STATUSFATURA_000001,
                EnumPermissoes.STATUSFATURA_000002,
                EnumPermissoes.STATUSFATURA_000003,

                // Lista de Compras
                EnumPermissoes.LISTACOMPRA_000001,
                EnumPermissoes.LISTACOMPRA_000002,
                EnumPermissoes.LISTACOMPRA_000003,

                ////Cobrtança
                //EnumPermissoes.COBRANCA_000001,
                //EnumPermissoes.COBRANCA_000002,
                //EnumPermissoes.COBRANCA_000003,
                //EnumPermissoes.COBRANCA_000004,

                ////Pagamento
                //EnumPermissoes.PAGAMENTO_000001,
                //EnumPermissoes.PAGAMENTO_000002,
                //EnumPermissoes.PAGAMENTO_000003,
                //EnumPermissoes.PAGAMENTO_000004,
            ];
        }
    }
}
