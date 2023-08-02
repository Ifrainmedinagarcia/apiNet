using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace API.Utils;

public class SwaggerVersion : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        var nameSpaceController = controller.ControllerType.Namespace;
        var versionApi = nameSpaceController?.Split(".").Last().ToLower();
        controller.ApiExplorer.GroupName = versionApi;
    }
}