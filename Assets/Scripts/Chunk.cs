using UnityEngine;

public class Chunk : MonoBehaviour
{
	public static readonly Vector3 ChunkSize = new Vector3(16, 16, 16);

	[SerializeField]
	private GameObject _water;

	private GameObject _player;

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

		if(chunkPosition.y == 0)
		{
			GameObject water = (GameObject)Instantiate(_water, transform.position, transform.rotation);
			water.transform.parent = transform;
		}
	}

	public void SetVoxel(Voxel voxel, int x, int y, int z)
	{
		_blocks[x, y, z] = voxel;
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

	public bool VoxelValueAt(int x, int y, int z)
	{
		return _blocks[x, y, z].on;
	}

	public Voxel VoxelAt(int x, int y, int z)
	{
		return _blocks[x, y, z];
	}

	public Voxel VoxelAt(Vector3 coord)
	{
		return _blocks[(int)coord.x, (int)coord.y, (int)coord.z];
	}
}
