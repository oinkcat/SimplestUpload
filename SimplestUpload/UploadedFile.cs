using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;

namespace SimplestUpload
{
    /// <summary>
    /// Uploaded file information
    /// </summary>
    [DataContract]
    public class UploadedFile
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [DataMember(Name = "id")]
        public uint Id { get; set; }

        /// <summary>
        /// File name
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// File size in bytes
        /// </summary>
        [DataMember(Name = "size")]
        public long Size { get; set; }

        /// <summary>
        /// Create from FileInfo
        /// </summary>
        /// <param name="fi">FileInfo object</param>
        /// <returns>Uploaded file information</returns>
        public static UploadedFile FromFileInfo(FileInfo fi)
        {
            return new UploadedFile()
            {
                Id = (uint)fi.Name.GetHashCode(),
                Name = fi.Name,
                Size = fi.Length
            };
        }
    }
}