﻿using Obsidian.Utilities.Mojang;

namespace Obsidian.Net.Actions.PlayerInfo;

public class AddPlayerInfoAction : InfoAction
{
    public string Name { get; set; }

    public List<SkinProperty> Properties { get; set; } = new List<SkinProperty>();

    public int Gamemode { get; set; }

    public int Ping { get; set; }

    public ChatMessage? DisplayName { get; set; } = null;

    public bool HasDisplayName => DisplayName != null;

    public bool HasSigData { get; set; }

    public DateTimeOffset KeyExpireTime { get; set; }

    public byte[] PublicKey { get; set; }

    public byte[] Signature { get; set; }

    public async override Task WriteAsync(MinecraftStream stream)
    {
        await base.WriteAsync(stream);

        await stream.WriteStringAsync(this.Name, 16);

        await stream.WriteVarIntAsync(this.Properties.Count);

        foreach (var props in this.Properties)
            await stream.WriteAsync(await props.ToArrayAsync());


        await stream.WriteVarIntAsync(this.Gamemode);

        await stream.WriteVarIntAsync(this.Ping);

        await stream.WriteBooleanAsync(this.HasDisplayName);

        if (this.HasDisplayName)
            await stream.WriteChatAsync(this.DisplayName);
    }

    public override void Write(MinecraftStream stream)
    {
        base.Write(stream);

        stream.WriteString(Name);
        stream.WriteVarInt(Properties.Count);

        foreach (var properties in Properties)
            stream.Write(properties.ToArray());

        stream.WriteVarInt(Gamemode);
        stream.WriteVarInt(Ping);
        stream.WriteBoolean(HasDisplayName);
        if (HasDisplayName)
            stream.WriteChat(DisplayName);

        stream.WriteBoolean(this.HasSigData);
        if (HasSigData)
        {
            stream.WriteDateTimeOffset(this.KeyExpireTime);

            stream.WriteVarInt(this.PublicKey.Length);
            stream.WriteByteArray(this.PublicKey);

            stream.WriteVarInt(this.Signature.Length);
            stream.WriteByteArray(this.Signature);
        }
    }
}
