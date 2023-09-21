using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using Azure.AI.OpenAI;
using Azure;
using System;
using System.Linq;
using RudeGPT.Shared;

namespace RudeGPT.Api
{
    public class GetCompletion
    {
        private static readonly string AZURE_OPENAI_KEY = System.Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY");
        private static readonly string AZURE_OPENAI_ENDPOINT = System.Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");

        private static readonly string SYSTEM_PROMPT = System.Environment.GetEnvironmentVariable("SYSTEM_PROMPT");

        [Function("GetCompletion")]
        public static async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "completions")] HttpRequestData req, FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("OpenAiFunction");

            try
            {

                OpenAIClient client = new OpenAIClient(new System.Uri(AZURE_OPENAI_ENDPOINT), new Azure.AzureKeyCredential(AZURE_OPENAI_KEY));

                // read the user message off the request body
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var t = JsonSerializer.Deserialize(requestBody, MessageContext.Default.ListMessage);

                var chatMessages = t.Select((message) => message.ToChatMessage()).ToList();

                // add the system prompt to the front of the chat messages list
                chatMessages.Insert(0, new ChatMessage(ChatRole.System, SYSTEM_PROMPT));

                var chatCompletionOptions = new ChatCompletionsOptions(chatMessages);

                Response<ChatCompletions> completionsResponse = await client.GetChatCompletionsAsync("gpt-35-turbo-16k", chatCompletionOptions);

                var responseContent = JsonSerializer.Serialize(completionsResponse.Value.Choices.FirstOrDefault().Message.Content);

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync(responseContent);

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in OpenAiFunction");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}