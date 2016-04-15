using System.Collections.Generic;
using UnityEngine;

internal class MeshGenerator
{
	private static Dictionary<Vector3, Chunk> _loadedChunks;
	
	public static void SetChunkReference(Dictionary<Vector3, Chunk> loadedChunks)
	{
		_loadedChunks = loadedChunks;
	}

	public static Mesh GenerateMesh(Vector3 chunkPosition)
	{
		Mesh mesh = new Mesh();

		//TODO: Jeff

		return mesh;
	}
}
