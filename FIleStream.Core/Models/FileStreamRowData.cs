namespace FileStream.Core.Models
{
    /// <summary>
    /// This model doesn't exists in the database
    /// but is used to data fetching from it
    /// </summary>
    public class FileStreamRowData
    {
        /// <summary>
        /// The path of the file to fetch, this path
        /// is used by filestream itself
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 
        /// </summary>The transaction data
        public byte[] Transaction { get; set; }
    }
}
