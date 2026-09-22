namespace System.Runtime.Remoting
{
    /// <summary>
    /// System.Runtime.Remoting.RemotingServices equivalent.
    ///
    /// .NET Remoting does not exist on .NET, and with it went the transparent proxy - the
    /// object that stood in for an instance living in another AppDomain. Code that asks
    /// "is this a transparent proxy?" is asking about a kind of object that can no longer be
    /// created, so the true answer is always no. n2cms's vendored Castle DynamicProxy asks
    /// it before deciding how to intercept an instance.
    ///
    /// Declared in the ORIGINAL namespace, like CallContext: the ported "using
    /// System.Runtime.Remoting;" keeps meaning what it meant.
    /// </summary>
    public static class RemotingServices
    {
        public static bool IsTransparentProxy(object proxy) => false;

        public static bool IsObjectOutOfAppDomain(object tp) => false;

        /// <summary>Always null: there are no transparent proxies to have a real proxy behind them.</summary>
        public static Proxies.RealProxy GetRealProxy(object proxy) => null;
    }

    /// <summary>System.Runtime.Remoting.IRemotingTypeInfo equivalent.</summary>
    public interface IRemotingTypeInfo
    {
        string TypeName { get; set; }

        bool CanCastTo(Type fromType, object o);
    }
}

namespace System.Runtime.Remoting.Proxies
{
    /// <summary>
    /// System.Runtime.Remoting.Proxies.RealProxy equivalent - declared so the signatures that
    /// name it compile. Nothing can produce one on .NET (see RemotingServices).
    /// </summary>
    public abstract class RealProxy
    {
        protected RealProxy()
        {
        }

        protected RealProxy(Type classToProxy) => ProxiedType = classToProxy;

        public Type ProxiedType { get; }

        public Type GetProxiedType() => ProxiedType;
    }
}
