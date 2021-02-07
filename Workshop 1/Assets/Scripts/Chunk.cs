using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

public class Chunk
{
    public Material cubeMaterial;
    public Block[,,] chunkData;
    public GameObject chunk;

    public Chunk(Vector3 position, Material c)
    {
        chunk = new GameObject(World.BuildChunkName(position));
        chunk.transform.position = position;
        cubeMaterial = c;
        BuildChunk();
    }

    void BuildChunk()
    {
        chunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];
        Vector3 chunkTransformPosition = chunk.transform.position;

        //create blocks
        for (int z = 0; z < World.chunkSize; z++)
            for (int y = 0; y < World.chunkSize; y++)
                for (int x = 0; x < World.chunkSize; x++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    int worldX = (int)(x + chunkTransformPosition.x);
                    int worldY = (int)(y + chunkTransformPosition.y);
                    int worldZ = (int)(z + chunkTransformPosition.z);

                    int stoneHeight = Utils.GenerateStoneHeight(worldX, worldZ, World.maxHeight, World.smooth, World.octaves, World.persistance);
                    int surfaceHeight = Utils.GenerateHeight(worldX, worldZ, World.maxHeight, World.smooth, World.octaves, World.persistance);

                    if (worldY == 0)
                        chunkData[x, y, z] = new Block(Block.BlockType.BEDROCK, pos,
                                        chunk.gameObject, this);
                    else if (Utils.fBM3D(worldX, worldY, worldZ, 0.1f, 3) < 0.42f && worldY > 1 && worldY <= stoneHeight)
                        chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos,
                                        chunk.gameObject, this);
                    else if (worldY <= stoneHeight)
                    {
                        if (Utils.fBM3D(worldX, worldY, worldZ, 0.01f, 2) < 0.4f && worldY <= 12 && worldY >= 5 && UnityEngine.Random.Range(0, 100) < 1)
                            chunkData[x, y, z] = new Block(Block.BlockType.DIAMOND, pos,
                                        chunk.gameObject, this);
                        else if (Utils.fBM3D(worldX, worldY, worldZ, 0.03f, 3) < 0.41f && worldY < 15 && UnityEngine.Random.Range(0, 100) < 8)
                            chunkData[x, y, z] = new Block(Block.BlockType.REDSTONE, pos,
                                        chunk.gameObject, this);
                        else
                            chunkData[x, y, z] = new Block(Block.BlockType.STONE, pos,
                                        chunk.gameObject, this);
                    }
                    else if (worldY < surfaceHeight)
                        chunkData[x, y, z] = new Block(Block.BlockType.DIRT, pos,
                                        chunk.gameObject, this);
                    else if (worldY == surfaceHeight)
                        chunkData[x, y, z] = new Block(Block.BlockType.GRASS, pos,
                                        chunk.gameObject, this);
                    else
                        chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos,
                                        chunk.gameObject, this);
                }
    }

    public void DrawChunk()
    {
        for (int z = 0; z < World.chunkSize; z++)
            for (int y = 0; y < World.chunkSize; y++)
                for (int x = 0; x < World.chunkSize; x++)
                {
                    chunkData[x, y, z].Draw();
                }

        CombineQuads();
        MeshCollider collider = chunk.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        collider.sharedMesh = chunk.transform.GetComponent<MeshFilter>().mesh;
    }


    void CombineQuads()
    {

        //1. Combine all children meshes
        MeshFilter[] meshFilters = chunk.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        //2. Create a new mesh on the parent object
        MeshFilter mf = (MeshFilter)chunk.gameObject.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();

        //3. Add combined meshes on children as the parent's mesh
        mf.mesh.CombineMeshes(combine);

        //4. Create a renderer for the parent
        MeshRenderer renderer = chunk.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        renderer.material = cubeMaterial;

        //5. Delete all uncombined children
        foreach (Transform quad in chunk.transform)
        {
            GameObject.Destroy(quad.gameObject);
        }
    }
}
