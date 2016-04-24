using System.Collections.Generic;
using UnityEngine;

public class WorldSampler
{
	private List<Biome> _biomes;

	public WorldSampler()
	{
		_biomes = new List<Biome>();
		_biomes.Add(new MountainBiome());
		_biomes.Add(new PlainsBiome());
	}

	public Voxel SamplePosition(int x, int y, int z)
	{
		Voxel voxel = new Voxel();
		voxel.on = false;

		float max = 0;
		Biome main = null;

		foreach(Biome biome in _biomes)
		{
			float bias = biome.Bias(x, z);
			//Debug.Log("biome bias: " + bias+", "+biome);
			if(bias > max)
			{
				max = bias;
				main = biome;
			}
		}

		return main.SamplePosition(x, y, z);
	}

	public float GroundHeight(int x, int z)
	{
        float height = 0f;
        float mountainThresh = 0.9f;
        float plainsThresh = 0.4f;
        float mult = 1 / (mountainThresh - plainsThresh);

        float bias = Mathf.PerlinNoise(x / 100f + 0.5f, z / 100f + 0.5f);

        if (bias > mountainThresh)
        {
            height = _biomes[0].GroundHeight(x, z);
        }
        else if (bias < plainsThresh)
        {
            height = _biomes[1].GroundHeight(x, z);
        }
        else
        {
            float weightMtns = Mathf.Pow(bias - plainsThresh, 2) * Mathf.Pow(mult, 2);
            height += _biomes[0].GroundHeight(x, z) * weightMtns;
            height += _biomes[1].GroundHeight(x, z) * (1 - weightMtns);
        }
        
        return height;
	}
}
