﻿namespace Obsidian.API.Registry.Codecs.Chat;

public sealed record class ChatCodec
{
    public string Name { get; set; }

    public int Id { get; set; }

    public ChatElement Element { get; set; } = new();
}