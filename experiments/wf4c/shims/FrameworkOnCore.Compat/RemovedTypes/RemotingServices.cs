namespace System.Runtime.Remoting
{
    /// <summary>
    /// .NET Framework's RemotingServices, where .NET answers: there is no .NET Remoting, so no object is a
    /// transparent proxy and none has a real proxy (proxy libraries ask before unwrapping their own
    /// proxies: N2's DynamicProxy, Castle DynamicProxy). Marshalling objects (Marshal, Connect,
    /// Disconnect) is not here: the code using it stays reported (it needs another transport).
    /// </summary>
    public static class RemotingServices
    {
        public static bool IsTransparentProxy(object proxy) => false;

        public static bool IsObjectOutOfAppDomain(object tp) => false;

        public static bool IsObjectOutOfContext(object tp) => false;

        public static Proxies.RealProxy GetRealProxy(object proxy) => null;
    }

    /// <summary>.NET Framework's IRemotingTypeInfo (a real proxy's type information).</summary>
    public interface IRemotingTypeInfo
    {
        string TypeName { get; set; }

        bool CanCastTo(Type fromType, object o);
    }
}

namespace System.Runtime.Remoting.Proxies
{
    /// <summary>
    /// .NET Framework's RealProxy, for code that names it: there are none on .NET
    /// (RemotingServices.GetRealProxy finds none). System.Reflection.DispatchProxy is .NET's proxy.
    /// </summary>
    public abstract class RealProxy
    {
        protected RealProxy() { }

        protected RealProxy(Type classToProxy) { }

        public virtual object GetTransparentProxy() => throw new PlatformNotSupportedException("There is no .NET Remoting on .NET: no transparent proxy.");
    }
}
