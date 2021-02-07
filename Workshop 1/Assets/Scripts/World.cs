using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class World : MonoBehaviour
{
    public Material textureAtlas;

    public int MaxHeight = 120;
    public float Smooth = 0.01f;
    public int Octaves = 4;
    public float Persistance = 0.5f;

    public static int maxHeight = 120;
    public static float smooth = 0.01f;
    public static int octaves = 4;
    public static float persistance = 0.5f;

    public bool RegenerateWorld = false;

    public static int columnHeight = 16;
    public static int chunkSize = 16;
    public static int worldSize = 4;
    public static ConcurrentDictionary<string, Chunk> chunks;

    void Start()
    {
        maxHeight = MaxHeight;
        smooth = Smooth;
        octaves = Octaves;
        persistance = Persistance;

        chunks = new ConcurrentDictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        StartCoroutine(BuildWorld());
    }

    private void Update()
    {
        if (RegenerateWorld)
        {
            RegenerateWorld = false;
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            Start();
        }
    }

    public static string BuildChunkName(Vector3 v)
    {
        return (int)v.x + "_" + (int)v.y + "_" + (int)v.z;
    }

    IEnumerator BuildWorld()
    {
        for (int z = 0; z < worldSize; z++)
            for (int x = 0; x < worldSize; x++)
                for (int y = 0; y < columnHeight; y++)
                {
                    Vector3 chunkPosition = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);
                    Chunk c = new Chunk(chunkPosition, textureAtlas);
                    c.chunk.transform.parent = transform;
                    chunks.TryAdd(c.chunk.name, c);

                    yield return null;
                }

        foreach (KeyValuePair<string, Chunk> c in chunks)
        {
            c.Value.DrawChunk();
            yield return null;
        }

    }
}

