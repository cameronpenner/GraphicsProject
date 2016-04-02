using UnityEngine;

public class Chunk : MonoBehaviour
{
	[SerializeField]
	private GameObject _block;

	private Voxel[,,] _blocks;
	private Vector3 _chunckPosition;

	private void Start()
	{
		if(_chunckPosition == null)
		{
			_chunckPosition = new Vector3(0, 0, 0);
		}

		_blocks = new Voxel[32, 32, 32];

		for(int x = 0; x < 32; x++)
		{
			for(int y = 0; y < 32; y++)
			{
				for(int z = 0; z < 32; z++)
				{
					_blocks[x, y, z] = new Voxel();
					if(y < 16)
					{
						_blocks[x,y,z].on = true;
					}
					else if(y == 16)
					{
						if(Random.Range(0, 2) < 1)
						{
							_blocks[x,y,z].on = false;
						}
						else
						{
							_blocks[x,y,z].on = true;
						}
					}
					else
					{
						_blocks[x,y,z].on = false;
					}

					if(_blocks[x, y, z].on)
					{
						GameObject block = (GameObject)Instantiate(_block, _chunckPosition + new Vector3(x, y, z), Quaternion.Euler(0, 0, 0));
						block.transform.SetParent(transform);
					}
				}
			}
		}
	}

	private void Update()
	{
	}
}
