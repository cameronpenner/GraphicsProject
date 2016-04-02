using UnityEngine;

public class Chunk : MonoBehaviour
{
	[SerializeField]
	private GameObject _block;

	private Voxel[,,] _blocks;
	private Vector3 _chunckPosition;

	private void Awake()
	{
		if(_chunckPosition == null)
		{
			_chunckPosition = new Vector3(0, 0, 0);
		}

		_blocks = new Voxel[32, 32, 32];
	}

	public void Initialize(Vector3 chunkPosition)
	{
		_chunckPosition = chunkPosition;
	}

	public void SetVoxel(Voxel voxel, int x, int y, int z)
	{
		_blocks[x, y, z] = voxel;

		if(voxel.on)
		{
			GameObject block = (GameObject)Instantiate(_block, _chunckPosition + new Vector3(x, y, z), Quaternion.Euler(0, 0, 0));
			block.transform.SetParent(transform);
		}
	}
}
