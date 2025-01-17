﻿using Obsidian.API.Noise;
using SharpNoise.Modules;
using static SharpNoise.Modules.Curve;

namespace Obsidian.WorldData.Generators.Overworld;

    public class OverworldTerrainNoise
{
    public Module Heightmap { get; set; }

    public OverworldTerrainSettings Settings { get; private set; } = new OverworldTerrainSettings();

    public readonly Module heightNoise, squashNoise, humidityNoise, tempNoise, erosionNoise;

    public readonly Module terrainSelector, biomeSelector;

    public readonly Module tunnels;

    private readonly int seed;

    public OverworldTerrainNoise(int seed)
    {
        this.seed = seed + 765; // add offset

        humidityNoise = new Cache()
        {
            Source0 = new Clamp()
            {
                Source0 = new Blur()
                {
                    Source0 = new Perlin()
                    {
                        Frequency = 0.004,
                        Quality = SharpNoise.NoiseQuality.Fast,
                        Seed = seed + 3
                    }

                }
            }
        };

        tempNoise = new Cache()
        {
            Source0 = new Clamp()
            {
                Source0 = new Blur()
                {
                    Source0 = new Perlin()
                    {
                        Frequency = 0.001,
                        Quality = SharpNoise.NoiseQuality.Best,
                        Seed = seed + 2
                    }
                }
            }
        };

        heightNoise = new Cache()
        {
            Source0 = new Clamp()
            {
                Source0 = new Curve
                {
                    ControlPoints = new List<ControlPoint>()
                {
                     new Curve.ControlPoint(-1, -0.75),
                     new Curve.ControlPoint(-0.6, -0.75),
                     new Curve.ControlPoint(-0.525, -0.33),
                     new Curve.ControlPoint(-0.3, -0.33),
                     new Curve.ControlPoint(-0.1, -0.08),
                     new Curve.ControlPoint(0.2, 0.04),
                     new Curve.ControlPoint(0.7, 0.1),
                     new Curve.ControlPoint(1, 0.12)
                },
                    Source0 = new Perlin()
                    {
                        Frequency = 0.002,
                        Quality = SharpNoise.NoiseQuality.Best,
                        Seed = seed + 4
                    }
                }
            }
        };

        erosionNoise = new Cache()
        {
            Source0 = new Clamp()
            {
                Source0 = new Curve
                {
                    ControlPoints = new List<ControlPoint>()
                {
                     new Curve.ControlPoint(-1, 0.5),
                     new Curve.ControlPoint(-0.5, 0),
                     new Curve.ControlPoint(-0.45, 0.2),
                     new Curve.ControlPoint(-0.2, -0.8),
                     new Curve.ControlPoint(2.5, -0.85),
                     new Curve.ControlPoint(0.5, -0.85),
                     new Curve.ControlPoint(0.55, -0.5),
                     new Curve.ControlPoint(0.7, -0.5),
                     new Curve.ControlPoint(0.75, -0.85),
                     new Curve.ControlPoint(1, -1)
                },
                    Source0 = new Perlin()
                    {
                        Frequency = 0.002,
                        Quality = SharpNoise.NoiseQuality.Best,
                        Seed = seed + 5
                    }
                }
            }
        };

        squashNoise = new Cache
        {
            Source0 = new Perlin()
            {
                Frequency = 0.001,
                Quality = SharpNoise.NoiseQuality.Best,
                Seed = seed + 5
            }
        };

        terrainSelector = new OverworldTerrain(heightNoise, squashNoise, erosionNoise)
        {
            Seed = seed,
            TerrainStretch = 15
        };

        biomeSelector = new BiomeSelector(tempNoise, humidityNoise, heightNoise);
    }

    public int GetTerrainHeight(int x, int z)
    {
        for (int y = 320; y > 64; y--)
        {
            if (IsTerrain(x, y, z))
            {
                return y;
            }
        }
        return 0;
    }

    public bool IsTerrain(int x, int y, int z)
    {
        return terrainSelector.GetValue(x, (y+22)*2, z) > 0;
    }

    public Module Cave => new Turbulence()
    {
        Frequency = 0.1234,
        Power = 1,
        Roughness = 3,
        Seed = seed + 1,
        Source0 = new Max()
        {
            Source0 = tunnels,
            Source1 = new ScalePoint
            {
                XScale = 1 / 1024.0,
                YScale = 1 / 384.0,
                ZScale = 1 / 1024.0,
                Source0 = new Curve
                {
                    ControlPoints = new List<ControlPoint>()
                {
                     new Curve.ControlPoint(-1, -1),
                     new Curve.ControlPoint(-0.7, -0.5),
                     new Curve.ControlPoint(-0.4, -0.5),
                     new Curve.ControlPoint(1, 1),
                },
                    Source0 = new Billow
                    {
                        Frequency = 18.12345,
                        Seed = seed + 2,
                        Quality = SharpNoise.NoiseQuality.Fast,
                        OctaveCount = 6,
                        Lacunarity = 1.2234,
                        Persistence = 1.23
                    }
                }
            }
        }
    };

    public Module Decoration => new Multiply
    {
        Source0 = new Checkerboard(),
        Source1 = new Perlin
        {
            Frequency = 1.14,
            Lacunarity = 2.222,
            Seed = seed + 3
        }
    };

    public Module Biome => new Cache
    {
        Source0 = biomeSelector
    };

    // Set a constant biome here for development
    //public Module Biome => new Constant() { ConstantValue = (int)Biomes.Forest };
}
