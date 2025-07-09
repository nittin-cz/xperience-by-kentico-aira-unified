﻿namespace Kentico.Xperience.AiraUnified.Models;

public class ChatMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Content { get; set; }
    public string? Author { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public ChatMessageType Type { get; set; }
}

public enum ChatMessageType
{
    User,
    AI,
    System
}
