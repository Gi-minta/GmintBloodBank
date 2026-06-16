using Application.Interfaces.Licensing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireFeatureFlagAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string _featureKey;

    public RequireFeatureFlagAttribute(string featureKey)
    {
        _featureKey = featureKey;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var featureFlagService = context.HttpContext.RequestServices.GetRequiredService<IFeatureFlagService>();
        var isEnabled = await featureFlagService.IsEnabledAsync(_featureKey);

        if (!isEnabled)
        {
            context.Result = new ForbidResult();
        }
    }
}
