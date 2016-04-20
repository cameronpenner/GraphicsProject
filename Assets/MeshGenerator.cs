using System.Collections.Generic;
using UnityEngine;

internal class MeshGenerator
{
	private static Dictionary<Vector3, Chunk> _loadedChunks;
	private static Mesh[] meshArray;

	private static readonly Dictionary<char, int[][]> RotationIndex = new Dictionary<char, int[][]>
	{
		{'x', new int[][] { new int[] { 0, 4, 7, 3 }, new int[] { 1, 5, 6, 2 } } },
		{'y', new int[][] { new int[] { 0, 1, 5, 4 }, new int[] { 3, 2, 6, 7 } } },
		{'z', new int[][] { new int[] { 0, 1, 2, 3 }, new int[] { 4, 5, 6, 7 } } }
	};

	// The midpoints on each edge of a cube used with Triangle indices to create mesh
	private static readonly Vector3[] BaseVertices = {
		new Vector3(0,      0,      0),
		new Vector3(0.5f    ,0,     0),
		new Vector3(1,      .5f,    0),
		new Vector3(0.5f,   1,      0),
		new Vector3(0,      0.5f,   0),
		new Vector3(0.5f,    0,     1),
		new Vector3(1,      0.5f,   1),
		new Vector3(0.5f,   1,      1),
		new Vector3(0,      0.5f,   1),
		new Vector3(0,      0,      0.5f),
		new Vector3(1,      0,      0.5f),
		new Vector3(1,      1,      0.5f),
		new Vector3(0,      1,      0.5f),
	};

	// Dictionary of Active Voxels corresponding to Triangle Indices.
	private static readonly Dictionary<int, int[]> VerticesToTriangles = new Dictionary<int, int[]>
	{
		{ // 0
            0,
			new int[]
			{}
		},
		{ // 1
            1,
			new int[]
			{
				1, 4, 9
			}
		},
		{ // 2
            3,
			new int[]
			{
				2, 4, 9,
				2, 9, 10
			}
		},
		{ // 3 NONE GENERATED??? Test when more advancet terrain is implemented
            5,
			new int[]
			{
				1, 4, 9,
				2, 3, 12
			}
		},
        { // 4 NONE GENERATED??? Test when more advancet terrain is implemented
            65,
			new int[]
			{
				1, 4, 9,
				6, 7, 12
			}
		},
		{ // 5
            50,
			new int[]
			{
				1, 9, 2,
				2, 9, 8,
				2, 8, 6
			}
		},
        { // 6 NONE GENERATED??? Test when more advancet terrain is implemented
            67,
			new int[]
			{
				2, 4, 9,
				2, 9, 10,
				6, 7, 12
			}
		},
        { // 7 NONE GENERATED??? Test when more advancet terrain is implemented
            74,
			new int[]
			{
				3, 4, 11,
				1, 2, 10,
				6, 7, 12
			}
		},
		{ // 8
            51,
			new int[]
			{
				6, 4, 8,
				2, 4, 6,
			}
		},
        { // 9 NONE GENERATED??? Test when more advancet terrain is implemented
            177,
			new int[]
			{
				4, 11, 7,
				4, 7, 1,
				6, 1, 7,
				6, 10, 1,
			}
		},
        { // 10 NONE GENERATED??? Test when more advancet terrain is implemented
            105,
			new int[]
			{
				3, 9, 11,
				3, 9, 1,
				5, 7, 12,
				5, 10, 12
			}
		},
        { // 11 NONE GENERATED??? Test when more advancet terrain is implemented
            113,
			new int[]
			{
				1, 4, 8,
				1, 8, 12,
			    7, 12, 8,
				1, 12, 10,
			}
		},
        { // 12
            58,
			new int[]
			{
				3, 4, 11,
				1, 9, 2,
			    8, 2, 9,
				8, 6, 2,
			}
		},
        { // 13 NONE GENERATED??? Test when more advancet terrain is implemented
            165,
			new int[]
			{
				1, 4, 9,
				2, 3, 12,
				5, 6, 10,
				7, 8, 11,
			}
		},
		{ // 14
            178,
			new int[]
			{
				1, 9, 11,
				1, 11, 6,
				1, 6, 2,
				7, 6, 11,
			}
		}
	};

	public static void SetChunkReference(Dictionary<Vector3, Chunk> loadedChunks)
	{
		_loadedChunks = loadedChunks;
	}

