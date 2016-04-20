using System.Collections;
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
	private int _viewDistance = 1;

	private Dictionary<Vector3, Chunk> _loadedChunks;

	private void Start()
	{
		_sampler = new WorldSampler();
		_loadedChunks = new Dictionary<Vector3, Chunk>();
		MeshGenerator.SetChunkReference(_loadedChunks);

		_player = ((GameObject)Instantiate(_playerPrefab, new Vector3(0, 110, 0), Quaternion.Euler(0, 0, 0))).transform;

		StartCoroutine("LoadChunks");
		//StartCoroutine("UnloadChunks");
	}

	private IEnumerator LoadChunks()
	{
		while(true)
		{
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
					for(int y = 0; y < 4; y++)
					{
						if(!_loadedChunks.ContainsKey(new Vector3(x, y, z)))
						{
							Chunk chunk = GenerateChunk(x, y, z);
							_loadedChunks.Add(new Vector3(x, y, z), chunk);
                            chunk.UpdateMesh();

                            yield return new WaitForSeconds(.01f);
						}
					}
				}
			}

			yield return new WaitForSeconds(.01f);
		}
        
	}

	private IEnumerator UnloadChunks()
	{
        while (true)
		{
			try
			{
				foreach(Vector3 point in _loadedChunks.Keys)
				{
					//if(startx < point.x || point.x < endx ||
						//startz < point.z || point.z < endz)
					if(Vector3.Distance(point, _player.transform.position / Chunk.ChunkSize.x) > _viewDistance)
					{
						Chunk chunk;
						if(_loadedChunks.TryGetValue(point, out chunk))
						{
							chunk.UnloadChunk();
							_loadedChunks.Remove(point);
							Debug.Log("unloading....");
						}
					}
				}
			}
			catch
			{
				Debug.LogError("OOPS CAN:T DO THINGS");
			}
			yield return new WaitForSeconds(.1f);
		}
        
	}

	private Chunk GenerateChunk(int chunkX, int chunkY, int chunkZ)
	{
		GameObject chunkGO = (GameObject)Instantiate(_chunkPrefab, new Vector3(chunkX*Chunk.ChunkSize.x, chunkY * Chunk.ChunkSize.y, chunkZ * Chunk.ChunkSize.z), Quaternion.Euler(0, 0, 0));
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
				}
			}
		}

		return chunk;
	}
}
