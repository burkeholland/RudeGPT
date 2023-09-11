using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.IO;
using System.Net.Http.Json;
using Azure.AI.OpenAI;
using Azure;
using System;

namespace RudeGPT.Api
{
    public class GetCompletion
    {
        private static readonly string AZURE_OPENAI_KEY = System.Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY");
        private static readonly string AZURE_OPENAI_ENDPOINT = System.Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");

        [Function("GetCompletion")]
        public static async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "completions")] HttpRequestData req, FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("OpenAiFunction");

            try
            {

                OpenAIClient client = new OpenAIClient(new System.Uri(AZURE_OPENAI_ENDPOINT), new Azure.AzureKeyCredential(AZURE_OPENAI_KEY));

                // read the user message off the request body
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var messages = JsonSerializer.Deserialize<IList<ChatMessage>>(requestBody);

                var chatCompletionOptions = new ChatCompletionsOptions(messages);

                Response<ChatCompletions> completionsResponse = await client.GetChatCompletionsAsync("gpt-35-turbo-16k", chatCompletionOptions);

                foreach (ChatChoice choice in completionsResponse.Value.Choices)
                {
                    logger.LogInformation($"Completion: {choice.Message.Content}");
                }

                var response = req.CreateResponse(HttpStatusCode.OK);
                var responseContent = JsonSerializer.Serialize(completionsResponse.Value.Choices);
                await response.WriteStringAsync(responseContent);

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in OpenAiFunction");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        // public async Task<HttpResponseMessage> GetResponseAsync(List<Message> messages)
        //         {
        //             var temperature = 0.5;

        //         var client = new HttpClient();

        //         var options = new JsonSerializerOptions
        //         {
        //             WriteIndented = true,
        //             PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        //             Converters = {
        //                     new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        //                 }
        //         };

        //         var httpRequestMessage = new HttpRequestMessage
        //         {
        //             Method = HttpMethod.Post,
        //             RequestUri = new System.Uri(AZURE_OPENAI_ENDPOINT),
        //             Headers = {
        //                     { "api-key", AZURE_OPENAI_KEY }
        //                 },
        //             Content = new StringContent(JsonSerializer.Serialize(new { messages, temperature }, options))
        //         };

        //         var response = await client.SendAsync(httpRequestMessage);

        //             return response;
        //         }
        // }
    }
}