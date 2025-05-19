using System;
using System.Threading.Tasks;
using Azure;
using Azure.AI.Language.Conversations;
using Azure.Core;
using Microsoft.Extensions.Configuration;

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
            var input = new
            {
                conversationItem = new
                {
                    text = message,
                    id = "1",
                    participantId = "1"
                },
                parameters = new
                {
                    projectName = _projectName,
                    deploymentName = _deploymentName,
                    verbose = true
                },
                kind = "Conversation"
            };

            var response = await _client.AnalyzeConversationAsync(RequestContent.Create(input));
            var result = response.Content.ToDynamicFromJson();

            return new ConversationPrediction
            {
                Intent = result.result.prediction.topIntent,
                Confidence = result.result.prediction.intents[0].confidence,
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
