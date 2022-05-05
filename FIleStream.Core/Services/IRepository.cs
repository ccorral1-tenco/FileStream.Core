using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FileStream.Core.Services
{
    public interface IRepository<T> where T : class
    {
        //IList<T> GetAll();
        T GetById(int id);
        //void Update(T entity);
        Task Insert(T entity, IFormFile file);
        //void Delete(int id);
    }
}
