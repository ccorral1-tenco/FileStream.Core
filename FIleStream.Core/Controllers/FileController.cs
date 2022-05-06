using FileStream.Core.Models;
using FileStream.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FileStream.Core.Controllers
{
    /// <summary>
    /// This controller exemplified the use of filestreams.
    /// 
    /// The [ApiController] attribute is required to let swagger
    /// recognizes the api actions
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class FileController : Controller
    {
        /// <summary>
        /// This service handles the filestream and api db connections
        /// </summary>
        private readonly IRepository<File> _fileService;
        /// <summary>
        /// This controller exemplified the use of filestreams.
        /// </summary>
        /// <param name="photoService">This service handles the filestream and api db connections</param>
        public FileController(IRepository<File> fileService)
        {
            _fileService = fileService;
        }
        [HttpGet]
        /// <summary>
        /// A simple call to download a photo
        /// </summary>
        /// <returns>A file</returns>
        public IActionResult GetFile(int id)
        {
            var result = _fileService.GetById(id);
            return File(result.Data, Application.Octet, result.Title);
        }
        /// <summary>
        /// A simple call to retrieve all photo data
        /// </summary>
        /// <returns>A json with all photo data except the file itself</returns>
        [HttpGet("All")]
        public IActionResult GetAllFiles()
        {
            return Ok(_fileService.GetAll());
        }
        /// <summary>
        /// Use this call to upload a file
        /// </summary>
        /// <param name="file">The file to upload</param>
        /// <returns>A okay response when everything finish</returns>
        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            var guid = Guid.NewGuid();
            var fileModel = new File()
            {
                Description = file.Name,
                Title = file.FileName,
                FileId = guid,
                Data = new byte[0x00],
                MimeType = file.ContentType
            };
            var newFile = await _fileService.Insert(fileModel, file);
            return Json(new { newFile.Id, newFile.Title, newFile.Description, newFile.MimeType });
        }
        /// <summary>
        /// Use this call to delete a given file by its id
        /// </summary>
        /// <param name="id">The id of the file to delete</param>
        /// <returns>A okay response when everything finish</returns>
        [HttpDelete]
        public IActionResult DeleteFile(int id)
        {
            _fileService.Delete(id);
            return Ok("El archivo fue borrado con exito!");
        }
    }
}
