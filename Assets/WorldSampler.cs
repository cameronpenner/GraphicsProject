using UnityEngine;

class WorldSampler
{
	public Voxel SamplePosition(int x, int y, int z)
	{
		Voxel voxel = new Voxel();
		voxel.on = false;

		if(y < 16)
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
		}
		
		return voxel;
	}
}
