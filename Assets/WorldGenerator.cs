using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
	private WorldSampler _sampler;
	private Transform _player;

	[SerializeField]
	private GameObject _playerPrefab;

	[SerializeField]
	private GameObject _chunkPrefab;

	[SerializeField]
	private int _viewDistance = 6;

	private List<Chunk> _loadedChunks;

	private void Start()
	{
		_sampler = new WorldSampler();
		_loadedChunks = new List<Chunk>();

		_player = ((GameObject)Instantiate(_playerPrefab, new Vector3(0, 20, 0), Quaternion.Euler(0, 0, 0))).transform;

		//GenerateChunk(0, 0, 0);
		int playerx = (int)Mathf.Floor(_player.position.x / Chunk.ChunkSize.x);
		int playerz = (int)Mathf.Floor(_player.position.z / Chunk.ChunkSize.z);

		int startx = playerx - _viewDistance / 2;
		int startz = playerz - _viewDistance / 2;
		int endx = playerx + _viewDistance / 2;
		int endz = playerz + _viewDistance / 2;

		for(int x = startx; x <= endx; x++)
		{
			for(int z = startz; z <= endz; z++)
			{
				GenerateChunk(x, 0, z);
			}
		}

	}

	private void Update()
	{
	}

	private Chunk GenerateChunk(int chunkX, int chunkY, int chunkZ)
	{
		GameObject chunkGO = (GameObject)Instantiate(_chunkPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
		Chunk chunk = chunkGO.GetComponent<Chunk>();
		chunk.Initialize(new Vector3(chunkX, chunkY, chunkZ));

		for(int x = 0; x < Chunk.ChunkSize.x; x++)
		{
			for(int y = 0; y < Chunk.ChunkSize.y; y++)
			{
				for(int z = 0; z < Chunk.ChunkSize.z; z++)
				{
					var voxel = _sampler.SamplePosition((int)(chunkX * Chunk.ChunkSize.x + x), (int)(chunkY * Chunk.ChunkSize.y + y), (int)(chunkZ * Chunk.ChunkSize.z) + z);

					chunk.SetVoxel(voxel, x, y, z);
					_loadedChunks.Add(chunk);
				}
			}
		}

		return chunk;
	}
}
