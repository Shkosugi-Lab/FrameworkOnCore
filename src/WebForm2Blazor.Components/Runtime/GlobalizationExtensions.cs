using System;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebForm2Blazor.Components;

/// <summary>
/// The WebForms &lt;globalization culture uiCulture&gt; setting.
///
/// On 4.8 this is not a formatting hint, it is the culture every request thread runs
/// under, so it decides how every DateTime and every number in the application prints -
/// on every page. n2 runs sv-SE ("2026-09-14", "1 234,50"), YAF runs en-US
/// ("9/14/2026", "1,234.50"). Without it the converted application inherits whatever
/// culture the SERVER happens to have, so the same page renders different text on a
/// different machine. That is the one thing the conversion must never do.
/// </summary>
public static class GlobalizationExtensions
{
    /// <summary>Configuration section the converter writes &lt;globalization&gt; into.</summary>
    public const string SectionName = "WebFormsGlobalization";

    /// <summary>
    /// Applies the ported &lt;globalization&gt; culture. Called from the generated Program.cs;
    /// a no-op when the Web.config named no culture (the section is simply absent).
    /// </summary>
    public static IApplicationBuilder UseWebFormsGlobalization(this IApplicationBuilder app)
    {
        var section = app.ApplicationServices.GetService<IConfiguration>()?.GetSection(SectionName);
        return app.UseWebFormsGlobalization(section?["Culture"], section?["UICulture"]);
    }

    /// <summary>The same, with the two values supplied directly (used by the tests).</summary>
    public static IApplicationBuilder UseWebFormsGlobalization(
        this IApplicationBuilder app, string? culture, string? uiCulture)
    {
        if (string.IsNullOrWhiteSpace(culture) && string.IsNullOrWhiteSpace(uiCulture))
        {
            return app;
        }

        // WebForms "auto" (and "auto:fallback") reads Accept-Language per request. Its
        // ASP.NET Core equivalent is the request localization middleware - and in Blazor
        // Server the circuit inherits the culture of the request that created it, so
        // negotiating here reaches the interactive components too.
        var negotiate = IsAuto(culture) || IsAuto(uiCulture);

        // "auto:en-US" names the fallback for when the browser asks for nothing.
        var pinnedCulture = IsAuto(culture) ? null : Parse(culture);
        var pinnedUiCulture = IsAuto(uiCulture) ? null : Parse(uiCulture);
        var fallbackCulture = Parse(Fallback(culture)) ?? pinnedCulture ?? CultureInfo.CurrentCulture;
        var fallbackUiCulture = Parse(Fallback(uiCulture)) ?? pinnedUiCulture ?? fallbackCulture;

        // A pinned culture is the process default as well: ported business logic that
        // formats on a background thread (a timer, a cache refresh) ran under the
        // configured culture on 4.8 too, not under the machine's.
        if (pinnedCulture is not null)
        {
            CultureInfo.DefaultThreadCurrentCulture = pinnedCulture;
        }
        if (pinnedUiCulture is not null)
        {
            CultureInfo.DefaultThreadCurrentUICulture = pinnedUiCulture;
        }

        if (!negotiate)
        {
            // Nothing to negotiate - the defaults above already cover every thread.
            return app;
        }

        var options = new RequestLocalizationOptions
        {
            // Null means "accept whatever the browser asks for", which is what "auto"
            // does on 4.8. An explicit list would silently collapse unlisted languages
            // onto the default, which the original application did not do.
            SupportedCultures = null,
            SupportedUICultures = null,
            DefaultRequestCulture = new RequestCulture(fallbackCulture, fallbackUiCulture),
        };

        app.UseRequestLocalization(options);

        // The half-auto case (culture="auto" uiCulture="en"): the browser decided both,
        // so pin the side the configuration fixed back afterwards.
        if (pinnedCulture is not null || pinnedUiCulture is not null)
        {
            app.Use(async (context, next) =>
            {
                if (pinnedCulture is not null)
                {
                    CultureInfo.CurrentCulture = pinnedCulture;
                }
                if (pinnedUiCulture is not null)
                {
                    CultureInfo.CurrentUICulture = pinnedUiCulture;
                }
                await next();
            });
        }

        return app;
    }

    private static bool IsAuto(string? value)
        => value is not null && value.TrimStart().StartsWith("auto", StringComparison.OrdinalIgnoreCase);

    /// <summary>The explicit fallback in "auto:en-US", or the value itself when it is not auto.</summary>
    private static string? Fallback(string? value)
        => IsAuto(value)
            ? (value!.Contains(':') ? value[(value.IndexOf(':') + 1)..] : null)
            : value;

    private static CultureInfo? Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        try
        {
            return CultureInfo.GetCultureInfo(value.Trim());
        }
        catch (CultureNotFoundException)
        {
            // An unknown culture name is not worth failing startup over - 4.8 would have
            // thrown at request time, but the honest fallback is the server default.
            return null;
        }
    }
}
