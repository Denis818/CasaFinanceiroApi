using System.ComponentModel;

namespace Domain.Enumeradores
{
    public enum EnumPermissoes
    {
        #region Categoria

        /// <summary>
        /// Permissão para criar categoria.
        /// </summary>
        [Description("Permissão para criar categoria.")]
        CATEGORIA_000001,

        /// <summary>
        /// Permissão para atualizar categoria.
        /// </summary>
        [Description("Permissão para atualizar categoria.")]
        CATEGORIA_000002,

        /// <summary>
        /// Permissão para deletar categoria.
        /// </summary>
        [Description("Permissão para deletar categoria.")]
        CATEGORIA_000003,

        #endregion

        #region Membro

        /// <summary>
        /// Permissão para criar membro.
        /// </summary>
        [Description("Permissão para criar membro.")]
        MEMBRO_000001,

        /// <summary>
        /// Permissão para atualizar membro.
        /// </summary>
        [Description("Permissão para atualizar membro.")]
        MEMBRO_000002,

        /// <summary>
        /// Permissão para deletar membro.
        /// </summary>
        [Description("Permissão para deletar membro.")]
        MEMBRO_000003,

        #endregion

        #region Despesa

        /// <summary>
        /// Permissão para criar despesa.
        /// </summary>
        [Description("Permissão para criar despesa.")]
        DESPESA_000001,

        /// <summary>
        /// Permissão para atualizar despesa.
        /// </summary>
        [Description("Permissão para atualizar despesa.")]
        DESPESA_000002,

        /// <summary>
        /// Permissão para deletar despesa.
        /// </summary>
        [Description("Permissão para deletar despesa.")]
        DESPESA_000003,

        /// <summary>
        /// Permissão para editar métricas do alerta de gastos.
        /// </summary>
        [Description("Permissão para editar métricas do alerta de gastos.")]
        DESPESA_000004,

        #endregion

        #region Grupo Fatura

        /// <summary>
        /// Permissão para criar grupo de fatura.
        /// </summary>
        [Description("Permissão para criar grupo de fatura.")]
        GRUPOFATURA_000001,

        /// <summary>
        /// Permissão para atualizar grupo de fatura.
        /// </summary>
        [Description("Permissão para atualizar grupo de fatura.")]
        GRUPOFATURA_000002,

        /// <summary>
        /// Permissão para deletar grupo de fatura.
        /// </summary>
        [Description("Permissão para deletar grupo de fatura.")]
        GRUPOFATURA_000003,

        #endregion

        #region Status Fatura

        /// <summary>
        /// Permissão para criar status de fatura.
        /// </summary>
        [Description("Permissão para criar status de fatura.")]
        STATUSFATURA_000001,

        /// <summary>
        /// Permissão para atualizar status de fatura.
        /// </summary>
        [Description("Permissão para atualizar status de fatura.")]
        STATUSFATURA_000002,

        /// <summary>
        /// Permissão para deletar status de fatura.
        /// </summary>
        [Description("Permissão para deletar status de fatura.")]
        STATUSFATURA_000003,

        #endregion

        #region Lista de Compras

        /// <summary>
        /// Permissão para criar lista de compras.
        /// </summary>
        [Description("Permissão para criar lista de compras.")]
        LISTACOMPRA_000001,

        /// <summary>
        /// Permissão para atualizar lista de compras.
        /// </summary>
        [Description("Permissão para atualizar lista de compras.")]
        LISTACOMPRA_000002,

        /// <summary>
        /// Permissão para deletar lista de compras.
        /// </summary>
        [Description("Permissão para deletar lista de compras.")]
        LISTACOMPRA_000003,

        #endregion

        #region Cobrança

        /// <summary>
        /// Permissão para criar cobrança.
        /// </summary>
        [Description("Permissão para criar cobrança.")]
        COBRANCA_000001,

        /// <summary>
        /// Permissão para atualizar cobrança.
        /// </summary>
        [Description("Permissão para atualizar cobrança.")]
        COBRANCA_000002,

        /// <summary>
        /// Permissão para deletar cobrança.
        /// </summary>
        [Description("Permissão para deletar cobrança.")]
        COBRANCA_000003,

        /// <summary>
        /// Permissão para vizualizar cobrança.
        /// </summary>
        [Description("Permissão para vizualizar cobrança.")]
        COBRANCA_000004,

        #endregion

        #region Pagamento

        /// <summary>
        /// Permissão para criar pagamento.
        /// </summary>
        [Description("Permissão para criar pagamento.")]
        PAGAMENTO_000001,

        /// <summary>
        /// Permissão para atualizar pagamento.
        /// </summary>
        [Description("Permissão para atualizar pagamento.")]
        PAGAMENTO_000002,

        /// <summary>
        /// Permissão para deletar pagamento.
        /// </summary>
        [Description("Permissão para deletar pagamento.")]
        PAGAMENTO_000003,

        /// <summary>
        /// Permissão para vizualizar pagamento.
        /// </summary>
        [Description("Permissão para vizualizar pagamento.")]
        PAGAMENTO_000004

        #endregion
    }
}
