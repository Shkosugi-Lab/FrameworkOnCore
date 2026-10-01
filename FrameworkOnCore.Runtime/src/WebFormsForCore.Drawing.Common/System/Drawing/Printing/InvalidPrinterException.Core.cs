// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

/*
 * This file is not intended to be used by Mono.
 * Instead InvalidPrinterException.Serializable.cs should be used.
 */

namespace System.Drawing.Printing
{
    [System.Runtime.CompilerServices.TypeForwardedFrom("System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    public partial class InvalidPrinterException
    {
        protected InvalidPrinterException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // Ignoring not deserializable input
        }

#if WebFormsForCore
        // As .NET 8's: the legacy serialization it overrides is obsolete (SYSLIB0051).
        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("settings", null);
        }
    }
}
