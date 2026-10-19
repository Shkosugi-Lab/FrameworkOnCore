using System.Configuration.Internal;

#nullable disable
namespace System.Configuration
{
	internal static class ConfigurationManagerInternalFactory
	{
#if NETFRAMEWORK
        private const string ConfigurationManagerInternalTypeString = "System.Configuration.Internal.ConfigurationManagerInternal, System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
#else
		// This assembly is System.Configuration.ConfigurationManager (not WebFormsForCore.Configuration): the type itself.
		private static readonly string ConfigurationManagerInternalTypeString = typeof(ConfigurationManagerInternal).AssemblyQualifiedName;
#endif
		private static volatile IConfigurationManagerInternal s_instance;

		internal static IConfigurationManagerInternal Instance
		{
			get
			{
				if (ConfigurationManagerInternalFactory.s_instance == null)
#if NETFRAMEWORK
					ConfigurationManagerInternalFactory.s_instance = (IConfigurationManagerInternal)TypeUtil.CreateInstanceWithReflectionPermission("System.Configuration.Internal.ConfigurationManagerInternal, System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
#else
					ConfigurationManagerInternalFactory.s_instance = (IConfigurationManagerInternal)TypeUtil.CreateInstanceWithReflectionPermission(ConfigurationManagerInternalTypeString);
#endif
				return ConfigurationManagerInternalFactory.s_instance;
			}
		}
	}
}
