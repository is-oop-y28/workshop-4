using Microsoft.AspNetCore.Components;

namespace Workshop4.Presentation.Services;

public static class DynamicRenderFragmentFactory
{
    public static RenderFragment Create<TComponent>(IDictionary<string, object> parameters) => builder =>
    {
        builder.OpenComponent<DynamicComponent>(0);
        builder.AddAttribute(1, nameof(DynamicComponent.Type), typeof(TComponent));
        builder.AddAttribute(2, nameof(DynamicComponent.Parameters), parameters);
        builder.CloseComponent();
    };
}
