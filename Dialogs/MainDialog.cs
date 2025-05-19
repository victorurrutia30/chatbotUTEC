using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using ChatbotUTEC.Services;

namespace ChatbotUTEC.Dialogs
{
    public class MainDialog : ActivityHandler
    {
        private readonly CLUPredictor _clu;
        private readonly DatabaseHelper _db;

        public MainDialog(CLUPredictor clu, DatabaseHelper db)
        {
            _clu = clu;
            _db = db;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var userMessage = turnContext.Activity.Text;
            var prediction = await _clu.GetPredictionAsync(userMessage);
            var intent = prediction.Intent;
            var response = "";

            switch (intent)
            {
                case "ConsultarHorario":
                    string carnet = ExtractEntity(prediction.Entities, "Carnet");
                    var horarios = _db.GetHorariosPorCarnet(carnet);
                    response = horarios.Count > 0 ? string.Join("\n", horarios) : "No se encontraron horarios.";
                    break;

                case "ConsultarTramite":
                    string carnetTramite = ExtractEntity(prediction.Entities, "Carnet");
                    var tramites = _db.GetTramites(carnetTramite);
                    response = tramites.Count > 0 ? string.Join("\n", tramites) : "No se encontraron trámites.";
                    break;

                case "ConsultarParcial":
                    string materia = ExtractEntity(prediction.Entities, "NombreMateria");
                    var parciales = _db.GetHorarioParcial(materia);
                    response = parciales.Count > 0 ? string.Join("\n", parciales) : "No se encontraron parciales.";
                    break;

                case "ConsultarDocente":
                    string facultad = ExtractEntity(prediction.Entities, "NombreFacultad");
                    var docentes = _db.GetDocentesPorFacultad(facultad);
                    response = docentes.Count > 0 ? string.Join("\n", docentes) : "No se encontraron docentes.";
                    break;

                default:
                    response = "Lo siento, no entendí tu solicitud. ¿Puedes reformularla?";
                    break;
            }

            await turnContext.SendActivityAsync(MessageFactory.Text(response), cancellationToken);
        }

        private string ExtractEntity(dynamic entities, string entityName)
        {
            try
            {
                var value = entities[entityName][0]["text"].ToString();
                return value;
            }
            catch
            {
                return "";
            }
        }
    }
}
