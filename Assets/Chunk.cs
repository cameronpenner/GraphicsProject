using UnityEngine;

public class Chunk : MonoBehaviour
{
	public static readonly Vector3 ChunkSize = new Vector3(16,16,16);

	[SerializeField]
	private GameObject _block;

	private Voxel[,,] _blocks;
	private Vector3 _chunckPosition;

	public void Initialize(Vector3 chunkPosition)
	{
		_chunckPosition = chunkPosition;

		_blocks = new Voxel[(int)ChunkSize.x, (int)ChunkSize.y, (int)ChunkSize.z];
	}

	public void SetVoxel(Voxel voxel, int x, int y, int z)
	{
		_blocks[x, y, z] = voxel;

		if(voxel.on)
		{
			var chunkPos = new Vector3(_chunckPosition.x * ChunkSize.x, _chunckPosition.y * ChunkSize.y, _chunckPosition.z * ChunkSize.z);
			GameObject block = (GameObject)Instantiate(_block, chunkPos + new Vector3(x, y, z), Quaternion.Euler(0, 0, 0));
			block.transform.SetParent(transform);
		}
	}
}
