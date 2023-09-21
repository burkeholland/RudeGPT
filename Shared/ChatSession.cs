using System.Collections.Generic;

namespace RudeGPT.Shared
{
    public class ChatSession
    {
        public string Title { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>();
    }
}