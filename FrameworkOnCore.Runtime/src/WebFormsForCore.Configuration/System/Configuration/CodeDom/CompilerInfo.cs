//------------------------------------------------------------------------------
// <copyright file="CompilerInfo.cs" company="Microsoft">
// 
// <OWNER>Microsoft</OWNER>
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

namespace WebFormsForCore.CodeDom.Compiler
{
	using System;
	using System.Reflection;
	using System.Security.Permissions;
	using System.CodeDom.Compiler;
	using System.Configuration;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Web;
    using System.Runtime.Loader;
    using System.IO;

    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	public sealed class CompilerInfo
	{
		internal String _codeDomProviderTypeName; // This can never by null
		internal CompilerParameters _compilerParams; // This can never by null
		internal String[] _compilerLanguages; // This can never by null
		internal String[] _compilerExtensions; // This can never by null
		internal String configFileName;
		internal IDictionary<string, string> _providerOptions;  // This can never be null
		internal int configFileLineNumber;
		internal Boolean _mapped;

		private Type type;

		private CompilerInfo() { } // Not createable

		public String[] GetLanguages()
		{
			return CloneCompilerLanguages();
		}

		public String[] GetExtensions()
		{
			return CloneCompilerExtensions();
		}

		// The Roslyn CodeDOM providers .NET Framework applications name in <system.codedom> (the
		// Microsoft.CodeDom.Providers.DotNetCompilerPlatform package, in every Visual Studio 2015+ web
		// template): WebFormsForCore's own Roslyn providers do the same on .NET.
		static string MapFrameworkProvider(string typeName)
		{
			var name = typeName.Split(',')[0].Trim();
			switch (name)
			{
				case "Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider":
					return "WebFormsForCore.CodeDom.Compiler.CSharpCodeProvider, " + AssemblyRef.WebFormsForCoreWeb;
				case "Microsoft.CodeDom.Providers.DotNetCompilerPlatform.VBCodeProvider":
					return "WebFormsForCore.CodeDom.Compiler.VBCodeProvider, " + AssemblyRef.WebFormsForCoreWeb;
				default:
					return typeName;
			}
		}

		public Type CodeDomProviderType
		{
			get
			{
				if (type == null)
				{
					lock (this)
					{
						if (type == null)
						{
                            //type = Type.GetType(_codeDomProviderTypeName);
                            var alc = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly());
							type = Type.GetType(MapFrameworkProvider(_codeDomProviderTypeName),
								assemblyName => {
									try
									{
										return alc.LoadFromAssemblyName(assemblyName);
									}
									catch (FileNotFoundException)
									{
										return null;
									}
								},
                                (assembly, name, ignoreCase) => assembly != null ? assembly.GetType(name, false, ignoreCase) : Type.GetType(name, false, ignoreCase),
								false);
							if (type == null)
							{
								if (configFileName == null)
								{
									throw new ConfigurationErrorsException(SR.GetString(SR.Unable_To_Locate_Type,
																	  _codeDomProviderTypeName, string.Empty, 0));
								}
								else
								{
									throw new ConfigurationErrorsException(SR.GetString(SR.Unable_To_Locate_Type,
																	  _codeDomProviderTypeName), configFileName, configFileLineNumber);
								}
							}
						}
					}
				}

				return type;
			}
		}

