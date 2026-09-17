using Microsoft.AspNetCore.Mvc;
using Portafolio.Models;
using Portafolio.Servicios;
using System.Diagnostics;

namespace Portafolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRepositorioProyectos repositorioProyectos;
        private readonly IServicioEmail servicioEmail;

        public HomeController(
            ILogger<HomeController> logger,
            IRepositorioProyectos repositorioProyectos,
            IServicioEmail servicioEmail)
        {
            _logger = logger;
            this.repositorioProyectos = repositorioProyectos;
            this.servicioEmail = servicioEmail;
        }

        public IActionResult Index()
        {
            var proyectos = repositorioProyectos.ObtenerProyectos().Take(3).ToList();
            var modelo = new HomeIndexViewModel() { Proyectos = proyectos };
            return View(modelo);
        }

        public IActionResult Proyectos()
        {
            var proyectos = repositorioProyectos.ObtenerProyectos();
            return View(proyectos);
        }

        public IActionResult Experiencia()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contacto()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contacto(ContactoViewModel contactoViewModel)
        {
            // Validar el modelo
            if (!ModelState.IsValid)
            {
                return View(contactoViewModel);
            }

            try
            {
                // Intentar enviar el email
                await servicioEmail.Enviar(contactoViewModel);

                _logger.LogInformation($"Email enviado correctamente desde: {contactoViewModel.Email}");

                return RedirectToAction("Gracias");
            }
            catch (Exception ex)
            {
                // Log del error
                _logger.LogError($"Error al enviar email: {ex.Message}");

                // Agregar error al modelo para mostrar al usuario
                ModelState.AddModelError(string.Empty,
                    "Hubo un error al enviar tu mensaje. Por favor, intenta de nuevo más tarde o contáctame directamente por email.");

                return View(contactoViewModel);
            }
        }

        public IActionResult Gracias()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}