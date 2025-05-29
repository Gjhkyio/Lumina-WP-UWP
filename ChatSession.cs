using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Lumina
{
    [DataContract]
    public class ChatSession
    {
        [DataMember]
        public string UserId { get; set; }

        // The model used during this session
        [DataMember]
        public string ModelUsed { get; set; }

        [DataMember]
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

        // Computed property to help display message count.
        public string MessageCountDisplay => $"Messages: {Messages?.Count ?? 0}";

        /// <summary>
        /// Builds the full prompt to send to the AI.
        /// </summary>
        public string GetFullPrompt(string userMessage)
        {
            // Only add if last message isn’t already from the user.
            if (Messages.Count == 0 || !Messages.Last().Role.Equals("User", StringComparison.OrdinalIgnoreCase))
            {
                Messages.Add(new ChatMessage
                {
                    Role = "User",
                    Content = userMessage,
                    Timestamp = DateTime.Now
                });
            }
            return string.Join("\n", Messages.Select(m => $"{m.Role}: {m.Content}"));
        }

        public void AddMessage(string role, string content)
        {
            Messages.Add(new ChatMessage
            {
                Role = role,
                Content = content,
                Timestamp = DateTime.Now
            });
        }
    }

    [DataContract]
    public class ChatMessage
    {
        [DataMember]
        public string Role { get; set; }

        [DataMember]
        public string Content { get; set; }

        [DataMember]
        public DateTime Timestamp { get; set; }
    }
}
