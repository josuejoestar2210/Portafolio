using Portafolio.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Portafolio.Servicios
{
    public class ServicioEmailResend : IServicioEmail
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ServicioEmailResend> _logger;
        private readonly HttpClient _httpClient;

        public ServicioEmailResend(IConfiguration configuration, ILogger<ServicioEmailResend> logger, HttpClient httpClient)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task Enviar(ContactoViewModel contacto)
        {
            try
            {
                var apiKey = _configuration.GetValue<string>("ResendSettings:ApiKey");
                var senderEmail = _configuration.GetValue<string>("ResendSettings:SenderEmail");

                var remitente = "onboarding@resend.dev";

                var cuerpoHtml = $@"
                    <html>
                    <body style='font-family: Arial, sans-serief;'>
                       <div style='background-color: #f4f4f4; padding: 20px;'>
                            <div style='background-color: white; padding: 30px; border-radius: 10px; max-width: 600px; margin: 0 auto;'>

                                <h2 style='color: #0d6efd; border-bottom: 3px solid #0d6efd; padding-bottom: 10px;'>
                                    Nuevo mensaje de Contacto
                                </h2>

                                <div style='margin: 20px 0;'>
                                    <p style='margin: 10px 0;'><strong> Nombre:</strong>   {contacto.Nombre}</p>
                                    <p style='margin: 10px 0;'><strong> Email:</strong> {contacto.Email}</p>
                                </div>

                                <div style='background-color: #f8f9fa; padding: 20px; border-left: 4px solid #0d6efd; margin: 20px 0;'>

                                    <p style='margin: 0;'><strong> Mensaje:</strong></p>
                                    <p style='margin: 10px 0; line-height: 1.6;'>{contacto.Mensaje}</p>
                                </div>

                                <hr  style='border: 1px solid #e9ecef; margin: 20px 0;' />

                                <p style='color: #6c757d; font-size: 12px; text-align: center; margin: 20px 0;'>
                                    Este mensaje fue enviado desde tu portafolio web.
                                </p>

                                <div style='text-align: center; margin-top: 20px;'>
                                    <a href='mailto:{contacto.Email}'
                                        style='background-color: #0d6efd; color: white; padding: 12px 30px;
                                                text-decoration: none; border-radius: 5px; display: inline-block;'>
                                        Responder Email
                                    </a>
                                </div>
                            </div>
                        </div>
                    </body>
                    </html>";

                var payload = new
                {
                    from = $"Portafolio <{remitente}>",
                    to = new[] { senderEmail },
                    reply_to = contacto.Email,
                    subject = $"Nuevo contacto desde el  portafolio: {contacto.Nombre}",
                    html = cuerpoHtml
                };

                var json = JsonSerializer.Serialize(payload);
                var contenido = new StringContent(json, Encoding.UTF8, "application/json");

                using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");
                request.Headers.Add("Authorization", $"Bearer {apiKey}");
                request.Content = contenido;

                var respuesta = await _httpClient.SendAsync(request);

                if (!respuesta.IsSuccessStatusCode)
                {
                    var errorBody = await respuesta.Content.ReadAsStringAsync();
                    _logger.LogError($"Error al enviar email con Resend:  {respuesta.StatusCode} - {errorBody}");
                    throw new Exception($"Resend devolvió un error: {respuesta.StatusCode}");
                }

                _logger.LogInformation($"Email enviado correctamente desde el formulario de contacto de {contacto.Email}");
            }


            catch(Exception ex)
            {
                _logger.LogError($"Error al enviar email: {ex.Message}");
                throw;
            }
        }
    }
}
