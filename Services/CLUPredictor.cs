// ChatbotUTEC/Services/CLUPredictor.cs
using System;
using System.Threading.Tasks;
using Azure;
using Azure.AI.Language.Conversations;
using Azure.Core;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;   // ← Asegúrate de tenerlo

namespace ChatbotUTEC.Services
{
    public class CLUPredictor
    {
        private readonly ConversationAnalysisClient _client;
        private readonly string _projectName;
        private readonly string _deploymentName;

        public CLUPredictor(IConfiguration configuration)
        {
            var endpoint = new Uri(configuration["LanguageUnderstanding:Endpoint"]);
            var key = new AzureKeyCredential(configuration["LanguageUnderstanding:EndpointKey"]);
            _projectName = configuration["LanguageUnderstanding:ProjectName"];
            _deploymentName = configuration["LanguageUnderstanding:DeploymentName"];

            _client = new ConversationAnalysisClient(endpoint, key);
        }

        public async Task<ConversationPrediction> GetPredictionAsync(string message)
        {
            // 1) Construyo el payload
            var data = new
            {
                analysisInput = new
                {
                    conversationItem = new
                    {
                        participantId = "1",
                        id = "1",
                        modality = "text",
                        language = "es",
                        text = message
                    }
                },
                parameters = new
                {
                    projectName = _projectName,
                    deploymentName = _deploymentName,
                    stringIndexType = "TextElement_V8"
                },
                kind = "Conversation"
            };

            // 2) Serializo y loggeo lo que voy a enviar
            var payloadJson = JsonConvert.SerializeObject(data, Formatting.Indented);
            Console.WriteLine("➡️  [CLU REQUEST] URL: "
                + _client.Endpoint
                + "/language/:analyze-conversations?api-version=2022-10-01-preview");
            Console.WriteLine("➡️  [CLU PAYLOAD]\n" + payloadJson);

            // 3) Llamada al servicio
            var response = await _client.AnalyzeConversationAsync(RequestContent.Create(data));

            // 4) Leo y muestro la respuesta cruda
            var rawJson = response.Content.ToString();
            Console.WriteLine("⬅️  [CLU RESPONSE]\n" + rawJson);

            // 5) Deserializo y devuelvo el objeto
            dynamic result = JsonConvert.DeserializeObject(rawJson);
            // DESPUÉS: recorremos el array de intents para extraer la confianza
            string topIntent = result.result.prediction.topIntent;
            double confidence = 0.0;

            foreach (var intentObj in result.result.prediction.intents)
            {
                if ((string)intentObj.category == topIntent)
                {
                    confidence = (double)intentObj.confidence;
                    break;
                }
            }

            return new ConversationPrediction
            {
                Intent = topIntent,
                Confidence = confidence,
                Entities = result.result.prediction.entities
            };
        }
    }

    public class ConversationPrediction
    {
        public string Intent { get; set; }
        public double Confidence { get; set; }
        public dynamic Entities { get; set; }
    }
}
