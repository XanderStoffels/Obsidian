﻿using System.Collections.Immutable;
using System.Text.Json;

namespace Obsidian.SourceGenerators.Registry.Models;

internal sealed class Assets
{
    public Block[] Blocks { get; }
    public Tag[] Tags { get; }
    public Item[] Items { get; }

    private Assets(Block[] blocks, Tag[] tags, Item[] items)
    {
        Blocks = blocks;
        Tags = tags;
        Items = items;
    }

    public static Assets Get(ImmutableArray<(string name, string json)> files, SourceProductionContext ctx)
    {
        Block[] blocks = GetBlocks(files.GetJsonFromArray("blocks"));
        Fluid[] fluids = GetFluids(files.GetJsonFromArray("fluids"));
        Tag[] tags = GetTags(files.GetJsonFromArray("tags"), blocks, fluids);
        Item[] items = GetItems(files.GetJsonFromArray("items"));

        return new Assets(blocks, tags, items);
    }

    public static Fluid[] GetFluids(string? json)
    {
        if (json is null)
            return Array.Empty<Fluid>();

        var fluids = new List<Fluid>();
        using var document = JsonDocument.Parse(json);

        var fluidProperties = document.RootElement.EnumerateObject();

        foreach(JsonProperty property in fluidProperties)
        {
            var name = property.Name;

            fluids.Add(new Fluid(name, name.Substring(name.IndexOf(':') + 1), property.Value.GetInt32()));
        }

        return fluids.ToArray();
    }

    public static Block[] GetBlocks(string? json)
    {
        if (json is null)
            return Array.Empty<Block>();

        var blocks = new List<Block>();
        using var document = JsonDocument.Parse(json);

        int id = 0;

        var blockProperties = document.RootElement.EnumerateObject();

        var newBlocks = new Dictionary<int, JsonProperty>();

        foreach (JsonProperty property in blockProperties)
        {
            foreach (var state in property.Value.GetProperty("states").EnumerateArray())
            {
                if (state.TryGetProperty("default", out var element))
                {
                    newBlocks.Add(state.GetProperty("id").GetInt32(), property);
                    break;
                }
            }
        }

        foreach (var property in newBlocks.OrderBy(x => x.Key).Select(x => x.Value))
            blocks.Add(Block.Get(property, id++));

        return blocks.ToArray();
    }

    private static Item[] GetItems(string? json)
    {
        if (json is null)
            return Array.Empty<Item>();

        var items = new List<Item>();
        using var document = JsonDocument.Parse(json);

        foreach (JsonProperty property in document.RootElement.EnumerateObject())
        {
            items.Add(Item.Get(property));
        }

        return items.ToArray();
    }

    public static Tag[] GetTags(string? json, Block[] blocks, Fluid[] fluids)
    {
        if (json is null)
            return Array.Empty<Tag>();

        var taggables = new Dictionary<string, ITaggable>();
        foreach (Block block in blocks)
        {
            if (block.Tag is "minecraft:water" or "minecraft:lava")//Skip fluids
                continue;

            taggables.Add(block.Tag, block);
        }

        foreach(Fluid fluid in fluids)
            taggables.Add(fluid.Tag, fluid);

        var tags = new List<Tag>();
        var knownTags = new Dictionary<string, Tag>();
        var missedTags = new Dictionary<string, List<string>>();

        using var document = JsonDocument.Parse(json);

        foreach (JsonProperty property in document.RootElement.EnumerateObject())
        {
            tags.Add(Tag.Get(property, taggables, knownTags, missedTags));
        }

        VerifyTags(knownTags, missedTags, taggables);
        VerifyTags(knownTags, missedTags, taggables);//I can't think of a better solution :skull:

        return tags.ToArray();
    }

    private static void VerifyTags(Dictionary<string, Tag> knownTags, Dictionary<string, List<string>> missedTags, Dictionary<string, ITaggable> taggables)
    {
        foreach (var missedTag in missedTags)
        {
            var propertyName = missedTag.Key;
            var tagsMissed = missedTag.Value;

            var prop = knownTags[propertyName];
            foreach (var tagMissed in tagsMissed)
            {
                if (knownTags.TryGetValue(tagMissed, out var tag))
                {
                    foreach (var value in tag.Values)
                    {
                        if (prop.Values.Contains(value))
                            continue;

                        prop.Values.Add(value);
                    }
                }
                else if (taggables.TryGetValue(tagMissed, out var taggable) && !prop.Values.Contains(taggable))
                {
                    prop.Values.Add(taggable);
                }
            }
        }
    }
}
