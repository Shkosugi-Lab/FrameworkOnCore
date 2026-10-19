namespace ConfigProbe.Sub
{
    /// <summary>The same cases in Sub: Sub/web.config (de-DE, its own appSettings and probe/shop) after the root's location.</summary>
    public partial class Default : ConfigPage
    {
        protected override void Run() => Common("~/Sub");
    }
}