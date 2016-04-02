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
		GenerateChunk(32, 0, 0);
	}

	private void Update()
	{
	}

	private Chunk GenerateChunk(int chunkX, int chunkY, int chunkZ)
	{
		GameObject chunkGO = (GameObject)Instantiate(_chunkPrefab, new Vector3(chunkX, chunkY, chunkZ), Quaternion.Euler(0, 0, 0));
		Chunk chunk = chunkGO.GetComponent<Chunk>();
		chunk.Initialize(new Vector3(chunkX, chunkY, chunkZ));

		for(int x = 0; x < 32; x++)
		{
			for(int y = 0; y < 32; y++)
			{
				for(int z = 0; z < 32; z++)
				{
					var voxel = _sampler.SamplePosition(x, y, z);

					chunk.SetVoxel(voxel, x, y, z);
				}
			}
		}

		return chunk;
	}
}
