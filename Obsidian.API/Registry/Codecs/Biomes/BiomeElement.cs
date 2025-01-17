﻿namespace Obsidian.API.Registry.Codecs.Biomes;

public sealed record class BiomeElement
{
    public BiomeEffect Effects { get; set; }

    public float Depth { get; set; }

    public float Temperature { get; set; }

    public float Scale { get; set; }

    public float Downfall { get; set; }

    public string Category { get; set; } = string.Empty;

    public string Precipitation { get; set; }

    public string TemperatureModifier { get; set; }
}
