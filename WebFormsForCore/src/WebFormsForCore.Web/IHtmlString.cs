//------------------------------------------------------------------------------
// <copyright file="IHtmlString.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

#if NETFRAMEWORK
using System.Diagnostics.CodeAnalysis;

namespace System.Web {
    // Marker interface implemented by objects that should NOT be HTML encoded when <%: o %> is used
    public interface IHtmlString {
        string ToHtmlString();
    }
}
#endif
#if !NETFRAMEWORK
// On .NET the interface is in System.Web.HttpUtility. .NET Framework libraries built against System.Web
// (System.Web.WebPages, MVC) look for it here.
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Web.IHtmlString))]
#endif
