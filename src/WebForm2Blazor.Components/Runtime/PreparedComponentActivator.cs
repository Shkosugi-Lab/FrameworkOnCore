using Microsoft.AspNetCore.Components;

namespace WebForm2Blazor.Components;

/// <summary>
/// Lets a control that was BUILT AND CONFIGURED in code be the instance Blazor renders.
///
/// WebForms code does this constantly:
///
///     PostViewBase postView = (PostViewBase)LoadControl(path);
///     postView.Post = Post;                 // ordinary property, not a parameter
///     pwPost.Controls.Add(postView);
///
/// Blazor has no way to render a component object you already hold - every path
/// (OpenComponent, DynamicComponent) constructs its own. Handing the state over as
/// parameters was tried first and is not enough: almost nothing a WebForms control
/// carries is a [Parameter], so the fresh instance got a null Post and BlogEngine's
/// home page died in PostViewBase.OnInit.
///
/// IComponentActivator is the one place the renderer asks for an instance. Registering
/// the prepared control here means the renderer receives that exact object, with the
/// state already on it, and OnInitialized sees what the page set.
///
/// Registration is idempotent and an instance is handed out once, so a re-render that
/// reuses the existing component (no CreateInstance call) leaves at most one entry per
/// live child rather than accumulating.
/// </summary>
public sealed class PreparedComponentActivator : IComponentActivator
{
    private readonly Dictionary<Type, List<IComponent>> _prepared = [];

    /// <summary>
    /// Offers an already-built control as the next instance of its type.
    ///
    /// Called while building the render tree, immediately before the frame that will
    /// cause it to be instantiated.
    /// </summary>
    public void Register(IComponent component)
    {
        if (component is null)
        {
            return;
        }

        var type = component.GetType();
        if (!_prepared.TryGetValue(type, out var pending))
        {
            _prepared[type] = pending = [];
        }

        if (!pending.Contains(component))
        {
            pending.Add(component);
        }
    }

    /// <summary>
    /// Hands back a registered instance of this type if one is waiting, otherwise
    /// constructs one - which is every component in the app that was not built in code.
    /// </summary>
    public IComponent CreateInstance(Type componentType)
    {
        if (_prepared.TryGetValue(componentType, out var pending) && pending.Count > 0)
        {
            var component = pending[0];
            pending.RemoveAt(0);
            return component;
        }

        return (IComponent)Activator.CreateInstance(componentType)!;
    }
}
