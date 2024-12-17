using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;
using WebsiteFront.Models;

namespace WebsiteFront.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, HttpClient httpClient)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, string year, string consecutivo, string description, string customFileName)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("No se seleccionó ningún archivo o está vacío.");
                TempData["Message"] = "Por favor, selecciona un archivo válido.";
                return RedirectToAction("Index");
            }

            try
            {
                // Leer la URL del backend desde la configuración
                var backendUrl = _configuration["BackendApi:UploadUrl"];
                if (string.IsNullOrEmpty(backendUrl))
                {
                    _logger.LogError("La URL de la API no está configurada.");
                    TempData["Message"] = "Ocurrió un error al conectar con el servidor.";
                    return RedirectToAction("Index");
                }

                // Crear el contenido del archivo
                using (var content = new MultipartFormDataContent())
                {
                    // Agregar el archivo al contenido
                    var fileStreamContent = new StreamContent(file.OpenReadStream());
                    fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    content.Add(fileStreamContent, "file", file.FileName);

                    // Agregar parámetros adicionales al contenido
                    content.Add(new StringContent(year.ToString()), "year");
                    content.Add(new StringContent(consecutivo.ToString()), "consecutivo");
                    content.Add(new StringContent(description), "description");

                    // Enviar la solicitud al backend
                    var response = await _httpClient.PostAsync(backendUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Message"] = "Archivo subido exitosamente.";
                        _logger.LogInformation("Archivo {FileName} enviado correctamente al backend.", file.FileName);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        TempData["Message"] = $"Error al subir el archivo: {errorContent}";
                        _logger.LogError("Error al enviar el archivo {FileName} al backend: {Error}", file.FileName, errorContent);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al enviar el archivo al backend.");
                TempData["Message"] = "Ocurrió un error inesperado. Inténtalo de nuevo.";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
