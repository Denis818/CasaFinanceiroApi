using Asp.Versioning;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Presentation.Version
{
    public class ApiVersioningFilter : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var version = controller.Attributes.OfType<ApiVersionAttribute>().FirstOrDefault();

            if (version != null)
            {
                controller.ApiExplorer.GroupName = $"v{version.Versions.FirstOrDefault()}";
            }
        }
    }
}
