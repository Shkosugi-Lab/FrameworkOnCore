using System;
using System.Web;

// Compiled by ASP.NET when the application starts (App_Code, not the project's assembly), C# 5 as .NET Framework's own
// compiler has it. Its classes are what the pages compiled at run time see.
public static class Greeter
{
    public static string Hello(string name)
    {
        return "Hello, " + name + " (App_Code, assembly " + (typeof(Greeter).Assembly.GetName().Name.StartsWith("App_Code") ? "App_Code" : "other") + ")";
    }

    public static string Culture()
    {
        return "culture " + System.Threading.Thread.CurrentThread.CurrentCulture.Name + ", ui culture " + System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
    }
}
