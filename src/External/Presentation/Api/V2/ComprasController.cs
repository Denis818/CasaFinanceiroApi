using Asp.Versioning;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories.Compras;
using Domain.Models.Compras;
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
    [ApiVersionRoute("compras", "v2")]
    [PermissoesFinance(EnumPermissoes.USU_000004)]
    public class ComprasController(
        IServiceProvider serviceProvider,
        ICompraRepository compraRepository,
        IRecebimentoRepository recebimentoRepository
    ) : MainController(serviceProvider)
    {
        #region Compras

        [HttpGet]
        public async Task<IEnumerable<Compra>> GetAllCompras() =>
            await compraRepository.Get(compra => !compra.DividioPorDois).ToListAsync();

        [HttpGet("divididas-por-dois")]
        public async Task<IEnumerable<Compra>> GetAllComprasDvididasPorDois() =>
            await compraRepository.Get(compra => compra.DividioPorDois).ToListAsync();

        [HttpPost]
        public async Task<bool> InsertCompra(Compra compra)
        {
            compra.ValorPorParcela = compra.ValorTotal / compra.Parcelas;

            await compraRepository.InsertAsync(compra);
            return await compraRepository.SaveChangesAsync();
        }

        [HttpPut("{code:Guid}")]
        public async Task<bool> UpdateCompra(Guid code, [FromBody] Compra compra)
        {
            var existingCompra = await compraRepository.GetByCodigoAsync(code);
            if (existingCompra == null)
            {
                Notificar(EnumTipoNotificacao.NotFount, "Não encontrado");
                return false;
            }

            existingCompra.Nome = compra.Nome;
            existingCompra.Parcelas = compra.Parcelas;
            existingCompra.ValorTotal = compra.ValorTotal;
            existingCompra.ValorPorParcela = compra.ValorTotal / compra.Parcelas;

            compraRepository.Update(existingCompra);
            return await compraRepository.SaveChangesAsync();
        }

        [HttpDelete("{code:Guid}")]
        public async Task<bool> DeleteCompra(Guid code)
        {
            var compra = await compraRepository.GetByCodigoAsync(code);
            if (compra == null)
            {
                Notificar(EnumTipoNotificacao.NotFount, "Não encontrado");
                return false;
            }

            compraRepository.Delete(compra);
            return await compraRepository.SaveChangesAsync();
        }

        #endregion

        #region Recebimentos

        [HttpGet("recebimentos")]
        public async Task<IEnumerable<Recebimento>> GetAllRecebimentos() =>
            await recebimentoRepository.Get().ToListAsync();

        [HttpPost("recebimentos")]
        public async Task<bool> InsertRecebimento(Recebimento recebimento)
        {
            await recebimentoRepository.InsertAsync(recebimento);
            return await recebimentoRepository.SaveChangesAsync();
        }

        [HttpPut("recebimentos/{code:Guid}")]
        public async Task<bool> UpdateRecebimento(Guid code, [FromBody] Recebimento recebimento)
        {
            var existingRecebimento = await recebimentoRepository.GetByCodigoAsync(code);
            if (existingRecebimento == null)
            {
                Notificar(EnumTipoNotificacao.NotFount, "Não encontrado");
                return false;
            }

            existingRecebimento.Data = recebimento.Data;
            existingRecebimento.Valor = recebimento.Valor;

            recebimentoRepository.Update(existingRecebimento);
            return await recebimentoRepository.SaveChangesAsync();
        }

        [HttpDelete("recebimentos/{code:Guid}")]
        public async Task<bool> DeleteRecebimento(Guid code)
        {
            var recebimento = await recebimentoRepository.GetByCodigoAsync(code);
            if (recebimento == null)
            {
                Notificar(EnumTipoNotificacao.NotFount, "Não encontrado");
                return false;
            }

            recebimentoRepository.Delete(recebimento);
            return await recebimentoRepository.SaveChangesAsync();
        }

        #endregion
    }
}
