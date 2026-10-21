// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace System.Web.WebPages.Deployment
{
    internal static class AppDomainHelper
    {
        // FrameworkOnCore: .NET has no AppDomain to load bin's assemblies into (AppDomain.CreateDomain throws); their
        // references are read from their metadata, as written (without the redirects the separate domain applied).
        public static IDictionary<string, IEnumerable<string>> GetBinAssemblyReferences(string appPath, string configPath)
        {
            string binDirectory = Path.Combine(appPath, "bin");
            if (!Directory.Exists(binDirectory))
            {
                return null;
            }

            return Directory.EnumerateFiles(binDirectory, "*.dll")
                .ToDictionary(assemblyPath => assemblyPath, assemblyPath => GetReferences(assemblyPath));
        }

        private static IEnumerable<string> GetReferences(string assemblyPath)
        {
            using (var stream = File.OpenRead(assemblyPath))
            using (var pe = new PEReader(stream))
            {
                if (!pe.HasMetadata)
                {
                    return Enumerable.Empty<string>();
                }
                var reader = pe.GetMetadataReader();
                if (!reader.IsAssembly)
                {
                    return Enumerable.Empty<string>();
                }
                return reader.AssemblyReferences
                    .Select(handle => reader.GetAssemblyReference(handle).GetAssemblyName().FullName)
                    .Concat(new[] { reader.GetAssemblyDefinition().GetAssemblyName().FullName })
                    .ToArray();
            }
        }
    }
}
