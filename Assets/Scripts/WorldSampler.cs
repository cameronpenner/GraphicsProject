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
		float max = 0;
		Biome main = null;

		foreach(Biome biome in _biomes)
		{
			float bias = biome.Bias(x, z);
			//Debug.Log("biome bias: " + bias + ", " + biome);
			if(bias > max)
			{
				max = bias;
				main = biome;
			}
		}

		return main.GroundHeight(x, z);
	}
}
