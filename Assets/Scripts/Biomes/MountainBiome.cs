using UnityEngine;

public class MountainBiome : Biome
{
	private float TerrainAmplitude = 100;
	private float TerrainOffset = 4;
	private float NoiseScale = 0.02f;

	private float NoiseOffset = 100;

	public Voxel SamplePosition(int x, int y, int z)
	{
		Voxel voxel = new Voxel();
		voxel.on = false;

		var height = PerlinSample(x, z);

		if(y < height)// && y+1 > height)
			voxel.on = true;

		return voxel;
	}

	public float GroundHeight(int x, int z)
	{
		Voxel voxel = new Voxel();
		voxel.on = false;

		var height = PerlinSample(x, z);

		return height;
	}

	private float PerlinSample(float x, float z)
	{
		var height = TerrainAmplitude * Mathf.PerlinNoise((NoiseScale) * x + NoiseOffset, (NoiseScale) * z + NoiseOffset);

		return height;
	}

	public float Bias(int x, int z)
	{
		return Mathf.PerlinNoise(x / 100f + .6234852f, z / 100f + .1237692f);
	}
}
