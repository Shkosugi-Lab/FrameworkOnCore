using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebForm2Blazor.Components;

public static class ServiceCollectionExtensions
{
    private const string SessionCookieName = "w2b-session-id";

    /// <summary>
    /// Registers the WebForms compatibility runtime (Session / Application /
    /// ConfigurationManager). Called from the Program.cs the converter generates.
    /// </summary>
    public static IServiceCollection AddWebFormsCompat(this IServiceCollection services, IConfiguration configuration)
    {
        Compat.ConfigurationManager.Initialize(configuration);

        services.AddHttpContextAccessor();
        services.AddSingleton<WebFormsSessionStore>();
        services.AddSingleton<WebFormsApplicationState>();

        // The session is resolved via the session-id cookie (survives circuit re-creation).
        // Environments without an HttpContext (bUnit etc.) get an independent per-scope session.
        services.AddScoped(provider =>
        {
            var accessor = provider.GetService<IHttpContextAccessor>();
            var context = accessor?.HttpContext;
            var sessionId = context?.Items[SessionCookieName] as string
                            ?? context?.Request.Cookies[SessionCookieName];
            return provider.GetRequiredService<WebFormsSessionStore>().GetOrCreate(sessionId);
        });

        return services;
    }

    /// <summary>
    /// Middleware that issues the WebForms-compatible session cookie.
    /// Called before UseAntiforgery in the Program.cs the converter generates.
    /// </summary>
    public static IApplicationBuilder UseWebFormsSession(this IApplicationBuilder app)
    {
        // Enables the HttpContext.Current compatibility shim (User / Session access
        // from ported business logic)
        WebForm2Blazor.Components.HttpContext.Services = app.ApplicationServices;

        return app.Use(async (context, next) =>
        {
            var sessionId = context.Request.Cookies[SessionCookieName];
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString("N");
                context.Response.Cookies.Append(SessionCookieName, sessionId, new CookieOptions
                {
                    HttpOnly = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax,
                });
            }
            context.Items[SessionCookieName] = sessionId;
            await next();
        });
    }
}
