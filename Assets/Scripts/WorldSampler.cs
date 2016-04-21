using UnityEngine;

internal class WorldSampler
{
	private float TerrainAmplitude = 10;
	private float TerrainOffset = 4;
	private float NoiseScale = 0.04f;

	private float NoiseOffset = 1000;

	public Voxel SamplePosition(int x, int y, int z)
	{
		Voxel voxel = new Voxel();
		voxel.on = false;

		var height = PerlinSample(x, z);// = TerrainAmplitude * Mathf.PerlinNoise(NoiseScale * x + NoiseOffset, NoiseScale * z + NoiseOffset) + TerrainOffset;

		if(y < height)// && y+1 > height)
			voxel.on = true;

		return voxel;
	}

	private float PerlinSample(float x, float z)
	{
		var height = (TerrainAmplitude/3) * Mathf.PerlinNoise(NoiseScale * x + NoiseOffset, NoiseScale * z + NoiseOffset);
		height += TerrainAmplitude * 4 * Mathf.PerlinNoise((NoiseScale /10)* x + NoiseOffset, (NoiseScale/10) * z + NoiseOffset);
		//height += TerrainOffset;
		return height;
	}
}
