using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ChatbotUTEC.Services;

namespace ChatbotUTEC.Dialogs
{
    public class MainDialog : ActivityHandler
    {
        private readonly CLUPredictor _clu;
        private readonly DatabaseHelper _db;
        private readonly ILogger<MainDialog> _logger;

        public MainDialog(CLUPredictor clu, DatabaseHelper db, ILogger<MainDialog> logger)
        {
            _clu = clu;
            _db = db;
            _logger = logger;
        }

        protected override async Task OnMessageActivityAsync(
            ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken)
        {
            var tStart = DateTime.UtcNow;
            var userMessage = turnContext.Activity.Text;
            ConversationPrediction prediction = null;
            string response;

            try
            {
                prediction = await _clu.GetPredictionAsync(userMessage);
                _logger.LogInformation(
                    "🧠 Intent detectado: {Intent}, Confianza: {Confidence}",
                    prediction.Intent,
                    prediction.Confidence
                );

                switch (prediction.Intent)
                {
                    case "Saludo":
                        response = "¡Hola! 👋 Soy el bot de UTEC. ¿En qué puedo ayudarte hoy? Puedes pedirme tu horario, trámites, parciales o docentes.";
                        break;

                    case "ConsultarHorario":
                        var carnet = ExtractEntity(prediction.Entities, "Carnet");
                        var horarios = _db.GetHorariosPorCarnet(carnet);
                        response = horarios.Any()
                            ? string.Join("\n", horarios)
                            : "No se encontraron horarios.";
                        break;

                    case "ConsultarTramite":
                        var carnetTramite = ExtractEntity(prediction.Entities, "Carnet");
                        var tramites = _db.GetTramites(carnetTramite);
                        response = tramites.Any()
                            ? string.Join("\n", tramites)
                            : "No se encontraron trámites.";
                        break;

                    case "ConsultarParcial":
                        var materia = ExtractEntity(prediction.Entities, "NombreMateria");
                        var parciales = _db.GetHorarioParcial(materia);
                        response = parciales.Any()
                            ? string.Join("\n", parciales)
                            : "No se encontraron parciales.";
                        break;

                    case "ConsultarDocente":
                        var facultad = ExtractEntity(prediction.Entities, "NombreFacultad");
                        var docentes = _db.GetDocentesPorFacultad(facultad);
                        response = docentes.Any()
                            ? string.Join("\n", docentes)
                            : "No se encontraron docentes.";
                        break;

                    default:
                        response = "Lo siento, no entendí tu solicitud. ¿Puedes reformularla?";
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en CLU o base de datos");
                response = "Ocurrió un error al procesar tu solicitud.";
            }

            // Envío de la respuesta al usuario
            await turnContext.SendActivityAsync(MessageFactory.Text(response), cancellationToken);

            // Registro de la interacción en la base de datos
            var responseTimeMs = (int)(DateTime.UtcNow - tStart).TotalMilliseconds;
            try
            {
                _db.InsertarInteraccion(
                    turnContext.Activity.From.Id,
                    userMessage,
                    prediction?.Intent ?? "None",
                    prediction?.Entities?.ToString() ?? "[]",
                    responseTimeMs
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al registrar la interacción en la base de datos");
            }
        }

        private string ExtractEntity(JArray entities, string entityName)
        {
            if (entities == null) return "";

            foreach (var entity in entities.Children<JObject>())
            {
                if (entity["category"]?.Value<string>() == entityName)
                {
                    return entity["text"]?.Value<string>() ?? "";
                }
            }

            return "";
        }
    }
}
