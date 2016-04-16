using UnityEngine;

public class Chunk : MonoBehaviour
{
	public static readonly Vector3 ChunkSize = new Vector3(16, 16, 16);

	[SerializeField]
	private GameObject _block;

	[SerializeField]
	private MeshFilter _meshFilter;

	[SerializeField]
	private MeshCollider _meshCollider;

	private Mesh _mesh;

	private Voxel[,,] _blocks;
	private Vector3 _chunkPosition;

	public void Initialize(Vector3 chunkPosition)
	{
		_chunkPosition = chunkPosition;

		_blocks = new Voxel[(int)ChunkSize.x, (int)ChunkSize.y, (int)ChunkSize.z];
	}

	public void SetVoxel(Voxel voxel, int x, int y, int z)
	{
		_blocks[x, y, z] = voxel;

		if(voxel.on)
		{
			var chunkPos = new Vector3(_chunkPosition.x * ChunkSize.x, _chunkPosition.y * ChunkSize.y, _chunkPosition.z * ChunkSize.z);
			//remove this once mesh generation is in
			GameObject block = (GameObject)Instantiate(_block, chunkPos + new Vector3(x, y, z), Quaternion.Euler(0, 0, 0));
			block.transform.SetParent(transform);
		}
	}

	public void UpdateMesh()
	{
		_mesh = MeshGenerator.GenerateMesh(_chunkPosition);
		_meshFilter.mesh = _mesh;
		_meshCollider.sharedMesh = _mesh;
	}

	public void UnloadChunk()
	{
		Debug.Log("chunk removed");
		/*foreach(Transform child in transform)
		{
			GameObject.Destroy(child.gameObject);
		}*/
		Destroy(gameObject);
		/*int children = transform.childCount;
		for(int i = 0; i < children; i++)
			transform.GetChild(i).;*/
	}

    public bool VoxelAt(int x, int y, int z)
    {
        return _blocks[x, y, z].on;
    }
}
