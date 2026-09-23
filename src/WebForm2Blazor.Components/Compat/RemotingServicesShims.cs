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

        /// <summary>
        /// RemotingServices.Marshal / Disconnect: publishing an object for remote callers.
        /// There is no remoting channel to publish on, so these throw - quietly returning
        /// would leave a server that believes it is listening. DNN's vendored log4net
        /// reaches them only when a RemoteLoggingServerPlugin is configured.
        /// </summary>
        public static object Marshal(MarshalByRefObject obj, string uri)
            => throw new PlatformNotSupportedException(".NET Remoting is not available on .NET (RemotingServices.Marshal).");

        /// <inheritdoc cref="Marshal(MarshalByRefObject, string)"/>
        public static object Marshal(MarshalByRefObject obj, string uri, Type requestedType)
            => throw new PlatformNotSupportedException(".NET Remoting is not available on .NET (RemotingServices.Marshal).");

        /// <inheritdoc cref="Marshal(MarshalByRefObject, string)"/>
        public static bool Disconnect(MarshalByRefObject obj)
            => throw new PlatformNotSupportedException(".NET Remoting is not available on .NET (RemotingServices.Disconnect).");
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

namespace System
{
    /// <summary>
    /// Members .NET removed from its own types, restored as C# 14 extension members - the only
    /// way to add a STATIC member (Activator.GetObject) or a property (ConfigurationFile) to a
    /// type this layer does not own.
    ///
    /// Declared in the extended type's own namespace, like RemotingServices above: a ported
    /// file that calls them imports System, and need not import anything of this layer's.
    /// </summary>
    public static class RemovedSystemMembers
    {
        extension(Activator)
        {
            /// <summary>
            /// Activator.GetObject: a proxy for a remote object. .NET Remoting is gone, so this
            /// throws where the original would have connected - DNN's vendored log4net
            /// RemotingAppender reaches it only when configured to log to a remote sink.
            /// </summary>
            public static object GetObject(Type type, string url)
                => throw new PlatformNotSupportedException(".NET Remoting is not available on .NET (Activator.GetObject).");

            /// <inheritdoc cref="GetObject(Type, string)"/>
            public static object GetObject(Type type, string url, object state)
                => throw new PlatformNotSupportedException(".NET Remoting is not available on .NET (Activator.GetObject).");
        }

        extension(AppDomainSetup setup)
        {
            /// <summary>
            /// AppDomainSetup.ConfigurationFile: the application's configuration file. On .NET
            /// that is the file System.Configuration itself reads for the process, which is
            /// what this returns (log4net watches it for its own section).
            /// </summary>
            public string ConfigurationFile
                => System.Configuration.ConfigurationManager
                    .OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None).FilePath;
        }
    }
}

namespace System.Data.Common
{
    /// <summary>DbProviderFactory members .NET removed (see RemovedSystemMembers for the pattern).</summary>
    public static class RemovedDbProviderFactoryMembers
    {
        // SYSLIB0003: obsolete because CAS is gone - which is what this answers for.
#pragma warning disable SYSLIB0003
        extension(DbProviderFactory factory)
        {
            /// <summary>
            /// DbProviderFactory.CreatePermission, removed with Code Access Security. There is
            /// no permission to create on .NET - nothing demands one - so it is null, and a
            /// wrapper that forwards to it (YAF's vendored ServiceStack.OrmLite profiler does)
            /// compiles and answers the same null. .NET itself never calls it.
            /// </summary>
            public System.Security.CodeAccessPermission CreatePermission(System.Security.Permissions.PermissionState state)
                => null;
        }
#pragma warning restore SYSLIB0003
    }
}
