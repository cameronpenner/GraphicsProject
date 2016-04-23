using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{
	private static Dictionary<Vector3, Chunk> _loadedChunks;
	private static Mesh[] meshArray;

	public static readonly Dictionary<char, int[][]> VertexRotationIndex = new Dictionary<char, int[][]>
	{
		{'x', new int[][] { new int[] { 0, 4, 7, 3 }, new int[] { 1, 5, 6, 2 } } },
		{'y', new int[][] { new int[] { 0, 1, 5, 4 }, new int[] { 3, 2, 6, 7 } } },
		{'z', new int[][] { new int[] { 0, 1, 2, 3 }, new int[] { 4, 5, 6, 7 } } }
	};

	public static readonly Dictionary<char, int[][]> MidpointRotationIndex = new Dictionary<char, int[][]>
	{
		{'x', new int[][] { new int[] { 1, 5, 7, 3 }, new int[] { 2, 10, 6, 12 }, new int[] { 4, 9, 8, 11 } } },
		{'y', new int[][] { new int[] { 1, 10, 5, 9 }, new int[] { 2, 6, 8, 4 }, new int[] { 3, 12, 7, 11 } } },
		{'z', new int[][] { new int[] { 1, 2, 3, 4 }, new int[] { 5, 6, 7, 8 }, new int[] { 9, 10, 12, 11 } } }
	};

	// The midpoints on each edge of a cube used with Triangle indices to create mesh
	private static readonly Vector3[] Midpoints = {
		new Vector3(0,      0,      0),     // centre
		new Vector3(0.5f    ,0,     0),     // 1
		new Vector3(1,      .5f,    0),     // 2
		new Vector3(0.5f,   1,      0),     // 3
		new Vector3(0,      0.5f,   0),     // 4
		new Vector3(0.5f,    0,     1),     // 5
		new Vector3(1,      0.5f,   1),     // 6
		new Vector3(0.5f,   1,      1),     // 7
		new Vector3(0,      0.5f,   1),     // 8
		new Vector3(0,      0,      0.5f),  // 9
		new Vector3(1,      0,      0.5f),  // 10
		new Vector3(0,      1,      0.5f),  // 11
		new Vector3(1,      1,      0.5f),  // 12
	};

	// Dictionary of Active Voxels corresponding to Triangle Indices.
	private static int[] VerticesToTriangles(int hash)
	{
		switch(hash)
		{
			case (1): // 1
				return new int[]
				{
					1, 4, 9
				};

			case (3): // 2
				return new int[]
				{
					2, 4, 9,
					2, 9, 10
				};

			case (5): // 3
				return new int[]
				{
					1, 4, 9,
					2, 3, 12
				};

			case (65): // 4
				return new int[]
				{
					1, 4, 9,
					6, 7, 12
				};

			case (50): // 5
				return new int[]
				{
					1, 9, 2,
					2, 9, 8,
					2, 8, 6
				};

			case (67): // 6
				return new int[]
				{
					2, 4, 9,
					2, 9, 10,
					6, 7, 12
				};

			case (74): // 7
				return new int[]
				{
					3, 4, 11,
					1, 2, 10,
					6, 7, 12
				};

			case (51): // 8
				return new int[]
				{
					6, 4, 8,
					2, 4, 6
				};

			case (177): // 9
				return new int[]
				{
					4, 11, 7,
					4, 7, 1,
					6, 1, 7,
					6, 10, 1
				};

			case (105): // 10
				return new int[]
				{
					3, 9, 11,
					3, 9, 1,
					5, 7, 12,
					5, 10, 12
				};

			case (113): // 11
				return new int[]
				{
					1, 4, 8,
					1, 8, 12,
					7, 12, 8,
					1, 12, 10
				};

			case (58): // 12
				return new int[]
				{
					3, 4, 11,
					1, 9, 2,
					8, 2, 9,
					8, 6, 2
				};

			case (165): // 13
				return new int[]
				{
					1, 4, 9,
					2, 3, 12,
					5, 6, 10,
					7, 8, 11
				};

			case (178): // 14
				return new int[]
				{
					1, 9, 11,
					1, 11, 6,
					1, 6, 2,
					7, 6, 11
				};
		}

		return null;
	}

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

		for(int x = 0; x < Chunk.ChunkSize.x; x++)
		{
			for(int y = 0; y < Chunk.ChunkSize.y; y++)
			{
				for(int z = 0; z < Chunk.ChunkSize.z; z++)
				{
					bool[] cube =
					{
						VoxelSelector.SelectVoxelValue(chunkPosition, x, y, z),
						VoxelSelector.SelectVoxelValue(chunkPosition, x+1, y, z),
						VoxelSelector.SelectVoxelValue(chunkPosition, x+1, y+1, z),
						VoxelSelector.SelectVoxelValue(chunkPosition, x, y+1, z),
						VoxelSelector.SelectVoxelValue(chunkPosition, x, y, z+1),
						VoxelSelector.SelectVoxelValue(chunkPosition, x+1, y, z+1),
						VoxelSelector.SelectVoxelValue(chunkPosition, x+1, y+1, z+1),
						VoxelSelector.SelectVoxelValue(chunkPosition, x, y+1, z+1),
					};

					processCube(cube, newTriangles, newVertices, x, y, z);
				}
			}
		}

		//turn list to vertex array
		mesh.vertices = newVertices.ToArray();
		//turn list into triangle array
		mesh.triangles = newTriangles.ToArray();

		return mesh;
	}

	private static int[] getTriangles(int[] matchVertices)
	{
		int[] triangles = null;

		// Iterate through all possible rotations to find match
		for(int rx = 0; rx < 4; rx++)
		{
			for(int ry = 0; ry < 4; ry++)
			{
				int matchHashKey = getHashKey(matchVertices);
				triangles = VerticesToTriangles(matchHashKey);

				if(triangles != null)
				{
					triangles = rotateOnIndex(triangles, -ry, MidpointRotationIndex['y']);
					triangles = rotateOnIndex(triangles, -rx, MidpointRotationIndex['x']);
					return triangles;
				}

				matchVertices = rotateOnIndex(matchVertices, 1, VertexRotationIndex['y']);
			}

			for(int rz = 0; rz < 4; rz++)
			{
				int matchHashKey = getHashKey(matchVertices);
				triangles = VerticesToTriangles(matchHashKey);

				if(triangles != null)
				{
					triangles = rotateOnIndex(triangles, -rz, MidpointRotationIndex['z']);
					triangles = rotateOnIndex(triangles, -rx, MidpointRotationIndex['x']);
					return triangles;
				}
				matchVertices = rotateOnIndex(matchVertices, 1, VertexRotationIndex['z']);
			}

			matchVertices = rotateOnIndex(matchVertices, 1, VertexRotationIndex['x']);
		}

		// Should never fail
		//Debug.Log("Return null set of triangles");
		return null;
	}

	public static int[] rotateOnIndex(int[] toRotate, int rotateTimes, int[][] rotationIndex)
	{
		if(rotationIndex == null)
		{
			Debug.LogError("Must Provide valid rotationIndex to RotateOnIndex method");
			return toRotate;
		}
		if(rotateTimes == 0) // don't waste time if we're not rotating
		{
			return toRotate;
		}

		int[] newArray = (int[])toRotate.Clone();

		for(int i = 0; i < newArray.Length; i++)
		{
			bool found = false;
			for(int findIndex = 0; !found && findIndex < rotationIndex.Length; findIndex++)
			{
				int length = rotationIndex[findIndex].Length;
				int indexOfVertex = System.Array.IndexOf(rotationIndex[findIndex], newArray[i]);
				if(indexOfVertex != -1)
				{
					found = true;
					newArray[i] = rotationIndex[findIndex][(indexOfVertex + rotateTimes + length) % length];
				}
			}
			if(!found)
			{
				Debug.LogError("Could not rotate!");
			}
		}
		return newArray;
	}

	private static int getHashKey(int[] vertices)
	{
		int key = 0;
		for(int v = 0; v < vertices.Length; v++)
		{
			key += 1 << vertices[v];
		}
		return key;
	}

	private static void processCube(bool[] cube, List<int> newTriangles, List<Vector3> newVertices, int x, int y, int z)
	{
		int numTrue = 0;
		foreach(bool b in cube)
		{
			if(b) numTrue++;
		}
		if(numTrue != 0 && numTrue != 8)
		{
			bool inverted = false;
			int[] vertices = null;

			// Invert values if more than 4 voxels are on
			if(numTrue > 4)
			{
				inverted = true;
				vertices = new int[8 - numTrue];
			}
			else
			{
				vertices = new int[numTrue];
			}

			int vert_index = 0;
			for(int i = 0; i < cube.Length; i++)
			{
				if(cube[i] != inverted)
				{
					vertices[vert_index] = i;
					vert_index++;
				}
			}

			int[] triangles = getTriangles(vertices);
			if(triangles != null)
			{
				// If inverted, switch the winding of each triangle by swapping the first two elements
				if(inverted)
				{
					for(int i = 0; i < triangles.Length - 1; i += 3)
					{
						int temp = triangles[i];
						triangles[i] = triangles[i + 1];
						triangles[i + 1] = temp;
					}
				}

				for(int tri = 0; tri < triangles.Length; tri++)
				{
					newTriangles.Add(newVertices.Count);
					newVertices.Add(Midpoints[triangles[tri]] + new Vector3(x, y, z));
				}
			}
		}
	}

	private static bool hasNullChunks(Chunk[] chunks)
	{
		foreach(Chunk chunk in chunks)
		{
			if(chunk == null)
			{
				return true;
			}
		}
		return false;
	}
}
