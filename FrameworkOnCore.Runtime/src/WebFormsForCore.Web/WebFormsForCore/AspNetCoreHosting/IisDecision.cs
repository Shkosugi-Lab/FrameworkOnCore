#if NETCOREAPP

using Core = Microsoft.AspNetCore.Http;

namespace System.Web.Hosting
{
	/// <summary>
	/// Which of IIS's modules the step that answered a request stands for (RequestFilteringModule, RewriteModule,
	/// HttpRedirectionModule, ProtocolSupportModule, StaticFile, ManagedPipeline: ASP.NET's modules and handlers): the last
	/// one set. Nothing reads it but the tests, which hold the port's steps to the module IIS's own trace of the same
	/// request names (tests/RuntimeUnitTests/Security/iis-golden/IIS-PIPELINE.md): the same answer for another module's
	/// reason is a step out of IIS's order.
	/// </summary>
	internal sealed class IisDecision
	{
		public const string RequestFiltering = "RequestFilteringModule", Rewrite = "RewriteModule", Redirect = "HttpRedirectionModule",
			ProtocolSupport = "ProtocolSupportModule", StaticFile = "StaticFile", Managed = "ManagedPipeline";

		public string Module;

		internal static void Set(Core.HttpContext context, string module)
		{
			if (context == null) return;
			var decision = context.Features.Get<IisDecision>();
			if (decision == null) context.Features.Set(decision = new IisDecision());
			decision.Module = module;
		}
	}
}

#endif
