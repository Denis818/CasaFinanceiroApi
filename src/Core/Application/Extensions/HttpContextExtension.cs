using Microsoft.AspNetCore.Http;

namespace Application.Extensions
{
    public static class HttpContextExtension
    {
        public static Guid GetCurrentGrupoFaturaCode(this HttpContext context)
        {
            return (Guid)(
                context.Items["grupo-fatura-code"]
                ?? new Guid("00000000-0000-0000-0000-000000000000")
            );
        }
    }
}
