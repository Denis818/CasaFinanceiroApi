using Microsoft.AspNetCore.Mvc;

namespace Presentation.Attributes.Util
{
    public class ApiVersionRouteAttribute(string template) : RouteAttribute("api/v1/" + template)
    {
    }
}
