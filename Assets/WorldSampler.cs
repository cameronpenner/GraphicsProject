using UnityEngine;

internal class WorldSampler
{
	private float TerrainAmplitude = 10;
	private float TerrainOffset = 4;
	private float NoiseScale = 0.02f;

	private float NoiseOffset = 1000;

	public Voxel SamplePosition(int x, int y, int z)
	{
		Voxel voxel = new Voxel();
		voxel.on = false;

		var height = TerrainAmplitude * Mathf.PerlinNoise(NoiseScale * x + NoiseOffset, NoiseScale * z + NoiseOffset) + TerrainOffset;

		if(y < height && y+1 > height)
			voxel.on = true;

		return voxel;
	}
}
