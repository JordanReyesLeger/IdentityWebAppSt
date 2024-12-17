using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace ApiBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class UploadController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ILogger<UploadController> _logger;

        public UploadController(IFileService fileService, ILogger<UploadController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] int year, [FromForm] long consecutivo)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("El archivo está vacío o no se envió.");
                return BadRequest("El archivo no puede estar vacío.");
            }

            try
            {
                var fileName = file.FileName;

                _logger.LogInformation("Iniciando la subida del archivo {FileName} con descripción: {Description}", fileName, "");

                using (var fileStream = file.OpenReadStream())
                {
                    var (fileUrl, uploadedFileName, fileDescription) = await _fileService.UploadFileAsync(fileStream, fileName, file.ContentType, "", year, consecutivo);

                    _logger.LogInformation("Archivo subido exitosamente: {FileUrl}", fileUrl);

                    return Ok(new
                    {
                        Message = "Archivo subido exitosamente.",
                        FileUrl = fileUrl,
                        FileName = uploadedFileName,
                        Description = fileDescription
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir el archivo: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error inesperado. Inténtalo de nuevo.");
            }
        }
    }
}