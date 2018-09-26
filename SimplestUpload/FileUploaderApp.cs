﻿using System;
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
        // Status codes
        private const int StatusOK = 200;
        private const int StatusBadRequest = 400;
        private const int StatusNotFound = 404;

        // Main page file name
        private const string IndexFileName = "index.html";

        // Directory containing static files
        private const string StaticFilesDirectory = "~/www";

        // Directory containing uploaded files
        private const string UploadDirectory = "~/Uploads";

        // Context of current request
        private HttpContext httpContext;

        // File storage
        private FileStorage storage;

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

            string requestUrl = Request.Url.LocalPath;
            bool isPost = Request.HttpMethod == "POST";

            if (!isPost && requestUrl == "/")
            {
                // Home page
                HandleIndexPage();
            }
            else if(!isPost && requestUrl.IndexOf('.') > 0)
            {
                // Static file
                HandleStaticFile(requestUrl);
            }
            else
            {
                // Error 400
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
            // Translate path
            string vPath = String.Concat(StaticFilesDirectory, "/", fileName);
            string localPath = httpContext.Server.MapPath(vPath);

            if(File.Exists(localPath))
            {
                Response.ContentType = MimeMapping.GetMimeMapping(localPath);
                Response.StatusCode = StatusOK;
                Response.WriteFile(localPath);
            }
            else
            {
                // Error 404
                Response.StatusCode = StatusNotFound;
                Response.Write(String.Format("File {0} not found!", fileName));
            }
        }

        public FileUploaderApp()
        {
            storage = new FileStorage(UploadDirectory);
        }
    }
}