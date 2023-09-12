using System.Collections.Generic;
using System.Text.Json.Serialization;
using Azure.AI.OpenAI;

namespace RudeGPT.Shared
{
    public class Message
    {
        [JsonPropertyName("role")]
        public string RoleLabel
        {
            get => Role.ToString();
            set
            {
                Role = new ChatRole(value);

            }
        }
        [JsonIgnore]
        public ChatRole Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        public ChatMessage ToChatMessage()
        {
            return new ChatMessage(Role, Content);

        }
    }

    [JsonSerializable(typeof(List<Message>))]
    public partial class MessageContext : JsonSerializerContext
    {

    }
}