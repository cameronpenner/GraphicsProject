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

	public int ViewDistance = 1;

	private Dictionary<Vector3, Chunk> _loadedChunks;

	private void Start()
	{
		_sampler = new WorldSampler();
		_loadedChunks = new Dictionary<Vector3, Chunk>();
		MeshGenerator.SetChunkReference(_loadedChunks);
		VoxelSelector.SetLoadedChunks(_loadedChunks);

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

			int startx = playerx - ViewDistance / 2;
			int startz = playerz - ViewDistance / 2;
			int endx = playerx + ViewDistance / 2;
			int endz = playerz + ViewDistance / 2;

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
							//chunk.UpdateMesh();

							/*for(int chunkx = x-1; chunkx <= x + 1; chunkx++)
							{
								for(int chunky = y - 1; chunkx <= y + 1; chunky++)
								{
									for(int chunkz = z - 1; chunkz <= z + 1; chunkz++)
									{
										Chunk neigbour;
										if(_loadedChunks.TryGetValue(new Vector3(chunkx,chunky,chunkz), out neigbour))
										{
											neigbour.UpdateMesh();
										}
									}
								}
							}*/

							yield return new WaitForSeconds(.001f);
						}
					}
				}
			}

			for(int x = startx; x <= endx; x++)
			{
				for(int z = startz; z <= endz; z++)
				{
					for(int y = 0; y < 4; y++)
					{
						Chunk chunk;
						if(_loadedChunks.TryGetValue(new Vector3(x, y, z), out chunk))
						{
							chunk.UpdateMesh();
							yield return new WaitForSeconds(.001f);
						}
					}
				}
			}

			yield return new WaitForSeconds(.05f);
		}
	}

	private IEnumerator UnloadChunks()
	{
		while(true)
		{
			try
			{
				foreach(Vector3 point in _loadedChunks.Keys)
				{
					//if(startx < point.x || point.x < endx ||
					//startz < point.z || point.z < endz)
					if(Vector3.Distance(point, _player.transform.position / Chunk.ChunkSize.x) > ViewDistance)
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
		GameObject chunkGO = (GameObject)Instantiate(_chunkPrefab, new Vector3(chunkX * Chunk.ChunkSize.x, chunkY * Chunk.ChunkSize.y, chunkZ * Chunk.ChunkSize.z), Quaternion.Euler(0, 0, 0));
		Chunk chunk = chunkGO.GetComponent<Chunk>();
		chunk.Initialize(new Vector3(chunkX, chunkY, chunkZ));

		for(int x = 0; x < Chunk.ChunkSize.x; x++)
		{
			for(int z = 0; z < Chunk.ChunkSize.z; z++)
			{
				int groundHeight = (int)_sampler.GroundHeight(x + chunkX * (int)Chunk.ChunkSize.x, z+chunkZ * (int)Chunk.ChunkSize.z );
				groundHeight -= (int)Chunk.ChunkSize.y * chunkY;

				for(int y = 0; y < groundHeight && y < Chunk.ChunkSize.y; y ++)
				{
					Voxel voxel = new Voxel();
					voxel.on = true;
					chunk.SetVoxel(voxel, x, y, z);
				}

				for(int y = Mathf.Max(0, groundHeight); y < Chunk.ChunkSize.y; y++)
				{
					Voxel voxel = new Voxel();
					voxel.on = false;
					chunk.SetVoxel(voxel, x, y, z);
				}
			}
		}

		return chunk;
	}
}
