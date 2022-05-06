using FileStream.Core.Contexts;
using FileStream.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileStream.Core.Services
{
    public class PhotoService : IRepository<Photo>
    {
        private const string RowDataStatement = @"SELECT Data.PathName() AS 'Path', GET_FILESTREAM_TRANSACTION_CONTEXT() AS 'Transaction' FROM {0} WHERE Id = @id";

        private readonly IServiceScopeFactory _scopeFactory;

        public PhotoService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public IList<Photo> GetAll()
        {
            using (var scope = _scopeFactory.CreateScope())
            using (var context = scope.ServiceProvider.GetRequiredService<FileStreamContext>())
            {

                return context.Photos
                    .Select(p => new Photo() 
                    { 
                        Title = p.Title,
                        Description = p.Description,
                        MimeType = p.MimeType
                    })
                    .ToList();
            }
        }

        public Photo GetById(int id)
        {
            using (var scope = _scopeFactory.CreateScope())
            using (var context = scope.ServiceProvider.GetRequiredService<FileStreamContext>())
            {
                var photo = context.Photos.FirstOrDefault(p => p.Id == id);
                if (photo == null)
                {
                    return null;
                }

                using var transaction = context.Database.BeginTransaction();
                {
                    var selectStatement = string.Format(RowDataStatement, "dbo.Photo");

                    var rowData = context.RowDatas
                        .FromSqlRaw(selectStatement, new SqlParameter("@id", id))
                        .First();
                            
                    using (var source = new SqlFileStream(rowData.Path, rowData.Transaction, FileAccess.Read))
                    {
                        var buffer = new byte[16 * 1024];
                        using (var ms = new MemoryStream())
                        {
                            int bytesRead;
                            while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                ms.Write(buffer, 0, bytesRead);
                            }
                            photo.Data = ms.ToArray();
                        }
                    }

                    transaction.Commit();
                }

                return photo;
            }
        }


        public async Task Insert(Photo entity, IFormFile file)
        {
            using (var scope = _scopeFactory.CreateScope())
            using (var context = scope.ServiceProvider.GetRequiredService<FileStreamContext>())
            {
               
                using var transaction = context.Database.BeginTransaction();
                {
                    context.Photos.Add(entity);
                    context.SaveChanges();
                    await SavePhotoData(context, entity.Id, file);
                    transaction.Commit();
                }
            }
        }

        public void Delete(int id)
        {
            using (var scope = _scopeFactory.CreateScope())
            using (var context = scope.ServiceProvider.GetRequiredService<FileStreamContext>())
            {
                context.Entry(new Photo { Id = id }).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }

        private async Task SavePhotoData(FileStreamContext context, int id, IFormFile file)
        {
            var selectStatement = string.Format(RowDataStatement, "dbo.Photo");

            var rowData = context.RowDatas
                .FromSqlRaw(selectStatement, new SqlParameter("@id", id))
                .First();

            using (var destination = new SqlFileStream(rowData.Path, rowData.Transaction, FileAccess.Write))
            {
                await file.CopyToAsync(destination);
            }
        }
    }
}