		public bool IsCodeDomProviderTypeValid
		{
			get
			{
                //Type type = Type.GetType(_codeDomProviderTypeName);
                var alc = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly());
                // Mapped as CodeDomProviderType maps it: this sets the type it returns (with the .NET
                // Framework provider's DLL in bin, as a deployed site has it, it would be that one).
                type = Type.GetType(MapFrameworkProvider(_codeDomProviderTypeName),
                    assemblyName => {
						try
						{
							return alc.LoadFromAssemblyName(assemblyName);
						} catch (FileNotFoundException)
						{
							return null;
						}
					},
                    (assembly, name, ignoreCase) => assembly != null ? assembly.GetType(name, false, ignoreCase) : Type.GetType(name, false, ignoreCase),
                    false);
				return (type != null);
			}
		}

		public CodeDomProvider CreateProvider()
		{
			// if the provider defines an IDictionary<string, string> ctor and
			// provider options have been provided then call that and give it the 
			// provider options dictionary.  Otherwise call the normal one.

			System.Diagnostics.Debug.Assert(_providerOptions != null, "Created CompilerInfo w/ null _providerOptions");

			if (_providerOptions.Count > 0)
			{
				ConstructorInfo ci = CodeDomProviderType.GetConstructor(new Type[] { typeof(IDictionary<string, string>) });
				if (ci != null)
				{
					return (CodeDomProvider)ci.Invoke(new object[] { _providerOptions });
				}
			}

			return (CodeDomProvider)Activator.CreateInstance(CodeDomProviderType);
		}

		public CodeDomProvider CreateProvider(IDictionary<String, String> providerOptions)
		{
			if (providerOptions == null)
				throw new ArgumentNullException("providerOptions");

			ConstructorInfo constructor = CodeDomProviderType.GetConstructor(new Type[] { typeof(IDictionary<string, string>) });
			if (constructor != null)
			{
				return (CodeDomProvider)constructor.Invoke(new object[] { providerOptions });
			}
			else
				throw new InvalidOperationException(SR.GetString(SR.Provider_does_not_support_options, CodeDomProviderType.ToString()));

		}

		public CompilerParameters CreateDefaultCompilerParameters()
		{
			return CloneCompilerParameters();
		}


		internal CompilerInfo(CompilerParameters compilerParams, String codeDomProviderTypeName, String[] compilerLanguages, String[] compilerExtensions)
		{
			_compilerLanguages = compilerLanguages;
			_compilerExtensions = compilerExtensions;
			_codeDomProviderTypeName = codeDomProviderTypeName;
			if (compilerParams == null)
				compilerParams = new CompilerParameters();

			_compilerParams = compilerParams;
		}

		internal CompilerInfo(CompilerParameters compilerParams, String codeDomProviderTypeName)
		{
			_codeDomProviderTypeName = codeDomProviderTypeName;
			if (compilerParams == null)
				compilerParams = new CompilerParameters();

			_compilerParams = compilerParams;
		}


		public override int GetHashCode()
		{
			return _codeDomProviderTypeName.GetHashCode();
		}

		public override bool Equals(Object o)
		{
			CompilerInfo other = o as CompilerInfo;
			if (o == null)
				return false;

			return CodeDomProviderType == other.CodeDomProviderType &&
				CompilerParams.WarningLevel == other.CompilerParams.WarningLevel &&
				CompilerParams.IncludeDebugInformation == other.CompilerParams.IncludeDebugInformation &&
				CompilerParams.CompilerOptions == other.CompilerParams.CompilerOptions;
		}

		private CompilerParameters CloneCompilerParameters()
		{
			CompilerParameters copy = new CompilerParameters();
			copy.IncludeDebugInformation = _compilerParams.IncludeDebugInformation;
			copy.TreatWarningsAsErrors = _compilerParams.TreatWarningsAsErrors;
			copy.WarningLevel = _compilerParams.WarningLevel;
			copy.CompilerOptions = _compilerParams.CompilerOptions;
			copy.ReferencedAssemblies.AddRange(
				_compilerParams.ReferencedAssemblies
					.OfType<string>()
					.ToArray());
			return copy;
		}

		private String[] CloneCompilerLanguages()
		{
			String[] compilerLanguages = new String[_compilerLanguages.Length];
			Array.Copy(_compilerLanguages, compilerLanguages, _compilerLanguages.Length);
			return compilerLanguages;
		}

		private String[] CloneCompilerExtensions()
		{
			String[] compilerExtensions = new String[_compilerExtensions.Length];
			Array.Copy(_compilerExtensions, compilerExtensions, _compilerExtensions.Length);
			return compilerExtensions;
		}

		internal CompilerParameters CompilerParams
		{
			get
			{
				return _compilerParams;
			}
		}

		// @
		internal IDictionary<string, string> ProviderOptions
		{
			get
			{
				return _providerOptions;
			}
		}
	}
}
