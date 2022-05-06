using FileStream.Core.Contexts;
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
    /// <summary>
    /// Represents a repository service for the model <see cref="Photo"/>
    /// </summary>
    public class FileService : IRepository<Models.File>
    {
        /// <summary>
        /// The query string used to retrieve data location of the file
        /// managed by the filestream functionality of sql server
        /// </summary>
        private const string RowDataStatement = @"SELECT Data.PathName() AS 'Path', GET_FILESTREAM_TRANSACTION_CONTEXT() AS 'Transaction' FROM {0} WHERE Id = @id";
        /// <summary>
        /// A simple service provider
        /// </summary>
        private readonly IServiceScopeFactory _scopeFactory;
        /// <summary>
        /// The table name
        /// </summary>
        private readonly string _fileTableName = "dbo.[File]";
        /// <summary>
        /// Represents a repository service for the model <see cref="Photo"/>
        /// </summary>
        /// <param name="scopeFactory">The service provider</param>
        public FileService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        /// <summary>
        /// Fetch all photo models found, this call
        /// purposely ignores the file data, since this
        /// could result in an slow fetch
        /// </summary>
        /// <returns>A list of the photo entities found</returns>
        public IList<Models.File> GetAll()
        {
            using (var scope = _scopeFactory.CreateScope())
            using (var context = scope.ServiceProvider.GetRequiredService<FileStreamContext>())
            {

                return context.Photos
                    .Select(p => new Models.File() 
                    { 
                        Title = p.Title,
                        Description = p.Description,
                        MimeType = p.MimeType
                    })
                    .ToList();
            }
        }
        /// <summary>
        /// Fetch an entity by its id, the filestream data is
        /// separately fetched using a memory stream
        /// </summary>
        /// <param name="id">The id of the entity to fetch</param>
        /// <returns>The found entity</returns>
        public Models.File GetById(int id)
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
                    var selectStatement = string.Format(RowDataStatement, _fileTableName);

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
        /// <summary>
        /// Insert a new entity
        /// </summary>
        /// <param name="entity">The entity to insert</param>
        /// <param name="file">The file related to that entity to insert</param>
        /// <returns>The task</returns>
        public async Task Insert(Models.File entity, IFormFile file)
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
        /// <inheritdoc/>
        public void Delete(int id)
        {
            using (var scope = _scopeFactory.CreateScope())
            using (var context = scope.ServiceProvider.GetRequiredService<FileStreamContext>())
            {
                context.Entry(new Models.File { Id = id }).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }
        /// <summary>
        /// Save the file related to the photo model data
        /// </summary>
        /// <param name="context">The context to use</param>
        /// <param name="id">The id of the entity related to the file to be saved</param>
        /// <param name="file">The file itself</param>
        /// <returns>The task</returns>
        private async Task SavePhotoData(FileStreamContext context, int id, IFormFile file)
        {
            var selectStatement = string.Format(RowDataStatement, _fileTableName);

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
