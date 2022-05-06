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
    public class FileController : Controller
    {
        private readonly IRepository<File> _fileService;

        public FileController(IRepository<File> fileService)
        {
            _fileService = fileService;
        }
        [HttpGet]
        public IActionResult GetFile(int id)
        {
            var result = _fileService.GetById(id);
            return File(result.Data, Application.Octet, result.Title);
        }
        [HttpGet("All")]
        public IActionResult GetAllFiles()
        {
            return Ok(_fileService.GetAll());
        }
        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            var guid = Guid.NewGuid();
            var photo = new File()
            {
                Description = file.Name,
                Title = file.FileName,
                FileId = guid,
                Data = new byte[0x00],
                MimeType = file.ContentType
            };
            await _fileService.Insert(photo, file);
            return Ok();
        }
        [HttpDelete]
        public IActionResult DeleteFile(int id)
        {
            _fileService.Delete(id);
            return Ok();
        }
    }
}
