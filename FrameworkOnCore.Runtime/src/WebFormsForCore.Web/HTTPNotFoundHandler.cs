//------------------------------------------------------------------------------
// <copyright file="HTTPNotFoundHandler.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

/*
 * Synchronous Http request handler interface
 * 
 * Copyright (c) 1998 Microsoft Corporation
 */

namespace System.Web {


    /// <devdoc>
    ///    <para>Provides a synchronous Http request handler interface.</para>
    /// </devdoc>
    internal class HttpNotFoundHandler : IHttpHandler {
        
        internal HttpNotFoundHandler() {
        }


        /// <devdoc>
        ///    <para>Drives web processing execution.</para>
        /// </devdoc>
        public void ProcessRequest(HttpContext context) {
            PerfCounters.IncrementCounter(AppPerfCounter.REQUESTS_NOT_FOUND);

            throw new HttpException(404, 
                                    SR.GetString(SR.Path_not_found, context.Request.Path));
        }


        /// <devdoc>
        ///    <para>Indicates whether an HttpNotFoundHandler instance can be recycled and used 
        ///       for another request.</para>
        /// </devdoc>
        public bool IsReusable {
            get { return true; }
        }
    }


    internal class HttpForbiddenHandler : IHttpHandler {
        
        internal HttpForbiddenHandler() {
        }

#if !NETFRAMEWORK
        static readonly string[] IisForbidden = { ".aspq", ".cshtm", ".cshtml", ".vbhtm", ".vbhtml" };
#endif


        /// <devdoc>
        ///    <para>Drives web processing execution.</para>
        /// </devdoc>
        public void ProcessRequest(HttpContext context) {
#if !NETFRAMEWORK
            // FrameworkOnCore: the root web.config's system.web/httpHandlers stand for IIS's integrated handlers, which forbid
            // five types only (applicationHost.config: .aspq, .cshtm, .cshtml, .vbhtm, .vbhtml); the root web.config's other
            // forbidden types (.config, .cs, .mdf...) IIS leaves to its request filtering, and a type a web.config allows
            // there is its static file handler's (IIS 10: .config allowed is 404.3, no MIME type). Not a handler of
            // system.webServer/handlers' (an application's forbidden type: 403).
            if (System.Web.IntegratedModeConfiguration.FindHandlerType(context.Request.HttpMethod, context.Request.FilePathObject) == null &&
                !IisForbidden.Contains(System.IO.Path.GetExtension(context.Request.FilePath), StringComparer.OrdinalIgnoreCase)) {
                StaticFileHandler.ProcessRequestInternal(context, null);
                return;
            }
#endif
            PerfCounters.IncrementCounter(AppPerfCounter.REQUESTS_NOT_FOUND);

            throw new HttpException(403, 
                                    SR.GetString(SR.Path_forbidden, context.Request.Path));
        }


        /// <devdoc>
        ///    <para>Indicates whether an HttpForbiddenHandler instance can be recycled and used 
        ///       for another request.</para>
        /// </devdoc>
        public bool IsReusable {
            get { return true; }
        }
    }


    /// <devdoc>
    ///    <para>Provides a synchronous Http request handler interface.</para>
    /// </devdoc>
    internal class HttpMethodNotAllowedHandler : IHttpHandler {
        
        internal HttpMethodNotAllowedHandler() {
        }


        /// <devdoc>
        ///    <para> Drives 
        ///       web processing execution.</para>
        /// </devdoc>
        public void ProcessRequest(HttpContext context) {
#if !NETFRAMEWORK
            // FrameworkOnCore: the root web.config's catch-all of the verbs other than GET, HEAD and POST, where IIS's
            // StaticFile handler (verb "*") is: its static file module's answer (IisStaticFile.VerbNotAllowed).
            System.Web.IisStaticFile.VerbNotAllowed(context);
            return;
#endif
            throw new HttpException(405,
                                    SR.GetString(SR.Path_forbidden, context.Request.HttpMethod));
        }


        /// <devdoc>
        ///    <para>Indicates whether an HttpForbiddenHandler instance can be recycled and used 
        ///       for another request.</para>
        /// </devdoc>
        public bool IsReusable {
            get { return true; }
        }
    }


    /// <devdoc>
    ///    <para>Provides a synchronous Http request handler interface.</para>
    /// </devdoc>
    internal class HttpNotImplementedHandler : IHttpHandler {
        
        internal HttpNotImplementedHandler() {
        }


        /// <devdoc>
        ///    <para>Drives web processing execution.</para>
        /// </devdoc>
        public void ProcessRequest(HttpContext context) {
            throw new HttpException(501, 
                                    SR.GetString(SR.Method_for_path_not_implemented, context.Request.HttpMethod, context.Request.Path));
        }


        /// <devdoc>
        ///    <para>Indicates whether an HttpNotImplementedHandler instance can be recycled and 
        ///       used for another request.</para>
        /// </devdoc>
        public bool IsReusable {
            get { return true; }
        }
    }
}

