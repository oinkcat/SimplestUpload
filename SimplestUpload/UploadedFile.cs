using System;
using System.Collections.Generic;
using System.Linq;

namespace SimplestUpload
{
    /// <summary>
    /// Uploaded file information
    /// </summary>
    public class UploadedFile
    {
        /// <summary>
        /// File name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// File size in bytes
        /// </summary>
        public int Size { get; set; }
    }
}