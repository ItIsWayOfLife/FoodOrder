using Core.Constants;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http.Headers;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UploadController : ControllerBaseGetUserId
    {
        private readonly ILoggerService _loggerService;

        private const string CONTROLLER_NAME = "api/upload";

        public UploadController(ILoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        [HttpPost, DisableRequestSizeLimit]
        public IActionResult Upload()
        {
            var file = Request.Form.Files[0];
            var folderName = Path.Combine("wwwroot", "files", "images");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            if (file.Length > 0)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var fullPath = Path.Combine(pathToSave, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                _loggerService.LogInformation(CONTROLLER_NAME, LoggerConstants.TYPE_POST, $"upload file: {fileName} successful", GetCurrentUserId());

                return Ok(new { fileName });
            }
            else
            {
                _loggerService.LogInformation(CONTROLLER_NAME, LoggerConstants.TYPE_POST, $"upload file error", GetCurrentUserId());

                return BadRequest("Invalid file");
            }
        }
    }
}
