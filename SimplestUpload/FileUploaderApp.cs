using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace SimplestUpload
{
    /// <summary>
    /// Simple file uploader request handler
    /// </summary>
    public class FileUploaderApp : IHttpHandler
    {
        private const int StatusOK = 200;
        private const int StatusBadRequest = 400;
        private const int StatusNotFound = 404;

        private const string IndexFileName = "index.html";

        private HttpContext httpContext;

        // Current request
        private HttpRequest Request
        {
            get { return httpContext.Request; }
        }

        // Current response
        private HttpResponse Response
        {
            get { return httpContext.Response; }
        }

        /// <summary>
        /// Can handler be reused
        /// </summary>
        public bool IsReusable
        {
            get { return true; }
        }

        /// <summary>
        /// Handle incoming request
        /// </summary>
        /// <param name="context">HTTP request details</param>
        public void ProcessRequest(HttpContext context)
        {
            httpContext = context;

            string requestUrl = context.Request.Url.LocalPath;

            if (requestUrl == "/")
            {
                HandleIndexPage();
            }
            else
            {
                Response.StatusCode = StatusBadRequest;
                Response.Write("Bad request!");
            }

            Response.End();
        }

        // Index page requested
        private void HandleIndexPage()
        {
            HandleStaticFile(IndexFileName);
        }

        // Static file requested
        private void HandleStaticFile(string fileName)
        {
            string localPath = httpContext.Server.MapPath(fileName);

            if(File.Exists(localPath))
            {
                Response.StatusCode = StatusOK;
                Response.WriteFile(localPath);
            }
            else
            {
                Response.StatusCode = StatusNotFound;
                Response.Write(String.Format("File {0} not found!", fileName));
            }
        }
    }
}