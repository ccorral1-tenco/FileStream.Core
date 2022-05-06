using FileStream.Core.Models;
using FileStream.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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
            return File(result.Data, Application.Octet, result.Title);
        }
        [HttpGet("All")]
        public IActionResult GetAllPhotos()
        {
            return Ok(_photoService.GetAll());
        }
        [HttpPost]
        public async Task<IActionResult> UploadPhoto([FromForm] IFormFile file)
        {
            var guid = Guid.NewGuid();
            var photo = new Photo()
            {
                Description = file.Name,
                Title = file.FileName,
                FileId = guid,
                Data = new byte[0x00],
                MimeType = file.ContentType
            };
            await _photoService.Insert(photo, file);
            return Ok();
        }
        [HttpDelete]
        public IActionResult DeletePhoto(int id)
        {
            _photoService.Delete(id);
            return Ok();
        }
    }
}
