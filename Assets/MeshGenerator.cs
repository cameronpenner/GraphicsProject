using System.Collections.Generic;
using UnityEngine;

internal class MeshGenerator
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
        {'z', new int[][] { new int[] { 1, 2, 3, 4 }, new int[] { 5, 6, 7, 8 }, new int[] { 9, 10, 11, 12 } } }
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

                        int[] triangles = getTriangles(vertices);
						if(triangles != null)
						{
                            // If inverted, switch the winding of each triangle by swapping the first two elements
                            if (inverted)
                            {
                                Debug.Log("doing inverted stuff");
                                for (int i = 0; i < triangles.Length - 1; i += 3)
                                {
                                    int temp = triangles[i];
                                    triangles[i] = triangles[i + 1];
                                    triangles[i + 1] = temp;
                                }
                            }

							for (int tri = 0; tri < triangles.Length; tri++)
							{
								newTriangles.Add(triangles[tri] + newVertices.Count);
							}

							for(int m = 0; m < Midpoints.Length; m++)
							{
								newVertices.Add(Midpoints[m] + new Vector3(x, y, z));
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
    
	private static int[] getTriangles(int[] matchVertices)
	{
        int[] triangles = null;

        // Iterate through all possible rotations to find match
        for (int rx = 0; rx < 4; rx++)
		{
			for(int ry = 0; ry < 4; ry++)
			{
				int matchHashKey = getHashKey(matchVertices);
				VerticesToTriangles.TryGetValue(matchHashKey, out triangles);

				if(triangles != null)
				{
                    //rotateOnIndex(triangles, -rx, MidpointRotationIndex['x']);
                    //rotateOnIndex(triangles, -ry, MidpointRotationIndex['y']);
                    return triangles;
                }

				rotateOnIndex(matchVertices, 1, VertexRotationIndex['y']);
			}
                
			for(int rz = 0; rz < 4; rz++)
			{
                int matchHashKey = getHashKey(matchVertices);
				VerticesToTriangles.TryGetValue(matchHashKey, out triangles);

				if(triangles != null)
                {
                    //rotateOnIndex(triangles, rx, MidpointRotationIndex['x']);
                    //rotateOnIndex(triangles, rz, MidpointRotationIndex['z']);
                    //return triangles;
				}
                rotateOnIndex(matchVertices, 1, VertexRotationIndex['z']);
            }
			
			rotateOnIndex(matchVertices, 1, VertexRotationIndex['x']);
		}

		// Should never fail
		//Debug.Log("Return null set of triangles");
		return null;
	}

	public static void rotateOnIndex(int[] toRotate, int rotateTimes, int[][] rotationIndex)
	{
		if(rotationIndex == null)
		{
			Debug.LogError("Must Provide valid rotationIndex to RotateOnIndex method");
			return;
		}
        if(rotateTimes == 0) // don't waste time if we're not rotating
        {
            return;
        }

		for(int i = 0; i < toRotate.Length; i++)
		{
            bool found = false;
            for (int findIndex = 0; !found && findIndex < rotationIndex.Length; findIndex++)
            {
                int length = rotationIndex[findIndex].Length;
                int indexOfVertex = System.Array.IndexOf(rotationIndex[findIndex], toRotate[i]);
                if (indexOfVertex != -1)
                {
                    found = true;
                    toRotate[i] = rotationIndex[findIndex][(indexOfVertex + rotateTimes + length) % length];
                }
            }
		}
		return;
	}

	private static int getHashKey(int[] vertices)
	{
		int key = 0;
		for (int v = 0; v < vertices.Length; v++)
		{
			key += 1 << vertices[v];
		}
		return key;
	}
}
