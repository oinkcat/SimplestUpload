using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Runtime.Serialization.Json;

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
            else if(!isPost && requestUrl.StartsWith("/download/"))
            {
                // Requested file for download
                var fileId = uint.Parse(requestUrl.Split('/')[2]);
                HandleFileDownload(fileId);
            }
            else if(isPost && requestUrl == "/list")
            {
                // List uploaded files
                HandleFileList();
            }
            else if(isPost && requestUrl == "/upload")
            {
                // Upload new file
                HandleFileUpload();
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

        // File list requested
        private void HandleFileList()
        {
            var uploadedFiles = GetStorage().GetFiles();
            var serializer = new DataContractJsonSerializer(uploadedFiles.GetType());
            serializer.WriteObject(Response.OutputStream, uploadedFiles);

            Response.StatusCode = StatusOK;
            Response.ContentType = "applicaion/json";
        }

        // Requested file for download
        private void HandleFileDownload(uint fileId)
        {
            var storage = GetStorage();
            var fileInfo = storage.GetFileById(fileId);

            if (fileInfo != null)
            {
                using (var stream = storage.GetFileStream(fileInfo))
                {
                    Response.StatusCode = 200;
                    Response.ContentType = "application/download";
                    
                    // Escape non-ASCII characters in file name
                    string escapedFileName = Uri.EscapeUriString(fileInfo.Name);
                    string headerValue = String.Format("attachment; filename=\"{0}\"",
                                                       escapedFileName);
                    // Will be shown "Save as.." dialog
                    Response.AddHeader("Content-Disposition", headerValue);

                    var fileBuffer = new byte[stream.Length];
                    stream.Read(fileBuffer, 0, fileBuffer.Length);
                    Response.BinaryWrite(fileBuffer);
                }
            }
            else
            {
                Response.StatusCode = 404;
            }
        }

        // Got uploaded file
        private void HandleFileUpload()
        {
            var uploadedFile = Request.Files[0];

            var storage = GetStorage();
            string fileName = Path.GetFileName(uploadedFile.FileName);
            storage.UploadNewFile(fileName, uploadedFile.InputStream);

            Response.StatusCode = StatusOK;
            Response.ContentType = "text/plain";
            Response.Write("OK");
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

        // Get uploaded files storage instance
        private FileStorage GetStorage()
        {
            return new FileStorage(httpContext.Server.MapPath(UploadDirectory));
        }
    }
}