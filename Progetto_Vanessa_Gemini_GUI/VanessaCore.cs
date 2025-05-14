using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.AIPlatform.V1;


namespace Progetto_Vanessa_Gemini_GUI
{
    public class VanessaCore
    {
        private readonly string _modelPath;
        private readonly PredictionServiceClient _predictionServiceClient;
        List<Messaggio> conversationHystory;
        public List<Messaggio> ConversationHystory { get { return conversationHystory; } }
        public string PromptText { get; set; }
        public string PromptResponse {  get; set; }
        public List<Content> _contents { get; set; }


        public VanessaCore(string modelPath, string location)
        {
            _modelPath = modelPath;

            // Create a prediction service client.
            _predictionServiceClient = new PredictionServiceClientBuilder
            {
                Endpoint = $"{location}-aiplatform.googleapis.com"
            }.Build();

            // Initialize contents to send over in every request.
            _contents = new List<Content>();
            PromptText = "";
            PromptResponse = "";

            conversationHystory = GestioneFile.ReadXMLConversation("CronologiaMessaggiVanessa.xml");

        }
        public async Task<string> SendMessageAsync(string prompt)
        {
            if (prompt!.Length > 0)
            {
                conversationHystory.Add(new Messaggio(prompt, "User"));
                PromptText =prompt;
                PromptResponse = "Elaborando...";
                // Initialize the content with the prompt.
                var content = new Content
                {
                    Role = "USER"
                };
                content.Parts.AddRange(new List<Part>()
            {
                new() {
                    Text = prompt
                }
            });
                _contents.Add(content);


                // Create a request to generate content.
                var generateContentRequest = new GenerateContentRequest
                {
                    Model = _modelPath,
                    GenerationConfig = new GenerationConfig
                    {
                        Temperature = 0.9f,
                        TopP = 1,
                        TopK = 32,
                        CandidateCount = 1,
                        MaxOutputTokens = 1024
                    }
                };
                generateContentRequest.Contents.AddRange(_contents);

                // Make a non-streaming request, get a response.
                GenerateContentResponse response = await _predictionServiceClient.GenerateContentAsync(generateContentRequest);

                // Save the content from the response.
                _contents.Add(response.Candidates[0].Content);

                try
                {
                    if (response.Candidates[0].Content != null)
                    {
                        PromptResponse = response.Candidates[0].Content.Parts[0].Text;
                        if(PromptResponse.Length>=1)
                        conversationHystory.Add(new Messaggio(PromptResponse, "Vanessa"));
                        return response.Candidates[0].Content.Parts[0].Text.TrimEnd('\n');

                    }
                    else
                    {
                        return "\n\n[1]C'è stato un errore nella elaborazione del messaggio... \n\n";
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return "\n\n[2]C'è stato un errore nella elaborazione del messaggio...\n\n";

                }
       
            }
            else return "Prompt assente";
        }
        public void InitializeKnowdelge(List<Messaggio> messaggi)
        {

            string vanessaresponse = "";
            string userrequest = "";
            Content content = new Content();


            foreach (Messaggio messaggio in messaggi)
            {


                if (messaggio.Role.Equals("User"))
                {
                    content = new Content { Role = "USER" };
                    content.Parts.AddRange(new List<Part>() { new() { Text = messaggio.Message } });
                    _contents.Add(content);

                }
                else if (messaggio.Role.Equals("Vanessa"))
                {
                    content = new Content { Role = "model" };
                    content.Parts.AddRange(new List<Part>() { new() { Text = messaggio.Message } });
                    _contents.Add(content);
                }


                // Initialize the content with the prompt.



                // Create a request to generate content.
                var generateContentRequest = new GenerateContentRequest
                {
                    Model = _modelPath,
                    GenerationConfig = new GenerationConfig
                    {
                        Temperature = 0.9f,
                        TopP = 1,
                        TopK = 32,
                        CandidateCount = 1,
                        MaxOutputTokens = 2048
                    }
                };
                generateContentRequest.Contents.AddRange(_contents);
       
            }



        }
        public void UpdateConversation(List<Messaggio> CH)
        {
            conversationHystory = CH;
        }

    }
}
