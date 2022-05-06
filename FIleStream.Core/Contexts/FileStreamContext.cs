using FileStream.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStream.Core.Contexts
{
    /// <summary>
    /// This context exemplified the configuration for a filestream
    /// database environment. This only cover the data connection but
    /// no migration is involved, that's why all models are ignored
    /// during this process.
    /// </summary>
    public class FileStreamContext : DbContext
    {
        /// <summary>
        /// A default constructor
        /// </summary>
        /// <param name="options">Default initialization properties</param>
        public FileStreamContext(DbContextOptions<FileStreamContext> options) : base(options) { }
        /// <summary>
        /// This db set represents the entities where the filestream
        /// data is located
        /// </summary>
        public virtual DbSet<File> Photos { get; set; }
        /// <summary>
        /// This db set doesn't represents any entity in the database
        /// but it's used when retrieving filestream data from 
        /// <see cref="Photo"/> model
        /// </summary>
        public virtual DbSet<FileStreamRowData> RowDatas { get; set; }
        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // for the purpose of this project, migrations are disabled
            modelBuilder.Entity<File>().Metadata.SetIsTableExcludedFromMigrations(true);
            // this is required to let the project know this entity
            // doesn't represents a table
            modelBuilder.Entity<FileStreamRowData>().HasNoKey();
        }
    }
}