	public static Mesh GenerateMesh(Vector3 chunkPosition)
	{
		Mesh mesh = new Mesh();
		Chunk chunk = null;
		_loadedChunks.TryGetValue(chunkPosition, out chunk);

		if(chunk == null)
		{
			return null;
		}

		List<Vector3> newVertices = new List<Vector3>();
		List<int> newTriangles = new List<int>();

		for(int x = 0; x < Chunk.ChunkSize.x - 1; x++)
		{
			for(int y = 0; y < Chunk.ChunkSize.y - 1; y++)
			{
				for(int z = 0; z < Chunk.ChunkSize.z - 1; z++)
				{
					bool[] cube =
					{
						chunk.VoxelAt(x, y, z),
						chunk.VoxelAt(x+1, y, z),
						chunk.VoxelAt(x+1, y+1, z),
						chunk.VoxelAt(x, y+1, z),
						chunk.VoxelAt(x, y, z+1),
						chunk.VoxelAt(x+1, y, z+1),
						chunk.VoxelAt(x+1, y+1, z+1),
						chunk.VoxelAt(x, y+1, z+1),
					};

					int numTrue = 0;
					foreach(bool b in cube)
					{
						if(b) numTrue++;
					}
					if(numTrue != 0 && numTrue != 8)
					{
						bool inverted = false;
						// Invert values if more than 4 voxels are on
						if(numTrue > 4)
						{
							inverted = true;
							for(int b = 0; b < cube.Length; b++)
							{
								cube[b] = !cube[b];
							}
						}

						int[] vertices = new int[numTrue];
						int vert_index = 0;
						for(int i = 0; i < cube.Length; i++)
						{
							if(cube[i])
							{
								vertices[vert_index] = i;
								vert_index++;
							}
						}

						if(getTriangles(vertices) != null)
						{
							foreach(int tri in getTriangles(vertices))
							{
								newTriangles.Add(tri + newVertices.Count);
							}

							foreach(Vector3 vertex in BaseVertices)
							{
								newVertices.Add(vertex + new Vector3(x, y, z));
							}
						}
						
					}
				}
			}
		}

		//turn list to vertex array
		mesh.vertices = newVertices.ToArray();
		//turn list into triangle array
		mesh.triangles = newTriangles.ToArray();

		return mesh;
	}

	private static void addToMesh(Mesh mesh, int x, int y, int z, int[] addTriangles)
	{
		Vector3[] vertices = mesh.vertices;
		//Vector2[] uv = mesh.uv;
		int[] triangles = mesh.triangles;

		// Transform and add new vertices
		Vector3[] newVertices = new Vector3[vertices.Length + BaseVertices.Length];
		for(int i = 0; i < vertices.Length; i++)
		{
			newVertices[i] = vertices[i];
		}

		//Rotate

		//Translate

		for(int i = 0; i < BaseVertices.Length; i++)
		{
			newVertices[i + vertices.Length] = BaseVertices[i] + new Vector3(x, y, z);
		}

		int[] newTriangles = new int[triangles.Length + addTriangles.Length];

		// Add triangles according to indexes of new vertices

		// Reassign to mesh
		mesh.vertices = newVertices;
		//mesh.uv = uv;
		mesh.triangles = newTriangles;
	}

	//TODO return rotation reference along with triangles, or rotated vertices
	private static int[] getTriangles(int[] matchVertices)
	{
		// Iterate through 14 templates * 24 possible rotations to find match
		int[] triangles = null;
		for(int rx = 0; rx < 4; rx++)
		{
			for(int templateIndex = 1; templateIndex < VerticesToTriangles.Count; templateIndex++)
			{
				// Iterate through 16 rotations in x-y rotations
				for(int ry = 0; ry < 4; ry++)
				{
					int matchHashKey = getHashKey(matchVertices);
					VerticesToTriangles.TryGetValue(matchHashKey, out triangles);

					if(triangles != null)
					{
						return triangles;
					}

					rotateVertices(matchVertices, 1, 'y');
				}

				// Iterate through remaining 8 rotations in x-z rotations
				for(int rz = 1; rz < 3; rz += 2)
				{
					int matchHashKey = getHashKey(matchVertices);
					VerticesToTriangles.TryGetValue(matchHashKey, out triangles);

					if(triangles != null)
					{
						return triangles;
					}

					rotateVertices(matchVertices, rz, 'z');
				}
				rotateVertices(matchVertices, 1, 'z'); // Rotate back to original position in z-axis
			}
			rotateVertices(matchVertices, 1, 'x');
		}

		// Should never fail
		Debug.Log("Return null set of triangles");
		return null;
	}

	private static void rotateVertices(int[] vertices, int rotateTimes, char rotateDimension)
	{
		int[][] rotationIndex = null;
		RotationIndex.TryGetValue(rotateDimension, out rotationIndex);

		if(rotationIndex == null)
		{
			Debug.LogError("Invalid Rotate Dimension: must be x y or z");
			return;
		}

		for(int i = 0; i < vertices.Length; i++)
		{
			for(int r = 0; r < rotateTimes; r++)
			{
				int indexOf = System.Array.IndexOf(rotationIndex[0], vertices[i]);
				if(indexOf != -1)
				{
					vertices[i] = rotationIndex[0][(indexOf + 1) % rotationIndex[0].Length];
				}
				else
				{
					indexOf = System.Array.IndexOf(rotationIndex[1], vertices[i]);
					vertices[i] = rotationIndex[1][(indexOf + 1) % rotationIndex[1].Length];
				}
			}
		}

		return;
	}

	private static int getHashKey(int[] vertices)
	{
		int key = 0;
		foreach(int v in vertices)
		{
			key += 1 << v;
		}
		return key;
	}
}
