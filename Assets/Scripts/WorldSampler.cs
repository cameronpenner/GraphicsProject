using System.Collections.Generic;
using System.Linq;

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
		float blendThreshold = 0.1f;
		Dictionary<float, Biome> biomes = new Dictionary<float, Biome>();

		//get biases for biomes
		foreach(Biome biome in _biomes)
		{
			float bias = biome.Bias(x, z);
			biomes.Add(bias, biome);
		}

		//find most biased biome
		float max = biomes.Keys.Max();

		//used to average biomes when there's more than one
		float amountBlended = 0;

		foreach(float bias in biomes.Keys)
		{
			float diff = (max - bias);

			//if current biome is close enough in bias to the most biased biome
			if(diff <= blendThreshold)
			{
				float visibility = 1 - diff / blendThreshold; //between 0 and 1, 0 being not potent, and 1 being most potent
				amountBlended += visibility;
				height += biomes[bias].GroundHeight(x, z) * visibility;
			}
		}

		height /= amountBlended;

		return height;
	}
}
