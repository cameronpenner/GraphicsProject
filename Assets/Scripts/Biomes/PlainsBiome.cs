using UnityEngine;

public class PlainsBiome : Biome
{
	private float TerrainAmplitude = 10;
	private float TerrainOffset = 4;
	private float NoiseScale = 0.04f;

	private float NoiseOffset = 1000;

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
		var height = TerrainAmplitude * 4 * Mathf.PerlinNoise((NoiseScale / 10) * x + NoiseOffset, (NoiseScale / 10) * z + NoiseOffset);
		height = 4;
		return height;
	}

	public float Bias(int x, int z)
	{
		return Mathf.PerlinNoise(x / 10 + NoiseOffset * 2, z / 10 + NoiseOffset * 2);
	}
}
