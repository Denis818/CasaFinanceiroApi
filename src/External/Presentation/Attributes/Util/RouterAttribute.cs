using Microsoft.AspNetCore.Mvc;

namespace Presentation.Attributes.Util
{
    public class ApiVersionRouteAttribute(string template, string versao = "v1") : RouteAttribute($"api/{versao}/" + template)
    {
    }
}
