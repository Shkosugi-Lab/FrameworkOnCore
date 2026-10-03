namespace WcfProbe
{
    // Activated by web.config (serviceActivations: Greeter.svc, no file), its endpoint WCF 4's default one.
    public class Greeter : IGreeter
    {
        public string Greet(string name)
        {
            return "Hello, " + name + "!";
        }
    }
}
