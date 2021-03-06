using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileStream.Core.Models
{
        /// <summary>
        /// Defines a photo
        /// </summary>
        [Table("File", Schema = "dbo")]
        public class File
        {
            /// <summary>
            /// Gets or sets the identifier.
            /// </summary>
            /// <value>
            /// The identifier.
            /// </value>
            public int Id { get; set; }
            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            /// <value>
            /// The title.
            /// </value>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            /// <value>
            /// The description.
            /// </value>
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the data (the photo content).
            /// </summary>
            /// <value>
            /// The data.
            /// </value>
            public byte[] Data { get; set; }
            /// <summary>
            /// This is a REQUIRED id for Filestream to work
            /// </summary>
            public Guid FileId { get; set; }
            /// <summary>
            /// The mime type of the file to upload
            /// </summary>
            public string MimeType { get; set; }
    }
}
