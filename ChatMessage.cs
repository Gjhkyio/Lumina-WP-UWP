using System;
using System.Runtime.Serialization;

[DataContract]
public class ChatMessage
{
    [DataMember]
    public string Content { get; set; }

    [DataMember]
    public bool IsUser { get; set; }

    [DataMember]
    public DateTime Timestamp { get; set; }
}
