using FileStream.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStream.Core.Contexts
{
    public class FileStreamContext : DbContext
    {
        public FileStreamContext(DbContextOptions<FileStreamContext> options) : base(options) { }

        //private void SetConfigurationOptions()
        //{
        //    Configuration.LazyLoadingEnabled = false;
        //    Configuration.ProxyCreationEnabled = false;
        //}

        public virtual DbSet<File> Photos { get; set; }
        public virtual DbSet<FileStreamRowData> RowDatas { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<File>().Metadata.SetIsTableExcludedFromMigrations(true);
            modelBuilder.Entity<FileStreamRowData>().HasNoKey();
        }
    }
}
