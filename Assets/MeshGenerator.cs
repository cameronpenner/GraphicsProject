using System.Collections.Generic;
using System;
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
    private static readonly Dictionary<int[], int[]> VerticesToTriangles = new Dictionary<int[], int[]>
    {
        { // 0
            new int[] 
            {},
            new int[]
            {}
        },
        { // 1
            new int[]
            {
                1
            },
            new int[]
            {
                1, 4, 9
            }
        },
        { // 2
            new int[]
            {
                1, 2
            },
            new int[]
            {
                2, 4, 9,
                2, 9, 10
            }
        },
        { // 3
            new int[]
            {
                1, 3
            },
            new int[]
            {
                1, 4, 9,
                2, 3, 12
            }
        },
        { // 4
            new int[]
            {
                1, 7
            },
            new int[]
            {
                1, 4, 9,
                6, 7, 12
            }
        },
        { // 5
            new int[]
            {
                2, 5, 6
            },
            new int[]
            {
                1, 2, 9,
                2, 8, 9,
                2, 6, 8
            }
        },
        { // 6
            new int[]
            {
                1, 2, 7
            },
            new int[]
            {
                2, 4, 9,
                2, 9, 10,
                6, 7, 12
            }
        },
        { // 7
            new int[]
            {
                2, 4, 7
            },
            new int[]
            {
                3, 4, 11,
                1, 2, 10,
                6, 7, 12
            }
        },
        { // 8
            new int[]
            {
                1, 2, 5, 6
            },
            new int[]
            {
                4, 6, 8,
                4, 6, 2,
            }
        },
        { // 9
            new int[]
            {
                1, 5, 6, 8
            },
            new int[]
            {
                4, 7, 11,
                4, 7, 1,
                6, 7, 1,
                6, 10, 1,
            }
        },
        { // 10
            new int[]
            {
                1, 4, 6, 7
            },
            new int[]
            {
                3, 9, 11,
                3, 9, 1,
                5, 7, 12,
                5, 10, 12
            }
        },
        { // 11
            new int[]
            {
                1, 5, 6, 7
            },
            new int[]
            {
                1, 4, 8,
                1, 12, 8,
                7, 12, 8,
                1, 12, 10,
            }
        },
        { // 12
            new int[]
            {
                2, 4, 5, 6
            },
            new int[]
            {
                3, 4, 11,
                1, 2, 9,
                8, 2, 9,
                8, 2, 6,
            }
        },
        { // 13
            new int[]
            {
                1, 3, 6, 8
            },
            new int[]
            {
                1, 4, 9,
                2, 3, 12,
                5, 6, 10,
                7, 8, 11,
            }
        },
        { // 14
            new int[]
            {
                2, 5, 6, 8
            },
            new int[]
            {
                1, 11, 9,
                1, 11, 6,
                1, 2, 6,
                7, 11, 6,
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
        
        if (chunk == null)
        {
            Debug.LogError("_loadedChunks.Length = " + _loadedChunks.Count);
            //Debug.LogError("Generate Mesh called on null chunk position");
            return null;
        }

        Debug.Log("in GenerateMesh(), " + chunk.ToString());

        for (int x = 0; x < Chunk.ChunkSize.x - 1; x++)
        {
            for (int y = 0; y < Chunk.ChunkSize.y - 1; y++)
            {
                for (int z = 0; z < Chunk.ChunkSize.z - 1; z++)
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
                    foreach (bool b in cube)
                    {
                        if (b) numTrue++;
                    }
                    if (numTrue != 0 && numTrue != 8)
                    {
                        Debug.Log("Got a cube to check out: "+x+", "+y+", "+z);
                        int[] triangles = getTriangles(cube);
                        if (triangles != null)
                        {
                            Debug.Log(triangles.ToString());
                        }
                    }
                }
            }
        }
        
        return mesh;
    }

    private static void addToMesh(Mesh mesh, int x, int y, int z, int meshIndex)
    {
        Vector3[] vertices = mesh.vertices;
        //Vector2[] uv = mesh.uv;
        int[] triangles = mesh.triangles;

        // Transform and add new vertices

        // Add triangles according to indexes of new vertices
        
        // Reassign to mesh
        mesh.vertices = vertices;
        //mesh.uv = uv;
        mesh.triangles = triangles;
    }

    //TODO return rotation reference along with triangles, or rotated vertices
    private static int[] getTriangles(bool[] cube)
    {
        bool matchTrue = true;
        int numTrue = 0;
        int[] matchVertices;
        int matchVerticesInsertIndex;

        Debug.Log("In getTriangles() " + cube.ToString());

        // Count number of true elements. If more than 4, match inverse
        foreach (bool b in cube)
        {
            if (b)
            {
                numTrue++;
            }
        }
        
        if (numTrue > 4)
        {
            matchTrue = false;
            matchVertices = new int[8 - numTrue];
        }
        else
        {
            matchVertices = new int[numTrue];
        }

        // Populate array of vertex indices
        matchVerticesInsertIndex = 0;
        for (int i = 0; i < cube.Length; i++)
        {
            if(cube[i] == matchTrue)
            {
                matchVertices[matchVerticesInsertIndex] = i;
                matchVerticesInsertIndex++;
            }
        }

        // Iterate through 14 templates * 24 possible rotations to find match
        int[] triangles = null;
        for (int rx = 0; rx < 4; rx++)
        {
            for (int templateIndex = 1; templateIndex < VerticesToTriangles.Count; templateIndex++)
            {
                // Iterate through 16 rotations in x-y rotations
                for (int ry = 0; ry < 4; ry++)
                {
                    VerticesToTriangles.TryGetValue(matchVertices, out triangles);

                    if (triangles != null)
                    {
                        return triangles;
                    }

                    rotateVertices(matchVertices, 1, RotationIndex['y']);
                }

                // Iterate through remaining 8 rotations in x-z rotations 
                for (int rz = 1; rz < 3; rz += 2)
                {
                    VerticesToTriangles.TryGetValue(matchVertices, out triangles);

                    if (triangles != null)
                    {
                        return triangles;
                    }

                    rotateVertices(matchVertices, rz, RotationIndex['z']);
                }
                rotateVertices(matchVertices, 1, RotationIndex['z']); // Rotate back to original position in z-axis
            }
            rotateVertices(matchVertices, 1, RotationIndex['x']);
        }

        // Should never fail
        Debug.LogError("Failed to match vertices");
        return null;
    }
    
    private static void rotateVertices(int[] vertices, int rotateTimes, int[][] rotationIndex)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            for (int r = 0; r < rotateTimes; r++)
            {
                int indexOf = System.Array.IndexOf(rotationIndex[0], vertices[i]);
                if (indexOf != -1)
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
}
