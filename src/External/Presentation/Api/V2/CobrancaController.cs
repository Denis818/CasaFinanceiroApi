using Asp.Versioning;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories.Cobrancas;
using Domain.Models.Cobrancas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.Api.Base;
using Presentation.Attributes.Auth;
using Presentation.Attributes.Util;
using Presentation.Version;

namespace Presentation.Api.V2
{
    [ApiController]
    [AutorizationFinance]
    [ApiVersion(ApiVersioning.V2)]
    [ApiVersionRoute("cobrancas", "v2")]
    [PermissoesFinance(EnumPermissoes.COBRANCA_000004, EnumPermissoes.PAGAMENTO_000004)]
    public class CobrancaController(
        IServiceProvider serviceProvider,
        ICobrancaRepository cobrancaRepository,
        IPagamentoRepository pagamentoRepository
    ) : MainController(serviceProvider)
    {
        #region Cobrança

        [HttpGet]
        public async Task<IEnumerable<Cobranca>> GetAllCompras() =>
            await cobrancaRepository.Get(cobranca => !cobranca.DividioPorDois).ToListAsync();

        [HttpGet("divididas-por-dois")]
        public async Task<IEnumerable<Cobranca>> GetAllComprasDvididasPorDois() =>
            await cobrancaRepository.Get(cobranca => cobranca.DividioPorDois).ToListAsync();

        [HttpPost]
        [PermissoesFinance(EnumPermissoes.COBRANCA_000001)]
        public async Task<bool> InsertCobranca(Cobranca cobranca)
        {
            cobranca.ValorPorParcela = cobranca.ValorTotal / cobranca.Parcelas;

            await cobrancaRepository.InsertAsync(cobranca);
            return await cobrancaRepository.SaveChangesAsync();
        }

        [HttpPut]
        [PermissoesFinance(EnumPermissoes.COBRANCA_000002)]
        public async Task<bool> UpdateCobranca(Guid code, [FromBody] Cobranca cobranca)
        {
            var existingCompra = await cobrancaRepository.GetByCodigoAsync(code);
            if (existingCompra == null)
            {
                Notificar(EnumTipoNotificacao.NotFount, "Não encontrado");
                return false;
            }

            existingCompra.Nome = cobranca.Nome;
            existingCompra.Parcelas = cobranca.Parcelas;
            existingCompra.ValorTotal = cobranca.ValorTotal;
            existingCompra.DividioPorDois = cobranca.DividioPorDois;
            existingCompra.ValorPorParcela = cobranca.ValorTotal / cobranca.Parcelas;

            cobrancaRepository.Update(existingCompra);
            return await cobrancaRepository.SaveChangesAsync();
        }

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.COBRANCA_000003)]
        public async Task<bool> DeleteCobranca(Guid code)
        {
            var cobranca = await cobrancaRepository.GetByCodigoAsync(code);
            if (cobranca == null)
            {
                Notificar(EnumTipoNotificacao.NotFount, "Não encontrado");
                return false;
            }

            cobrancaRepository.Delete(cobranca);
            return await cobrancaRepository.SaveChangesAsync();
        }

        #endregion

        #region Pagamento

        [HttpGet("pagamento")]
        public async Task<IEnumerable<Pagamento>> GetAllPagamentos() =>
            await pagamentoRepository.Get().ToListAsync();

        [HttpPost("pagamento")]
        [PermissoesFinance(EnumPermissoes.PAGAMENTO_000001)]
        public async Task<bool> InsertPagamento(Pagamento pagamento)
        {
            if (pagamento.Data.Date > DateTime.Now)
            {
                Notificar(EnumTipoNotificacao.ClientError, "Data não pode ser futura.");
                return false;
            }
            await pagamentoRepository.InsertAsync(pagamento);
            return await pagamentoRepository.SaveChangesAsync();
        }

        [HttpPut("pagamento")]
        [PermissoesFinance(EnumPermissoes.PAGAMENTO_000002)]
        public async Task<bool> UpdatePagamento(Guid code, [FromBody] Pagamento pagamento)
        {
            var existingPagamento = await pagamentoRepository.GetByCodigoAsync(code);
            if (existingPagamento == null)
            {
                Notificar(EnumTipoNotificacao.NotFount, "Não encontrado");
                return false;
            }

            existingPagamento.Data = pagamento.Data;
            existingPagamento.Valor = pagamento.Valor;

            pagamentoRepository.Update(existingPagamento);
            return await pagamentoRepository.SaveChangesAsync();
        }

        [HttpDelete("pagamento")]
        [PermissoesFinance(EnumPermissoes.PAGAMENTO_000003)]
        public async Task<bool> DeletePagamento(Guid code)
        {
            var pagamento = await pagamentoRepository.GetByCodigoAsync(code);
            if (pagamento == null)
            {
                Notificar(EnumTipoNotificacao.NotFount, "Não encontrado");
                return false;
            }

            pagamentoRepository.Delete(pagamento);
            return await pagamentoRepository.SaveChangesAsync();
        }

        #endregion
    }
}
