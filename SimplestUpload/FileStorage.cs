using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SimplestUpload
{
    /// <summary>
    /// Uploaded files storage
    /// </summary>
    public class FileStorage
    {
        // Path to storage directory
        private string dirPath;

        /// <summary>
        /// Get list of all uploaded files
        /// </summary>
        /// <returns>All uploaded files</returns>
        public IList<UploadedFile> GetFiles()
        {
            return new List<UploadedFile>();
        }

        public FileStorage(string path)
        {
            dirPath = path;
        }
    }
}