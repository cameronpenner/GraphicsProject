using UnityEngine;

public class RiverBiome : Biome
{
	private float NoiseOffset = 6000;
	private float riverbedHeight = 1;

	public Voxel SamplePosition(int x, int y, int z)
	{
		Voxel voxel = new Voxel();
		voxel.on = false;

		if(y < riverbedHeight)// && y+1 > height)
			voxel.on = true;

		return voxel;
	}

	public float GroundHeight(int x, int z)
	{
		return riverbedHeight;
	}

	public float Bias(int x, int z)
	{
		float thinness = 50;//higher is thinner;
		float flatness = 3;//make this smaller than thinness and >= 1

		float bias = Mathf.PerlinNoise(x / 200f + .8327094f, z / 200f + .1926389f);//set to smooth noise
		bias = Mathf.Abs(0.5f - bias); //create a ridge along the .5 value height in the noise (between 0 and .5 where 0 is the ridge)
		bias = (0.5f - bias)*2;//between 0 and 1, 1 is the ridge
		bias = bias * thinness - thinness + flatness;
		bias = Mathf.Clamp(bias, 0, 1);//clamp bias

		return bias;
	}
}
