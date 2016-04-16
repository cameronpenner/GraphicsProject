using System.Collections.Generic;
using UnityEngine;

internal class MeshGenerator
{
	private static Dictionary<Vector3, Chunk> _loadedChunks;
    private static Mesh[] meshArray;

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

    private static readonly List<int[]> BaseTriangles = new List<int[]>
    {
        new int[] //0
        {
        },
        new int[] //1
        {
            1, 4, 9
        },
        new int[] //2
        {
            2, 4, 9,
            2, 9, 10
        },
        new int[] //3
        {
            1, 4, 9,
            2, 3, 12
        },
        new int[] //4
        {
            1, 4, 9,
            6, 7, 12
        },
        new int[] //5
        {
            1, 2, 9,
            2, 8, 9,
            2, 6, 8
        },
        new int[] //6
        {
            2, 4, 9,
            2, 9, 10,
            6, 7, 12
        },
        new int[] //7
        {
            3, 4, 11,
            1, 2, 10,
            6, 7, 12
        },
        new int[] //8
        {
            4, 6, 8,
            4, 6, 2,
        },
        new int[] //9
        {
            4, 7, 11,
            4, 7, 1,
            6, 7, 1,
            6, 10, 1,
        },
        new int[] //10
        {
            3, 9, 11,
            3, 9, 1,
            5, 7, 12,
            5, 10, 12,
        },
        new int[] //11
        {
            1, 4, 8,
            1, 12, 8,
            7, 12, 8,
            1, 12, 10,
        },
        new int[] //12
        {
            3, 4, 11,
            1, 2, 9,
            8, 2, 9,
            8, 2, 6,
        },
        new int[] //13
        {
            1, 4, 9,
            2, 3, 12,
            5, 6, 10,
            7, 8, 11,
        },
        new int[] //14
        {
            1, 11, 9,
            1, 11, 6,
            1, 2, 6,
            7, 11, 6,
        },
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
            Debug.LogError("Generate Mesh called on null chunk position");
            return null;
        }

        for (int x = 0; x < Chunk.ChunkSize.x - 1; x++)
        {
            for (int y = 0; y < Chunk.ChunkSize.y - 1; y++)
            {
                for (int z = 0; z < Chunk.ChunkSize.z - 1; z++)
                {
                    if (chunk.VoxelAt(x, y, z))
                    {
                        int meshIndex = 0;
                        meshIndex += chunk.VoxelAt(x, y, z) ? 1 : 0;
                        meshIndex += chunk.VoxelAt(x, y, z + 1) ? 2 : 0;
                        meshIndex += chunk.VoxelAt(x, y + 1, z) ? 4 : 0;
                        meshIndex += chunk.VoxelAt(x, y + 1, z + 1) ? 8 : 0;
                        meshIndex += chunk.VoxelAt(x + 1, y, z) ? 16 : 0;
                        meshIndex += chunk.VoxelAt(x + 1, y, z + 1) ? 32 : 0;
                        meshIndex += chunk.VoxelAt(x + 1, y + 1, z) ? 64 : 0;
                        meshIndex += chunk.VoxelAt(x + 1, y + 1, z + 1) ? 128 : 0;

                        if (meshIndex != 0 && meshIndex != 255)
                        {
                            addToMesh(mesh, x, y, z, meshIndex);
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
}
