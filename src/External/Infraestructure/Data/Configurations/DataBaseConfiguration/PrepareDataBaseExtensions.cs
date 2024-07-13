using Application.Helpers;
using Application.Interfaces.Services.User;
using Domain.Converters.DatesTimes;
using Domain.Dtos.User.Auth;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Categorias;
using Domain.Models.Despesas;
using Domain.Models.Membros;
using Domain.Models.Users;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace Infraestructure.Data.Configurations.DataBaseConfiguration
{
    public class PrepareDataBaseExtensions
    {
        public static void PrepareDataBase(IServiceProvider service, string nomeDominio)
        {
            PrepareUserMaster(service, nomeDominio);
            PrepararVisitante(service);
            PrepareCategoryAndMember(service).Wait();
        }

        private static void PrepareUserMaster(IServiceProvider service, string nomeDominio)
        {
            var usuarioRepository = service.GetRequiredService<IUsuarioRepository>();
            var authService = service.GetRequiredService<IAuthAppService>();

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

            var permissoes = new EnumPermissoes[]
            {
                EnumPermissoes.USU_000001,
                EnumPermissoes.USU_000002,
                EnumPermissoes.USU_000003,
            };

            usuarioRepository.InsertAsync(usuario).Wait();
            usuarioRepository.SaveChangesAsync().Wait();

            authService.AddPermissaoAsync(new AddUserPermissionDto(usuario.Id, permissoes)).Wait();
        }

        private static void PrepararVisitante(IServiceProvider service)
        {
            var usuarioRepository = service.GetRequiredService<IUsuarioRepository>();
            var authService = service.GetRequiredService<IAuthAppService>();

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

        private static async Task PrepareCategoryAndMember(IServiceProvider service)
        {
            var categoriaRepository = service.GetRequiredService<ICategoriaRepository>();
            var memberRepository = service.GetRequiredService<IMembroRepository>();
            var grupoFaturaRepository = service.GetRequiredService<IGrupoFaturaRepository>();

            if (categoriaRepository.Get().ToList().Count < 1)
            {
                var listCategoria = new List<Categoria>
                {
                    new() { Descricao = "Almoço/Janta" },
                    new() { Descricao = "Condomínio" },
                    new() { Descricao = "Aluguel" },
                    new() { Descricao = "Limpeza" },
                    new() { Descricao = "Lanches" },
                    new() { Descricao = "Higiêne" },
                    new() { Descricao = "Internet" },
                    new() { Descricao = "Conta de Luz" }
                };

                await categoriaRepository.InsertRangeAsync(listCategoria);
                await categoriaRepository.SaveChangesAsync();
            }

            if (memberRepository.Get().ToList().Count < 1)
            {
                var dataInicio = DateTimeZoneProvider.GetBrasiliaDateTimeZone();

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

            if (grupoFaturaRepository.Get().ToList().Count < 1)
            {
                var dataCriacao = DateTimeZoneProvider.GetBrasiliaDateTimeZone();

                string mesAtualName = dataCriacao.ToString("MMMM", new CultureInfo("pt-BR"));

                mesAtualName = char.ToUpper(mesAtualName[0]) + mesAtualName[1..].ToLower();

                var grupoFatura = new GrupoFatura
                {
                    Nome = $"Fatura de {mesAtualName} {DateTime.Now.Year}",
                    DataCriacao = dataCriacao,
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
    }
}
