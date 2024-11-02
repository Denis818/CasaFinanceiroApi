using System.ComponentModel;

namespace Domain.Enumeradores
{
    public enum EnumPermissoes
    {
        /// <summary>
        /// Permissão para criar.
        /// </summary>
        [Description("Permissao para criar.")]
        USU_000001 = 1,

        /// <summary>
        /// Permissão para atualizar.
        /// </summary>
        [Description("Permissao para atualizar.")]
        USU_000002 = 2,

        /// <summary>
        /// Permissão para deletar.
        /// </summary>
        [Description("Permissao para deletar.")]
        USU_000003 = 3,

        /// <summary>
        /// Permissão para deletar.
        /// </summary>
        [Description("Permissao para Tela de calculos.")]
        USU_000004 = 4,

        /// <summary>
        /// Permissao para editar compra em Tela de calculos.
        /// </summary>
        [Description("Permissao para fazer alterações em Tela de calculos.")]
        USU_000005 = 5,
    }
}
