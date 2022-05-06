using FileStream.Core.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileStream.Core.Services
{
    /// <summary>
    /// A simple abstraction for repository calls
    /// </summary>
    /// <typeparam name="T">The type of the model this repository will be focused on</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Fetch all entities
        /// </summary>
        /// <returns>A list of all entities</returns>
        IList<T> GetAll();
        /// <summary>
        /// Fetch an entity by its id
        /// </summary>
        /// <param name="id">The id of the entity to fetch</param>
        /// <returns>The found entity</returns>
        T GetById(int id);
        /// <summary>
        /// Insert a new entity
        /// </summary>
        /// <param name="entity">The entity to insert</param>
        /// <param name="file">The file related to the entity to fetch</param>
        /// <returns>A task</returns>
        Task<File> Insert(T entity, IFormFile file);
        /// <summary>
        /// Delete an entity by its id
        /// </summary>
        /// <param name="id">The id of the entity to delete</param>
        void Delete(int id);
    }
}
