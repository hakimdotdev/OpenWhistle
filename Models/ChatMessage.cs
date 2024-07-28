using System;
using OpenWhistle.Models.Common;

namespace OpenWhistle.Models
{
    public class ChatMessage : BaseEntity
    {
        public WhistleblowerReport Report { get; set; }
        public string Message { get; set; }
        public string Sender { get; set; }

        public ChatMessage() { }

        public ChatMessage(WhistleblowerReport report, string message, string sender)
        {
            Report = report;
            Message = message;
            Sender = sender;
        }
    }
}