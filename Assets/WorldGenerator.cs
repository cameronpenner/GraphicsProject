using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
	private WorldSampler _sampler;

	[SerializeField]
	private GameObject _player;

	[SerializeField]
	private GameObject _chunkPrefab;

	private List<Chunk> _loadedChunks;

	private void Start()
	{
		_sampler = new WorldSampler();
		_loadedChunks = new List<Chunk>();

		GenerateChunk(0, 0, 0);
		GenerateChunk(1, 0, 0);
	}

	private void Update()
	{
	}

	private Chunk GenerateChunk(int chunkX, int chunkY, int chunkZ)
	{
		GameObject chunkGO = (GameObject)Instantiate(_chunkPrefab, new Vector3(0,0,0), Quaternion.Euler(0, 0, 0));
		Chunk chunk = chunkGO.GetComponent<Chunk>();
		chunk.Initialize(new Vector3(chunkX, chunkY, chunkZ));

		for(int x = 0; x < Chunk.ChunkSize.x; x++)
		{
			for(int y = 0; y < Chunk.ChunkSize.y; y++)
			{
				for(int z = 0; z < Chunk.ChunkSize.z; z++)
				{
					var voxel = _sampler.SamplePosition((int)(chunkX*Chunk.ChunkSize.x + x), (int)(chunkY * Chunk.ChunkSize.y + y), (int)(chunkZ * Chunk.ChunkSize.z) + z);

					chunk.SetVoxel(voxel, x, y, z);
				}
			}
		}

		return chunk;
	}
}
