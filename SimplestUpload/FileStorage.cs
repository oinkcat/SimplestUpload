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
            var allFiles = new DirectoryInfo(dirPath).EnumerateFiles()
                .Select(fi => UploadedFile.FromFileInfo(fi))
                .OrderBy(f => f.Name)
                .ToList();

            return allFiles;
        }

        /// <summary>
        /// Get uploaded file by it's id
        /// </summary>
        /// <param name="fileId">File id</param>
        public UploadedFile GetFileById(uint fileId)
        {
            return GetFiles().SingleOrDefault(f => f.Id == fileId);
        }

        /// <summary>
        /// Get file contents
        /// </summary>
        /// <param name="fileInfo">Uploaded file info</param>
        /// <returns>File contents stream</returns>
        public Stream GetFileStream(UploadedFile fileInfo)
        {
            string fullName = Path.Combine(dirPath, fileInfo.Name);
            return File.OpenRead(fullName);
        }

        /// <summary>
        /// Save uploaded file into upload directory
        /// </summary>
        /// <param name="fileName">Uploaded file name</param>
        /// <param name="contents">File contents stream</param>
        public void UploadNewFile(string fileName, Stream contents)
        {
            string localFileName = Path.Combine(dirPath, fileName);

            using(var outputStream = new FileStream(localFileName, FileMode.Create))
            {
                var uploadedFileBytes = new byte[contents.Length];
                contents.Read(uploadedFileBytes, 0, uploadedFileBytes.Length);

                outputStream.Write(uploadedFileBytes, 0, uploadedFileBytes.Length);
            }
        }

        public FileStorage(string path)
        {
            dirPath = path;
        }
    }
}