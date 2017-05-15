using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMap : MonoBehaviour {

    public float size = 2f;

    public  int voxelResolution = 4;
    public  int chunkResolution = 2;

    public int cellConfig;

    public Material meshMaterial;

    private BrushController brush;

    public VoxelChunk voxelChunkPrefab;

    private VoxelChunk[] chunks;

    private float chunkSize, voxelSize, halfSize;

    private void Awake()
    {
        halfSize = size * 0.5f;
        chunkSize = size / chunkResolution;
        voxelSize = chunkSize / voxelResolution;
        
        chunks = new VoxelChunk[chunkResolution * chunkResolution * chunkResolution];

        for (int i = 0, z = 0; z < chunkResolution; z++)
        {
            for (int y = 0; y < chunkResolution; y++)
            {
                for (int x = 0; x < chunkResolution; x++, i++)
                {
                    createChunk(i, x, y, z);
                }
            }
        }
    }

    private void Start()
    {
        brush = FindObjectOfType<BrushController>();
        brush.onVoxelCollided += editVoxels;
    }

    private void editVoxels(Vector3 point, BrushController.BrushMode mode)
    {

        int voxelX = (int)((point.x + halfSize) / voxelSize);
        int voxelY = (int)((point.y + halfSize) / voxelSize);
        int voxelZ = (int)((point.z + halfSize) / voxelSize);

        int chunkX = voxelX / voxelResolution;
        int chunkY = voxelY / voxelResolution;
        int chunkZ = voxelZ / voxelResolution;

        voxelX -= chunkX * voxelResolution;
        voxelY -= chunkY * voxelResolution;
        voxelZ -= chunkZ * voxelResolution;
        Debug.Log(voxelX + ", " + voxelY + ", " + voxelZ + ", ");
        bool state = mode == BrushController.BrushMode.Filled ? true : false;

        if (chunks[chunkZ * chunkResolution * chunkResolution + chunkY * chunkResolution + chunkX].voxels[voxelZ * voxelResolution * voxelResolution + voxelY * voxelResolution + voxelX].state != state)
        {
            chunks[chunkZ * chunkResolution * chunkResolution + chunkY * chunkResolution + chunkX].setVoxel(voxelX, voxelY, voxelZ, state);
        }
    }

    private void createChunk(int i, int x, int y, int z)
    {
        VoxelChunk chunk = Instantiate(voxelChunkPrefab) as VoxelChunk;

        chunk.initialize(voxelResolution, chunkSize, meshMaterial, cellConfig);
        chunk.transform.parent = transform;
        chunk.transform.localPosition = new Vector3(x * chunkSize - halfSize, y * chunkSize - halfSize, z * chunkSize - halfSize );
        chunks[i] = chunk;//a

        if (x > 0)
        {
            chunks[i - 1].xNeighbour = chunk;//b
        }
        if(y > 0)
        {
            chunks[i - chunkResolution].yNeighbour = chunk;//c
            if (x > 0)
            {
                chunks[i - chunkResolution - 1].xyNeighbour = chunk;//d
                if(z > 0)
                {
                    chunks[i - chunkResolution * chunkResolution - chunkResolution - 1].xyzNeighbour = chunk;//h
                }
            }
            if(z > 0)
            {
                chunks[i - chunkResolution * chunkResolution - chunkResolution].yzNeighbour = chunk;
            }
        }
    }
}
