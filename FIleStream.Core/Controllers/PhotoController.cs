using FileStream.Core.Models;
using FileStream.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FileStream.Core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PhotoController : Controller
    {
        private readonly IRepository<Photo> _photoService;

        public PhotoController(IRepository<Photo> photoService)
        {
            _photoService = photoService;
        }
        [HttpGet]
        public IActionResult GetPhoto(int id)
        {
            var result = _photoService.GetById(id);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> UploadPhoto([FromForm] IFormFile file)
        {
            var guid = Guid.NewGuid();
            var photo = new Photo()
            {
                Description = $"Hola ${guid}",
                Title = $"Titulo ${DateTime.Now}",
                FileId = guid,
            };
            await _photoService.Insert(photo, file);
            return Ok();
        }
    }
}
