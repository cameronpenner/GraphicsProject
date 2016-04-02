using UnityEngine;

internal class WorldSampler
{
	private float TerrainAmplitude = 10;
	private float TerrainOffset = 4;
	private float NoiseScale = 0.05f;

	public Voxel SamplePosition(int x, int y, int z)
	{
		Voxel voxel = new Voxel();
		voxel.on = false;

		var height = TerrainAmplitude * Mathf.PerlinNoise(NoiseScale * x, NoiseScale * z) + TerrainOffset;
		//Debug.Log(height);
		//Debug.Log("Height at [" + x + ", " + y+"]: " + Mathf.PerlinNoise(NoiseScale * x, NoiseScale * z));
		if(y < height)
			voxel.on = true;
		/*if(y < 16)
		{
			voxel.on = true;
		}
		else if(y == 16)
		{
			if(Random.Range(0, 2) < 1)
			{
				voxel.on = false;
			}
			else
			{
				voxel.on = true;
			}
		}
		else
		{
			voxel.on = false;
		}*/

		return voxel;
	}
}
