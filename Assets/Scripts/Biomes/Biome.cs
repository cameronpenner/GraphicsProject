using UnityEngine;

public interface Biome
{
	Voxel SamplePosition(int x, int y, int z);

	float GroundHeight(int x, int z);

	float Bias(int x, int z);
}
