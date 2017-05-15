  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[SelectionBase]
public class VoxelChunk : MonoBehaviour {

    public int resolution;
    public GameObject voxelPrefab;
    public Voxel[] voxels;
    public int  checkCellType;

    private Material[] voxelMaterials;
    private Material meshMaterial;

    public float voxelSize;
    public  float halfSize;

    public VoxelChunk xNeighbour, yNeighbour, zNeighbour, xyNeighbour, yzNeighbour, xzNeighbour, xyzNeighbour;

    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> triangles;

    private MeshRenderer meshRenderer;

    public GameObject cubeprefab;

    public void initialize(int _resolution, float size, Material meshMaterial, int cellConfig)
    {
        checkCellType = cellConfig;
        resolution = _resolution;
        voxelSize = size / resolution;
        this.meshMaterial = meshMaterial;
        halfSize = size / 2;
        voxels = new Voxel[resolution * resolution * resolution];
        voxelMaterials = new Material[voxels.Length];

        for (int i = 0, z = 0; z < resolution; z++)
        {
            for(int y = 0; y < resolution; y++)
            {
                for(int x = 0; x < resolution; x++, i++)
                {
                    createVoxel(i, x, y, z);
                }
            }
        }

        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = meshMaterial;
        mesh.name = "VoxelGrid Mesh";
       
        vertices = new List<Vector3>();
        triangles = new List<int>();
        refresh();
    }

    private void createVoxel(int i, int x, int y, int z)
    {
        GameObject o = Instantiate(voxelPrefab) as GameObject;
        o.transform.parent = transform;
        o.transform.localPosition = new Vector3((x + 0.5f) * voxelSize, (y + 0.5f) * voxelSize, (z + 0.5f) * voxelSize);
        o.transform.localScale = Vector3.one * voxelSize*0.03f;
        voxelMaterials[i] = o.GetComponent<MeshRenderer>().material;
        voxels[z * resolution * resolution + y * resolution + x] = new Voxel(x, y, z, voxelSize);
    }

    public void setVoxel(int x, int y, int z, bool state)
    {
        voxels[z * resolution * resolution + y * resolution + x].state = state;
        refresh();
    }

    private void setVoxelColors()
    {
        for(int i = 0; i < voxels.Length; ++i)
        {
            voxelMaterials[i].color = voxels[i].state ? Color.red : Color.white;
        }
    }

    private void refresh()
    {
        setVoxelColors();
        triangulate();
    }

    private void triangulate()
    {
        vertices.Clear();
        triangles.Clear();
        mesh.Clear();

        triangulateCell();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        
        
    }

    private void triangulateCell()
    {
        
        int cells = resolution-1;
        //Debug.Log("cells " + cells);
        //0, 
        for (int i = 0, z = 0; z < cells; z++, i+=resolution)
        {
            for (int y = 0; y < cells; y++, i++)
            {
                for (int x = 0; x < cells; x++, i++)
                {
                    /*Debug.Log("cell" + i + " with voxels: "
                        + i + ", " + (i+1) + ", " + (i + resolution) + ", " + (i + resolution + 1) + 
                        ", " + (i + resolution*resolution) + ", " + (i + resolution*resolution + 1) + ", "
                        + (i + resolution* resolution + resolution) + ", " + (i+resolution*resolution+resolution+1));
                    */
                    lookupCell(
                        voxels[i],
                        voxels[i + 1],
                        voxels[i + resolution],
                        voxels[i + resolution + 1],
                        voxels[i + resolution * resolution],
                        voxels[i + resolution * resolution + 1],
                        voxels[i + resolution * resolution + resolution],
                        voxels[i + resolution * resolution + resolution + 1]
                        );
                }
            }
        }
    }

    private void lookupCell(Voxel a, Voxel b, Voxel c, Voxel d, Voxel e, Voxel f, Voxel g, Voxel h)
    {
       /* cubeprefab = GameObject.FindGameObjectWithTag("Sphere");
        GameObject[] visualizationCubes = new GameObject[8];
        Voxel[] voxelArgs = { a, b, c, d, e, f, g, h };
        Color[] colors = { Color.black, Color.white, Color.red, Color.yellow, Color.green, Color.cyan, Color.magenta, Color.grey };
        for(int i = 0; i < 8; ++i)
        {
            visualizationCubes[i] = Instantiate(cubeprefab);
            visualizationCubes[i].transform.localScale = Vector3.one *0.1f;
            visualizationCubes[i].transform.position = voxelArgs[i].position;
            //visualizationCubes[i].transform.position = new Vector3(voxelArgs[i].position.x, voxelArgs[i].position.z, voxelArgs[i].position.y);
            visualizationCubes[i].GetComponent<MeshRenderer>().material.color = colors[i];
        }*/

        int cellType = 0;
        if (a.state)
        {
            cellType |= 1;
        }
        if (b.state)
        {
            cellType |= 2;
        }
        if (c.state)
        {
            cellType |= 4;
        }
        if (d.state)
        {
            cellType |= 8;
        }
        if (e.state)
        {
            cellType |= 16;
        }
        if (f.state)
        {
            cellType |= 32;
        }
        if (g.state)
        {
            cellType |= 64;
        }
        if (h.state)
        {
            cellType |= 128;
        }
        //Debug.Log("Cell Type = " + cellType);
        //cellType = 20;
        switch (cellType)
        {
            case 0:
                return;
                // one vertex configurations
            case 1:
                addTriangle(a.xEdge, a.yEdge, a.zEdge); 
                break;
            case 2:
                addTriangle(a.xEdge, b.zEdge, b.yEdge);
                break;
                //xm_stranno
            case 4:
                
                addTriangle(a.yEdge, c.xEdge, c.zEdge);
                break;
            case 8:
                addTriangle(c.xEdge, b.yEdge, d.zEdge);
                break;
                //
            case 16:
               
                addTriangle(a.zEdge, e.yEdge, e.xEdge);
                break;
            case 32:
                
                addTriangle(e.xEdge, f.yEdge, b.zEdge);
                break;
            case 64:
               
                addTriangle(e.yEdge, c.zEdge, g.xEdge);
                break;
            case 128:
                
                addTriangle(f.yEdge, g.xEdge, d.zEdge);
                break;
            // two vertex configurations

            case 3:
                addTriangle(b.zEdge, a.yEdge, a.zEdge);
                addTriangle(a.yEdge, b.zEdge, b.yEdge);
                break;
            case 5:
                addTriangle(a.xEdge, c.zEdge, a.zEdge);
                addTriangle(c.xEdge, c.zEdge, a.xEdge);
                break;
            case 6:
                addTriangle(a.xEdge, b.zEdge, b.yEdge);
                addTriangle(a.yEdge, c.xEdge, c.zEdge);
                break;
            case 9:
                addTriangle(a.xEdge, a.yEdge, a.zEdge);
                addTriangle(c.xEdge, b.yEdge, d.zEdge);
                break;
            case 10:
                addTriangle(a.xEdge, d.zEdge, c.xEdge);
                addTriangle(b.zEdge, d.zEdge, a.xEdge);
                break;
            case 12:
                addTriangle(a.yEdge, b.yEdge, d.zEdge);
                addTriangle(d.zEdge, c.zEdge, a.yEdge);
                break;
            case 17:
                addTriangle(a.xEdge, e.yEdge, e.xEdge);
                addTriangle(e.yEdge, a.xEdge, a.yEdge);
                break;
            case 18:
                addTriangle(a.xEdge, b.zEdge, b.yEdge);
                addTriangle(a.zEdge, e.yEdge, e.xEdge);
                break;
            case 20:
                addTriangle(a.yEdge, c.xEdge, c.zEdge);
                addTriangle(a.zEdge, e.yEdge, e.xEdge);
                break;
            case 24:
                addTriangle(c.xEdge, b.yEdge, d.zEdge);
                addTriangle(a.zEdge, e.yEdge, e.xEdge);
                break;
            case 33:
                addTriangle(a.xEdge, a.yEdge, a.zEdge);
                addTriangle(e.xEdge, f.yEdge, b.zEdge);
                break;
            case 34:
                addTriangle(e.xEdge, b.yEdge, a.xEdge);
                addTriangle(b.yEdge, e.xEdge, f.yEdge);
                break;
            case 36:
                addTriangle(a.yEdge, c.xEdge, c.zEdge);
                addTriangle(e.xEdge, f.yEdge, b.zEdge);
                break;
            case 40:
                addTriangle(c.xEdge, b.yEdge, d.zEdge);
                addTriangle(e.xEdge, f.yEdge, b.zEdge);
                break;
            case 48:
                addTriangle(a.zEdge, e.yEdge, b.zEdge);
                addTriangle(e.yEdge, f.yEdge, b.zEdge);
                break;
            case 65:
                addTriangle(a.xEdge, a.yEdge, a.zEdge);
                addTriangle(e.yEdge, c.zEdge, g.xEdge);
                break;
            case 66:
                addTriangle(a.xEdge, b.zEdge, b.yEdge);
                addTriangle(e.yEdge, c.zEdge, g.xEdge);
                break;
            case 68:
                addTriangle(c.xEdge, g.xEdge, a.yEdge);
                addTriangle(e.yEdge, a.yEdge, g.xEdge);
                break;
            case 72:
                addTriangle(c.xEdge, b.yEdge, d.zEdge);
                addTriangle(e.yEdge, c.zEdge, g.xEdge);
                break;
            case 80:
                addTriangle(a.zEdge, g.xEdge, e.xEdge);
                addTriangle(g.xEdge, a.zEdge, c.zEdge);
                break;
            case 96:
                addTriangle(e.xEdge, f.yEdge, b.zEdge);
                addTriangle(e.yEdge, c.zEdge, g.xEdge);
                break;
            case 129:
                addTriangle(a.xEdge, a.yEdge, a.zEdge);
                addTriangle(f.yEdge, g.xEdge, d.zEdge);
                break;
            case 130:
                addTriangle(a.xEdge, b.zEdge, b.yEdge);
                addTriangle(f.yEdge, g.xEdge, d.zEdge);
                break;
            case 132:
                addTriangle(a.yEdge, c.xEdge, c.zEdge);
                addTriangle(f.yEdge, g.xEdge, d.zEdge);
                break;
            case 136:
                addTriangle(g.xEdge, c.xEdge, b.yEdge);
                addTriangle(b.yEdge, f.yEdge, g.xEdge);
                break;
            case 144:
                addTriangle(a.zEdge, e.yEdge, e.xEdge);
                addTriangle(f.yEdge, g.xEdge, d.zEdge);
                break;
            case 160:
                addTriangle(g.xEdge, b.zEdge, e.xEdge);
                addTriangle(b.zEdge, g.xEdge, d.zEdge);
                break;
            case 192:
                addTriangle(e.yEdge, c.zEdge, f.yEdge);
                addTriangle(c.zEdge, d.zEdge, f.yEdge);
                break;
           //three vertices
            case 7:
                addTriangle(c.zEdge, a.zEdge, b.zEdge);
                addTriangle(c.zEdge, b.zEdge, b.yEdge);
                addTriangle(c.xEdge, c.zEdge, b.yEdge);
                break;
            case 11:
                addTriangle(b.zEdge, d.zEdge, a.zEdge);
                addTriangle(a.zEdge, d.zEdge, a.yEdge);
                addTriangle(a.yEdge, d.zEdge, c.xEdge);
                break;
            case 13:
                addTriangle(d.zEdge, a.zEdge, a.xEdge);
                addTriangle(c.zEdge, a.zEdge, d.zEdge);
                addTriangle(a.xEdge, b.yEdge, d.zEdge);
                break;
            case 14:
                addTriangle(b.zEdge, d.zEdge, c.zEdge);
                addTriangle(c.zEdge, a.xEdge, b.zEdge);
                addTriangle(c.zEdge, a.yEdge, a.xEdge);
                break;
            case 19:
                addTriangle(a.yEdge, e.yEdge, b.yEdge);
                addTriangle(e.yEdge, e.xEdge, b.zEdge);
                addTriangle(b.yEdge, e.yEdge, b.zEdge);
                break;
            case 21:
                addTriangle(a.xEdge, c.xEdge, e.xEdge);
                addTriangle(c.xEdge, c.zEdge, e.xEdge);
                addTriangle(c.zEdge, e.yEdge, e.xEdge);
                break;
            case 22:
                addTriangle(a.xEdge, b.zEdge, b.yEdge);
                addTriangle(a.yEdge, c.xEdge, c.zEdge);
                addTriangle(a.zEdge, e.yEdge, e.xEdge);
                break;
            case 25:
                addTriangle(a.xEdge, e.yEdge, e.xEdge);
                addTriangle(e.yEdge, a.xEdge, a.yEdge);
                addTriangle(c.xEdge, b.yEdge, d.zEdge);
                break;
            case 26:
                addTriangle(a.xEdge, d.zEdge, c.xEdge);
                addTriangle(b.zEdge, d.zEdge, a.xEdge);
                addTriangle(a.zEdge, e.yEdge, e.xEdge);
                break;
            case 28:
                addTriangle(a.yEdge, b.yEdge, d.zEdge);
                addTriangle(d.zEdge, c.zEdge, a.yEdge);
                addTriangle(a.zEdge, e.yEdge, e.xEdge);
                break;
            case 35:
                addTriangle(a.yEdge, f.yEdge, b.yEdge);
                addTriangle(a.yEdge, a.zEdge, f.yEdge);
                addTriangle(e.xEdge, f.yEdge, a.zEdge);
                break;
            case 37:
                addTriangle(a.xEdge, c.zEdge, a.zEdge);
                addTriangle(c.xEdge, c.zEdge, a.xEdge);
                addTriangle(e.xEdge, f.yEdge, b.zEdge);
                break;
            case 38:
                addTriangle(e.xEdge, b.yEdge, a.xEdge);
                addTriangle(b.yEdge, e.xEdge, f.yEdge);
                addTriangle(a.yEdge, c.xEdge, c.zEdge);
                break;
            case 41:
                addTriangle(a.xEdge, a.yEdge, a.zEdge);
                addTriangle(c.xEdge, b.yEdge, d.zEdge);
                addTriangle(e.xEdge, f.yEdge, b.zEdge);
                break;
            case 42:
                addTriangle(a.xEdge, e.xEdge, c.xEdge);
                addTriangle(c.xEdge, e.xEdge, f.yEdge);
                addTriangle(f.yEdge, d.zEdge, c.xEdge);
                break;
            case 44:
                addTriangle(a.yEdge, b.yEdge, d.zEdge);
                addTriangle(d.zEdge, c.zEdge, a.yEdge);
                addTriangle(e.xEdge, f.yEdge, b.zEdge);
                break;
            case 49:
                addTriangle(a.yEdge, e.yEdge, f.yEdge);
                addTriangle(a.xEdge, a.yEdge, f.yEdge);
                addTriangle(a.xEdge, f.yEdge, b.zEdge);
                break;
            case 50:
                addTriangle(b.yEdge, e.yEdge, f.yEdge);
                addTriangle(e.yEdge, b.yEdge, a.xEdge);
                addTriangle(a.xEdge, a.zEdge, e.yEdge);
                break;
            case 52:
                addTriangle(a.zEdge, e.yEdge, b.zEdge);
                addTriangle(e.yEdge, f.yEdge, b.zEdge);
                addTriangle(a.yEdge, c.xEdge, c.zEdge);
                break;
            case 56:
                addTriangle(a.zEdge, e.yEdge, b.zEdge);
                addTriangle(e.yEdge, f.yEdge, b.zEdge);
                addTriangle(c.xEdge, b.yEdge, d.zEdge);
                break;
            case 67:
                addTriangle(b.zEdge, a.yEdge, a.zEdge);
                addTriangle(a.yEdge, b.zEdge, b.yEdge);
                addTriangle(e.yEdge, c.zEdge, g.xEdge);
                break;
            case 69:
                addTriangle(a.xEdge, c.xEdge, g.xEdge);
                addTriangle(a.xEdge, g.xEdge, a.zEdge);
                addTriangle(a.zEdge, g.xEdge, e.yEdge);
                break;
            case 70:
                addTriangle(c.xEdge, g.xEdge, a.yEdge);
                addTriangle(e.yEdge, a.yEdge, g.xEdge);
                addTriangle(a.xEdge, b.zEdge, b.yEdge);
                break;
            case 73:
                addTriangle(a.xEdge, a.yEdge, a.zEdge);
                addTriangle(c.xEdge, b.yEdge, d.zEdge);
                addTriangle(e.yEdge, c.zEdge, g.xEdge);
                break;
            case 74:
                addTriangle(a.xEdge, d.zEdge, c.xEdge);
                addTriangle(b.zEdge, d.zEdge, a.xEdge);
                addTriangle(e.yEdge, c.zEdge, g.xEdge);
                break;
            case 76:
                addTriangle(a.yEdge, b.yEdge, e.yEdge);
                addTriangle(d.zEdge, g.xEdge, b.yEdge);
                addTriangle(e.yEdge, b.yEdge, g.xEdge);
                break;
            case 81:
                addTriangle(a.xEdge, g.xEdge, e.xEdge);
                addTriangle(a.xEdge, a.yEdge, g.xEdge);
                addTriangle(a.yEdge, c.zEdge, g.xEdge);
                break;
            case 82:
                addTriangle(a.zEdge, g.xEdge, e.xEdge);
                addTriangle(g.xEdge, a.zEdge, c.zEdge);
                addTriangle(a.xEdge, b.zEdge, b.yEdge);
                break;
            case 84:
                addTriangle(c.xEdge, g.xEdge, e.xEdge);
                addTriangle(a.yEdge, c.xEdge, a.zEdge);
                addTriangle(a.zEdge, c.xEdge, e.xEdge);
                break;
            case 88:
                addTriangle(a.zEdge, g.xEdge, e.xEdge);
                addTriangle(g.xEdge, a.zEdge, c.zEdge);
                addTriangle(c.xEdge, b.yEdge, d.zEdge);
                break;
            case 97:
                addTriangle(a.xEdge, a.yEdge, a.zEdge);
                addTriangle(e.xEdge, f.yEdge, b.zEdge);
                addTriangle(e.yEdge, c.zEdge, g.xEdge);
                break;
            case 98:
                addTriangle(e.xEdge, b.yEdge, a.xEdge);
                addTriangle(b.yEdge, e.xEdge, f.yEdge);
                addTriangle(e.yEdge, c.zEdge, g.xEdge);
                break;
            case 100:
                addTriangle(c.xEdge, g.xEdge, a.yEdge);
                addTriangle(e.yEdge, a.yEdge, g.xEdge);
                addTriangle(e.xEdge, f.yEdge, b.zEdge);
                break;
            case 104:
                addTriangle(c.xEdge, b.yEdge, d.zEdge);
                addTriangle(e.xEdge, f.yEdge, b.zEdge);
                addTriangle(e.yEdge, c.zEdge, g.xEdge);
                break;
            case 112:
                addTriangle(a.zEdge, c.zEdge, b.zEdge);
                addTriangle(c.zEdge, g.xEdge, f.yEdge);
                addTriangle(c.zEdge, f.yEdge, b.zEdge);
                break;
            case 131:                           
                addTriangle(b.zEdge, a.yEdge, a.zEdge);
                addTriangle(a.yEdge, b.zEdge, b.yEdge);
                addTriangle(f.yEdge, g.xEdge, d.zEdge); //128
                break;
            case 133:
                addTriangle(a.xEdge, c.zEdge, a.zEdge);
                addTriangle(c.xEdge, c.zEdge, a.xEdge);
                addTriangle(f.yEdge, g.xEdge, d.zEdge);
                break;
            case 134:
                addTriangle(a.xEdge, b.zEdge, b.yEdge);               
                addTriangle(a.yEdge, c.xEdge, c.zEdge);
                addTriangle(f.yEdge, g.xEdge, d.zEdge);
                break;
            case 137:
                addTriangle(g.xEdge, c.xEdge, b.yEdge);
                addTriangle(b.yEdge, f.yEdge, g.xEdge);
                addTriangle(a.xEdge, a.yEdge, a.zEdge);
                break;
            case 138:
                addTriangle(a.xEdge, g.xEdge, c.xEdge);
                addTriangle(b.zEdge, f.yEdge, g.xEdge);
                addTriangle(a.xEdge, b.zEdge, g.xEdge);
                break;
            case 140:
                addTriangle(a.yEdge, b.yEdge, f.yEdge);
                addTriangle(c.zEdge, a.yEdge, g.xEdge);
                addTriangle(a.yEdge, f.yEdge, g.xEdge);
                break;
            case 145:
                addTriangle(a.xEdge, e.yEdge, e.xEdge);
                addTriangle(e.yEdge, a.xEdge, a.yEdge);
                addTriangle(f.yEdge, g.xEdge, d.zEdge);
                break;
            case 146:
                addTriangle(a.xEdge, b.zEdge, b.yEdge);
                addTriangle(a.zEdge, e.yEdge, e.xEdge);
                addTriangle(f.yEdge, g.xEdge, d.zEdge);
                break;
            case 148:
                addTriangle(a.yEdge, c.xEdge, c.zEdge);
                addTriangle(a.zEdge, e.yEdge, e.xEdge);
                addTriangle(f.yEdge, g.xEdge, d.zEdge);
                break;
            case 152:
                addTriangle(g.xEdge, c.xEdge, b.yEdge);
                addTriangle(b.yEdge, f.yEdge, g.xEdge);
                addTriangle(a.zEdge, e.yEdge, e.xEdge);//16
                break;
            case 161:
                addTriangle(g.xEdge, b.zEdge, e.xEdge);
                addTriangle(b.zEdge, g.xEdge, d.zEdge);
                addTriangle(a.xEdge, a.yEdge, a.zEdge);//1
                break;
            case 162:
                addTriangle(a.xEdge, e.xEdge, g.xEdge);
                addTriangle(d.zEdge, b.yEdge, a.xEdge);
                addTriangle(a.xEdge, g.xEdge, d.zEdge);
                break;
            case 164:
                addTriangle(g.xEdge, b.zEdge, e.xEdge);
                addTriangle(b.zEdge, g.xEdge, d.zEdge);
                addTriangle(a.yEdge, c.xEdge, c.zEdge);//4
                break;
            case 168:
                addTriangle(c.xEdge, e.xEdge, g.xEdge);
                addTriangle(b.zEdge, c.xEdge, b.yEdge);
                addTriangle(e.xEdge, c.xEdge, b.zEdge);
                break;
            case 176:
                addTriangle(a.zEdge, d.zEdge, b.zEdge);
                addTriangle(e.yEdge, g.xEdge, d.zEdge);
                addTriangle(a.zEdge, e.yEdge, d.zEdge);
                break;
            case 193:                
                addTriangle(e.yEdge, c.zEdge, f.yEdge);//192->
                addTriangle(c.zEdge, d.zEdge, f.yEdge);//192->
                addTriangle(a.xEdge, a.yEdge, a.zEdge);
                break;
            case 194:
                addTriangle(e.yEdge, c.zEdge, f.yEdge);//192->
                addTriangle(c.zEdge, d.zEdge, f.yEdge);
                addTriangle(a.xEdge, b.zEdge, b.yEdge);
                break;
            case 196:
                addTriangle(a.yEdge, f.yEdge, e.yEdge);
                addTriangle(c.xEdge, d.zEdge, a.yEdge);
                addTriangle(a.yEdge, d.zEdge, f.yEdge);
                break;
            case 200:
                addTriangle(b.yEdge, f.yEdge, e.yEdge);
                addTriangle(c.xEdge, b.yEdge, c.zEdge);
                addTriangle(c.zEdge, b.yEdge, e.yEdge);
                break;
            case 208:
                addTriangle(a.zEdge, c.zEdge, d.zEdge);
                addTriangle(f.yEdge, e.xEdge, a.zEdge);
                addTriangle(a.zEdge, d.zEdge, f.yEdge);
                break;
            case 224:                
                addTriangle(b.zEdge, c.zEdge, d.zEdge);
                addTriangle(e.yEdge, b.zEdge, e.xEdge);
                addTriangle(b.zEdge, e.yEdge, c.zEdge);
                break;
            //4 vertices finally
            case 15:
                addTriangle(a.zEdge, b.zEdge, c.zEdge);
                addTriangle(c.zEdge, b.zEdge, d.zEdge);
                break;
            case 23:
                addTriangle(b.yEdge, c.xEdge, c.zEdge);
                addTriangle(b.yEdge, c.zEdge, b.zEdge);
                addTriangle(c.zEdge, e.yEdge, b.zEdge);
                addTriangle(b.zEdge, e.yEdge, e.xEdge);
                break;
            case 27:
                addTriangle(a.yEdge, e.yEdge, c.xEdge);
                addTriangle(c.xEdge, e.yEdge, b.zEdge);
                addTriangle(e.yEdge, e.xEdge, b.zEdge);
                addTriangle(c.xEdge, b.zEdge, d.zEdge);
                break;
            case 29:
                addTriangle(a.xEdge, b.yEdge, e.xEdge);
                addTriangle(e.xEdge, b.yEdge, c.zEdge);
                addTriangle(e.xEdge, c.zEdge, e.yEdge);
                addTriangle(b.yEdge, d.zEdge, c.zEdge);
                break;
            case 30:
                addTriangle(b.zEdge, d.zEdge, c.zEdge);
                addTriangle(c.zEdge, a.xEdge, b.zEdge);
                addTriangle(c.zEdge, a.yEdge, a.xEdge);
                addTriangle(a.zEdge, e.yEdge, e.xEdge);//16
                break;
            case 39:
                /*addTriangle(c.zEdge, a.zEdge, e.xEdge);
                addTriangle(c.zEdge, e.xEdge, c.xEdge);
                addTriangle(e.xEdge, f.yEdge, c.xEdge);
                addTriangle(e.xEdge, f.yEdge, b.yEdge);*/
                addTriangle(a.zEdge, e.xEdge, c.zEdge);
                addTriangle(c.zEdge, e.xEdge, b.yEdge);
                addTriangle(e.xEdge, f.yEdge, b.yEdge);
                addTriangle(c.zEdge, b.yEdge, c.xEdge);
                break;
            case 43:
                addTriangle(a.yEdge, d.zEdge, c.xEdge);
                addTriangle(a.yEdge, a.zEdge, d.zEdge);
                addTriangle(a.zEdge, f.yEdge, d.zEdge);
                addTriangle(a.zEdge, e.xEdge, f.yEdge);
                break;
            case 45:
                addTriangle(d.zEdge, a.zEdge, a.xEdge);
                addTriangle(c.zEdge, a.zEdge, d.zEdge);
                addTriangle(a.xEdge, b.yEdge, d.zEdge);
                addTriangle(e.xEdge, f.yEdge, b.zEdge);//32
                break;
            case 46:
                addTriangle(a.xEdge, e.xEdge, a.yEdge);
                addTriangle(a.yEdge, e.xEdge, d.zEdge);
                addTriangle(e.xEdge, f.yEdge, d.zEdge);
                addTriangle(a.yEdge, d.zEdge, c.zEdge);
                break;
            case 51:
                addTriangle(a.yEdge, e.yEdge, b.yEdge);
                addTriangle(e.yEdge, f.yEdge, b.yEdge);
                break;
            case 53:
                addTriangle(a.xEdge, c.xEdge, b.zEdge);
                addTriangle(c.xEdge, e.yEdge, b.zEdge);
                addTriangle(c.xEdge, c.zEdge, e.yEdge);
                addTriangle(b.zEdge, e.yEdge, f.yEdge);
                break;
            case 54:
                addTriangle(b.yEdge, e.yEdge, f.yEdge);
                addTriangle(e.yEdge, b.yEdge, a.xEdge);
                addTriangle(a.xEdge, a.zEdge, e.yEdge);
                addTriangle(a.yEdge, c.xEdge, c.zEdge);
                break;
            case 57:
                addTriangle(a.yEdge, e.yEdge, f.yEdge);
                addTriangle(a.xEdge, a.yEdge, f.yEdge);
                addTriangle(a.xEdge, f.yEdge, b.zEdge);
                addTriangle(c.xEdge, b.yEdge, d.zEdge);
                break;
            case 58:
                addTriangle(a.xEdge, a.zEdge, c.xEdge);
                addTriangle(c.xEdge, a.zEdge, f.yEdge);
                addTriangle(f.yEdge, d.zEdge, c.xEdge);
                addTriangle(a.zEdge, e.yEdge, f.yEdge);
                break;
            case 60:
                addTriangle(a.yEdge, b.yEdge, d.zEdge);
                addTriangle(d.zEdge, c.zEdge, a.yEdge);
                addTriangle(a.zEdge, e.yEdge, b.zEdge);
                addTriangle(e.yEdge, f.yEdge, b.zEdge);
                break;
            case 71:
                addTriangle(a.xEdge, b.yEdge, e.xEdge);
                addTriangle(e.xEdge, b.yEdge, c.zEdge);
                addTriangle(e.xEdge, c.zEdge, e.yEdge);
                addTriangle(b.yEdge, d.zEdge, c.zEdge);
                break;
            case 75:
                addTriangle(b.zEdge, d.zEdge, a.zEdge);
                addTriangle(a.zEdge, d.zEdge, a.yEdge);
                addTriangle(a.yEdge, d.zEdge, c.xEdge);
                addTriangle(e.yEdge, c.zEdge, g.xEdge);//64
                break;
            case 77:
                addTriangle(a.zEdge, g.xEdge, e.yEdge);
                addTriangle(a.xEdge, g.xEdge, a.zEdge);
                addTriangle(a.xEdge, d.zEdge, g.xEdge);
                addTriangle(a.xEdge, b.yEdge, d.zEdge);
                break;
            case 78:
                addTriangle(a.xEdge, e.yEdge, a.yEdge);
                addTriangle(a.xEdge, d.zEdge, e.yEdge);
                addTriangle(e.yEdge, d.zEdge, g.xEdge);
                addTriangle(a.xEdge, b.zEdge, d.zEdge);
                break;
            case 83:
                addTriangle(a.yEdge, c.zEdge, b.yEdge);
                addTriangle(b.yEdge, c.zEdge, e.xEdge);
                addTriangle(b.yEdge, e.xEdge, b.zEdge);
                addTriangle(c.zEdge, g.xEdge, e.xEdge);
                break;
            case 85:
                addTriangle(a.xEdge, c.xEdge, g.xEdge);
                addTriangle(g.xEdge, e.xEdge, a.xEdge);
                break;
            case 86:
                addTriangle(c.xEdge, g.xEdge, e.xEdge);
                addTriangle(a.yEdge, c.xEdge, a.zEdge);
                addTriangle(a.zEdge, c.xEdge, e.xEdge);
                addTriangle(a.xEdge, b.zEdge, b.yEdge);
                break;
            case 89:
                addTriangle(a.xEdge, g.xEdge, e.xEdge);
                addTriangle(a.xEdge, a.yEdge, g.xEdge);
                addTriangle(a.yEdge, c.zEdge, g.xEdge);
                addTriangle(c.xEdge, b.yEdge, d.zEdge);
                break;
            case 90:
                addTriangle(a.zEdge, g.xEdge, e.xEdge);
                addTriangle(g.xEdge, a.zEdge, c.zEdge);
                addTriangle(a.xEdge, d.zEdge, c.xEdge);
                addTriangle(b.zEdge, d.zEdge, a.xEdge);
                break;
            case 92:
                addTriangle(a.yEdge, b.yEdge, a.zEdge);
                addTriangle(a.zEdge, b.yEdge, g.xEdge);
                addTriangle(b.yEdge, d.zEdge, g.xEdge);
                addTriangle(a.zEdge, g.xEdge, e.xEdge);
                break;
            case 99:
                addTriangle(a.yEdge, f.yEdge, b.yEdge);
                addTriangle(a.yEdge, a.zEdge, f.yEdge);
                addTriangle(e.xEdge, f.yEdge, a.zEdge);
                addTriangle(e.yEdge, c.zEdge, g.xEdge);
                break;
            case 101:
                addTriangle(a.xEdge, c.xEdge, g.xEdge);
                addTriangle(a.xEdge, g.xEdge, a.zEdge);
                addTriangle(a.zEdge, g.xEdge, e.yEdge);
                addTriangle(e.xEdge, f.yEdge, b.zEdge);
                break;
            case 102:
                addTriangle(c.xEdge, g.xEdge, a.yEdge);
                addTriangle(e.yEdge, a.yEdge, g.xEdge);
                addTriangle(e.xEdge, b.yEdge, a.xEdge);
                addTriangle(b.yEdge, e.xEdge, f.yEdge);
                break;
            case 105:
                addTriangle(a.xEdge, a.yEdge, a.zEdge);
                addTriangle(c.xEdge, b.yEdge, d.zEdge);
                addTriangle(e.xEdge, f.yEdge, b.zEdge);
                addTriangle(e.yEdge, c.zEdge, g.xEdge);
                break;
            case 106:
                addTriangle(a.xEdge, e.xEdge, c.xEdge);
                addTriangle(c.xEdge, e.xEdge, f.yEdge);
                addTriangle(f.yEdge, d.zEdge, c.xEdge);
                addTriangle(e.yEdge, c.zEdge, g.xEdge);
                break;
            case 108:
                addTriangle(a.yEdge, b.yEdge, e.yEdge);
                addTriangle(d.zEdge, g.xEdge, b.yEdge);
                addTriangle(e.yEdge, b.yEdge, g.xEdge);
                addTriangle(e.xEdge, f.yEdge, b.zEdge);
                break;
            case 113:
                addTriangle(a.yEdge, c.zEdge, g.xEdge);
                addTriangle(a.yEdge, g.xEdge, a.xEdge);
                addTriangle(a.xEdge, g.xEdge, f.yEdge);
                addTriangle(a.xEdge, f.yEdge, b.zEdge);
                break;
            case 114:
                addTriangle(a.zEdge, a.xEdge, c.zEdge);
                addTriangle(a.xEdge, f.yEdge, c.zEdge);
                addTriangle(c.zEdge, f.yEdge, g.xEdge);
                addTriangle(a.xEdge, b.yEdge, f.yEdge);
                break;
            case 116:
                addTriangle(g.xEdge, f.yEdge, c.xEdge);
                addTriangle(f.yEdge, a.zEdge, c.xEdge);
                addTriangle(c.xEdge, a.zEdge, a.yEdge);
                addTriangle(f.yEdge, b.zEdge, a.zEdge);
                break;
            case 120:
                addTriangle(a.zEdge, c.zEdge, b.zEdge);
                addTriangle(c.zEdge, g.xEdge, f.yEdge);
                addTriangle(c.zEdge, f.yEdge, b.zEdge);
                addTriangle(c.xEdge, b.yEdge, d.zEdge);
                break;
            /////halfway there/////////
            case 255:
                return;
            // one vertex configurations
            case 254:
                addTriangle(a.xEdge, a.zEdge, a.yEdge);
                break;
            case 253:
                addTriangle(a.xEdge, b.yEdge, b.zEdge);
                break;
            //xm_stranno
            case 251:
                addTriangle(a.yEdge, c.zEdge, c.xEdge);
                break;
            case 247:
                addTriangle(c.xEdge, d.zEdge, b.yEdge);
                break;
            //
            case 239:
                addTriangle(a.zEdge, e.xEdge, e.yEdge);
                break;
            case 223:

                addTriangle(e.xEdge, b.zEdge, f.yEdge);
                break;
            case 191:

                addTriangle(e.yEdge, g.xEdge, c.zEdge);
                break;
            case 127:
                addTriangle(f.yEdge, d.zEdge, g.xEdge);
                break;
            // two vertex configurations

            case 252:
                addTriangle(b.zEdge, a.zEdge, a.yEdge);
                addTriangle(a.yEdge, b.yEdge, b.zEdge);
                break;
            case 250:
                addTriangle(a.xEdge, a.zEdge, c.zEdge);
                addTriangle(c.xEdge, a.xEdge, c.zEdge);
                break;
            case 249:
                addTriangle(a.xEdge, b.yEdge, b.zEdge);
                addTriangle(a.yEdge, c.zEdge, c.xEdge);
                break;
            case 246:
                addTriangle(a.xEdge, a.zEdge, a.yEdge);
                addTriangle(c.xEdge, d.zEdge, b.yEdge);
                break;
            case 245:
                addTriangle(a.xEdge, c.xEdge, d.zEdge);
                addTriangle(b.zEdge, a.xEdge, d.zEdge);
                break;
            case 243:
                addTriangle(a.yEdge, d.zEdge, b.yEdge);
                addTriangle(d.zEdge, a.yEdge, c.zEdge);
                break;
            case 238:
                addTriangle(a.xEdge, e.xEdge, e.yEdge);
                addTriangle(e.yEdge, a.yEdge, a.xEdge);
                break;
            case 237:
                addTriangle(a.xEdge, b.yEdge, b.zEdge);
                addTriangle(a.zEdge, e.xEdge, e.yEdge);
                break;
            case 235:
                addTriangle(a.yEdge, c.zEdge, c.xEdge);
                addTriangle(a.zEdge, e.xEdge, e.yEdge);
                break;
            case 231:
                addTriangle(c.xEdge, d.zEdge, b.yEdge);
                addTriangle(a.zEdge, e.xEdge, e.yEdge);
                break;
            case 222:
                addTriangle(a.xEdge, a.zEdge, a.yEdge);
                addTriangle(e.xEdge, b.zEdge, f.yEdge);
                break;
            case 221:
                addTriangle(e.xEdge, a.xEdge, b.yEdge);
                addTriangle(b.yEdge, f.yEdge, e.xEdge);
                break;
            case 219:
                addTriangle(a.yEdge, c.zEdge, c.xEdge);
                addTriangle(e.xEdge, b.zEdge, f.yEdge);
                break;
            case 215:
                addTriangle(c.xEdge, d.zEdge, b.yEdge);
                addTriangle(e.xEdge, b.zEdge, f.yEdge);
                break;
            case 207:
                addTriangle(a.zEdge, b.zEdge, e.yEdge);
                addTriangle(e.yEdge, b.zEdge, f.yEdge);
                break;
            case 190:
                addTriangle(a.xEdge, a.zEdge, a.yEdge);
                addTriangle(e.yEdge, g.xEdge, c.zEdge);
                break;
            case 189:
                addTriangle(a.xEdge, b.yEdge, b.zEdge);
                addTriangle(e.yEdge, g.xEdge, c.zEdge);
                break;
            case 187:
                addTriangle(c.xEdge, a.yEdge, g.xEdge);
                addTriangle(e.yEdge, g.xEdge, a.yEdge);
                break;
            case 183:
                addTriangle(c.xEdge, d.zEdge, b.yEdge);
                addTriangle(e.yEdge, g.xEdge, c.zEdge);
                break;
            case 175:
                addTriangle(a.zEdge, e.xEdge, g.xEdge);
                addTriangle(g.xEdge, c.zEdge, a.zEdge);
                break;
            case 159:
                addTriangle(e.xEdge, b.zEdge, f.yEdge);
                addTriangle(e.yEdge, g.xEdge, c.zEdge);
                break;
            case 126:
                addTriangle(a.xEdge, a.zEdge, a.yEdge);
                addTriangle(f.yEdge, d.zEdge, g.xEdge);
                break;
            case 125:
                addTriangle(a.xEdge, b.yEdge, b.zEdge);
                addTriangle(f.yEdge, d.zEdge, g.xEdge);
                break;
            case 123:
                addTriangle(a.yEdge, c.zEdge, c.xEdge);
                addTriangle(f.yEdge, d.zEdge, g.xEdge);
                break;
            case 119:
                addTriangle(g.xEdge, b.yEdge, c.xEdge);
                addTriangle(b.yEdge, g.xEdge, f.yEdge);
                break;
            case 111:
                addTriangle(a.zEdge, e.xEdge, e.yEdge);
                addTriangle(f.yEdge, d.zEdge, g.xEdge);
                break;
            case 95:
                addTriangle(g.xEdge, e.xEdge, b.zEdge);
                addTriangle(b.zEdge, d.zEdge, g.xEdge);
                break;
            case 63:
                addTriangle(e.yEdge, f.yEdge, c.zEdge);
                addTriangle(c.zEdge, f.yEdge, d.zEdge);
                break;
            //three vertices
            case 248:
                addTriangle(c.zEdge, b.zEdge, a.zEdge);
                addTriangle(c.zEdge, b.yEdge, b.zEdge);
                addTriangle(c.xEdge, b.yEdge, c.zEdge);
                break;
            case 244:
                addTriangle(b.zEdge, a.zEdge, d.zEdge);
                addTriangle(a.zEdge, a.yEdge, d.zEdge);
                addTriangle(a.yEdge, c.xEdge, d.zEdge);
                break;
            case 242:
                addTriangle(d.zEdge, a.xEdge, a.zEdge);
                addTriangle(c.zEdge, d.zEdge, a.zEdge);
                addTriangle(a.xEdge, d.zEdge, b.yEdge);
                break;
            case 241:
                addTriangleR(b.zEdge, d.zEdge, c.zEdge);
                addTriangleR(c.zEdge, a.xEdge, b.zEdge);
                addTriangleR(c.zEdge, a.yEdge, a.xEdge);
                break;
            case 236:
                addTriangleR(a.yEdge, e.yEdge, b.yEdge);
                addTriangleR(e.yEdge, e.xEdge, b.zEdge);
                addTriangleR(b.yEdge, e.yEdge, b.zEdge);
                break;
            case 234:
                addTriangleR(a.xEdge, c.xEdge, e.xEdge);
                addTriangleR(c.xEdge, c.zEdge, e.xEdge);
                addTriangleR(c.zEdge, e.yEdge, e.xEdge);
                break;
            case 233:
                addTriangleR(a.xEdge, b.zEdge, b.yEdge);
                addTriangleR(a.yEdge, c.xEdge, c.zEdge);
                addTriangleR(a.zEdge, e.yEdge, e.xEdge);
                break;
            case 230:
                addTriangleR(a.xEdge, e.yEdge, e.xEdge);
                addTriangleR(e.yEdge, a.xEdge, a.yEdge);
                addTriangleR(c.xEdge, b.yEdge, d.zEdge);
                break;
            case 229:
                addTriangleR(a.xEdge, d.zEdge, c.xEdge);
                addTriangleR(b.zEdge, d.zEdge, a.xEdge);
                addTriangleR(a.zEdge, e.yEdge, e.xEdge);
                break;
            case 227:
                addTriangleR(a.yEdge, b.yEdge, d.zEdge);
                addTriangleR(d.zEdge, c.zEdge, a.yEdge);
                addTriangleR(a.zEdge, e.yEdge, e.xEdge);
                break;
            case 220:
                addTriangleR(a.yEdge, f.yEdge, b.yEdge);
                addTriangleR(a.yEdge, a.zEdge, f.yEdge);
                addTriangleR(e.xEdge, f.yEdge, a.zEdge);
                break;
            case 218:
                addTriangleR(a.xEdge, c.zEdge, a.zEdge);
                addTriangleR(c.xEdge, c.zEdge, a.xEdge);
                addTriangleR(e.xEdge, f.yEdge, b.zEdge);
                break;
            case 217:
                addTriangleR(e.xEdge, b.yEdge, a.xEdge);
                addTriangleR(b.yEdge, e.xEdge, f.yEdge);
                addTriangleR(a.yEdge, c.xEdge, c.zEdge);
                break;
            case 214:
                addTriangleR(a.xEdge, a.yEdge, a.zEdge);
                addTriangleR(c.xEdge, b.yEdge, d.zEdge);
                addTriangleR(e.xEdge, f.yEdge, b.zEdge);
                break;
            case 213:
                addTriangleR(a.xEdge, e.xEdge, c.xEdge);
                addTriangleR(c.xEdge, e.xEdge, f.yEdge);
                addTriangleR(f.yEdge, d.zEdge, c.xEdge);
                break;
            case 211:
                addTriangleR(a.yEdge, b.yEdge, d.zEdge);
                addTriangleR(d.zEdge, c.zEdge, a.yEdge);
                addTriangleR(e.xEdge, f.yEdge, b.zEdge);
                break;
            case 206:
                addTriangleR(a.yEdge, e.yEdge, f.yEdge);
                addTriangleR(a.xEdge, a.yEdge, f.yEdge);
                addTriangleR(a.xEdge, f.yEdge, b.zEdge);
                break;
            case 205:
                addTriangleR(b.yEdge, e.yEdge, f.yEdge);
                addTriangleR(e.yEdge, b.yEdge, a.xEdge);
                addTriangleR(a.xEdge, a.zEdge, e.yEdge);
                break;
            case 203:
                addTriangleR(a.zEdge, e.yEdge, b.zEdge);
                addTriangleR(e.yEdge, f.yEdge, b.zEdge);
                addTriangleR(a.yEdge, c.xEdge, c.zEdge);
                break;
            case 199:
                addTriangleR(a.zEdge, e.yEdge, b.zEdge);
                addTriangleR(e.yEdge, f.yEdge, b.zEdge);
                addTriangleR(c.xEdge, b.yEdge, d.zEdge);
                break;
            case 188:
                addTriangleR(b.zEdge, a.yEdge, a.zEdge);
                addTriangleR(a.yEdge, b.zEdge, b.yEdge);
                addTriangleR(e.yEdge, c.zEdge, g.xEdge);
                break;
            case 186:
                addTriangleR(a.xEdge, c.xEdge, g.xEdge);
                addTriangleR(a.xEdge, g.xEdge, a.zEdge);
                addTriangleR(a.zEdge, g.xEdge, e.yEdge);
                break;
            case 185:
                addTriangleR(c.xEdge, g.xEdge, a.yEdge);
                addTriangleR(e.yEdge, a.yEdge, g.xEdge);
                addTriangleR(a.xEdge, b.zEdge, b.yEdge);
                break;
            case 182:
                addTriangleR(a.xEdge, a.yEdge, a.zEdge);
                addTriangleR(c.xEdge, b.yEdge, d.zEdge);
                addTriangleR(e.yEdge, c.zEdge, g.xEdge);
                break;
            case 181:
                addTriangleR(a.xEdge, d.zEdge, c.xEdge);
                addTriangleR(b.zEdge, d.zEdge, a.xEdge);
                addTriangleR(e.yEdge, c.zEdge, g.xEdge);
                break;
            case 179:
                addTriangleR(a.yEdge, b.yEdge, e.yEdge);
                addTriangleR(d.zEdge, g.xEdge, b.yEdge);
                addTriangleR(e.yEdge, b.yEdge, g.xEdge);
                break;
            case 174:
                addTriangleR(a.xEdge, g.xEdge, e.xEdge);
                addTriangleR(a.xEdge, a.yEdge, g.xEdge);
                addTriangleR(a.yEdge, c.zEdge, g.xEdge);
                break;
            case 173:
                addTriangleR(a.zEdge, g.xEdge, e.xEdge);
                addTriangleR(g.xEdge, a.zEdge, c.zEdge);
                addTriangleR(a.xEdge, b.zEdge, b.yEdge);
                break;
            case 171:
                addTriangleR(c.xEdge, g.xEdge, e.xEdge);
                addTriangleR(a.yEdge, c.xEdge, a.zEdge);
                addTriangleR(a.zEdge, c.xEdge, e.xEdge);
                break;
            case 167:
                addTriangleR(a.zEdge, g.xEdge, e.xEdge);
                addTriangleR(g.xEdge, a.zEdge, c.zEdge);
                addTriangleR(c.xEdge, b.yEdge, d.zEdge);
                break;
            case 158:
                addTriangleR(a.xEdge, a.yEdge, a.zEdge);
                addTriangleR(e.xEdge, f.yEdge, b.zEdge);
                addTriangleR(e.yEdge, c.zEdge, g.xEdge);
                break;
            case 157:
                addTriangleR(e.xEdge, b.yEdge, a.xEdge);
                addTriangleR(b.yEdge, e.xEdge, f.yEdge);
                addTriangleR(e.yEdge, c.zEdge, g.xEdge);
                break;
            case 155:
                addTriangleR(c.xEdge, g.xEdge, a.yEdge);
                addTriangleR(e.yEdge, a.yEdge, g.xEdge);
                addTriangleR(e.xEdge, f.yEdge, b.zEdge);
                break;
            case 151:
                addTriangleR(c.xEdge, b.yEdge, d.zEdge);
                addTriangleR(e.xEdge, f.yEdge, b.zEdge);
                addTriangleR(e.yEdge, c.zEdge, g.xEdge);
                break;
            case 143:
                addTriangleR(a.zEdge, c.zEdge, b.zEdge);
                addTriangleR(c.zEdge, g.xEdge, f.yEdge);
                addTriangleR(c.zEdge, f.yEdge, b.zEdge);
                break;
            case 124:
                addTriangleR(b.zEdge, a.yEdge, a.zEdge);
                addTriangleR(a.yEdge, b.zEdge, b.yEdge);
                addTriangleR(f.yEdge, g.xEdge, d.zEdge); //128
                break;
            case 122:
                addTriangleR(a.xEdge, c.zEdge, a.zEdge);
                addTriangleR(c.xEdge, c.zEdge, a.xEdge);
                addTriangleR(f.yEdge, g.xEdge, d.zEdge);
                break;
            case 121:
                addTriangleR(a.xEdge, b.zEdge, b.yEdge);
                addTriangleR(a.yEdge, c.xEdge, c.zEdge);
                addTriangleR(f.yEdge, g.xEdge, d.zEdge);
                break;
            case 118:
                addTriangleR(g.xEdge, c.xEdge, b.yEdge);
                addTriangleR(b.yEdge, f.yEdge, g.xEdge);
                addTriangleR(a.xEdge, a.yEdge, a.zEdge);
                break;
            case 117:
                addTriangleR(a.xEdge, g.xEdge, c.xEdge);
                addTriangleR(b.zEdge, f.yEdge, g.xEdge);
                addTriangleR(a.xEdge, b.zEdge, g.xEdge);
                break;
            case 115:
                addTriangleR(a.yEdge, b.yEdge, f.yEdge);
                addTriangleR(c.zEdge, a.yEdge, g.xEdge);
                addTriangleR(a.yEdge, f.yEdge, g.xEdge);
                break;
            case 110:
                addTriangleR(a.xEdge, e.yEdge, e.xEdge);
                addTriangleR(e.yEdge, a.xEdge, a.yEdge);
                addTriangleR(f.yEdge, g.xEdge, d.zEdge);
                break;
            case 109:
                addTriangleR(a.xEdge, b.zEdge, b.yEdge);
                addTriangleR(a.zEdge, e.yEdge, e.xEdge);
                addTriangleR(f.yEdge, g.xEdge, d.zEdge);
                break;
            case 107:
                addTriangleR(a.yEdge, c.xEdge, c.zEdge);
                addTriangleR(a.zEdge, e.yEdge, e.xEdge);
                addTriangleR(f.yEdge, g.xEdge, d.zEdge);
                break;
            case 103:
                addTriangleR(g.xEdge, c.xEdge, b.yEdge);
                addTriangleR(b.yEdge, f.yEdge, g.xEdge);
                addTriangleR(a.zEdge, e.yEdge, e.xEdge);//16
                break;
            case 94:
                addTriangleR(g.xEdge, b.zEdge, e.xEdge);
                addTriangleR(b.zEdge, g.xEdge, d.zEdge);
                addTriangleR(a.xEdge, a.yEdge, a.zEdge);//1
                break;
            case 93:
                addTriangleR(a.xEdge, e.xEdge, g.xEdge);
                addTriangleR(d.zEdge, b.yEdge, a.xEdge);
                addTriangleR(a.xEdge, g.xEdge, d.zEdge);
                break;
            case 91:
                addTriangleR(g.xEdge, b.zEdge, e.xEdge);
                addTriangleR(b.zEdge, g.xEdge, d.zEdge);
                addTriangleR(a.yEdge, c.xEdge, c.zEdge);//4
                break;
            case 87:
                addTriangleR(c.xEdge, e.xEdge, g.xEdge);
                addTriangleR(b.zEdge, c.xEdge, b.yEdge);
                addTriangleR(e.xEdge, c.xEdge, b.zEdge);
                break;
            case 79:
                addTriangleR(a.zEdge, d.zEdge, b.zEdge);
                addTriangleR(e.yEdge, g.xEdge, d.zEdge);
                addTriangleR(a.zEdge, e.yEdge, d.zEdge);
                break;
            case 62:
                addTriangleR(e.yEdge, c.zEdge, f.yEdge);//192->
                addTriangleR(c.zEdge, d.zEdge, f.yEdge);//192->
                addTriangleR(a.xEdge, a.yEdge, a.zEdge);
                break;
            case 61:
                addTriangleR(e.yEdge, c.zEdge, f.yEdge);//192->
                addTriangleR(c.zEdge, d.zEdge, f.yEdge);
                addTriangleR(a.xEdge, b.zEdge, b.yEdge);
                break;
            case 59:
                addTriangleR(a.yEdge, f.yEdge, e.yEdge);
                addTriangleR(c.xEdge, d.zEdge, a.yEdge);
                addTriangleR(a.yEdge, d.zEdge, f.yEdge);
                break;
            case 55:
                addTriangleR(b.yEdge, f.yEdge, e.yEdge);
                addTriangleR(c.xEdge, b.yEdge, c.zEdge);
                addTriangleR(c.zEdge, b.yEdge, e.yEdge);
                break;
            case 47:
                addTriangleR(a.zEdge, c.zEdge, d.zEdge);
                addTriangleR(f.yEdge, e.xEdge, a.zEdge);
                addTriangleR(a.zEdge, d.zEdge, f.yEdge);
                break;
            case 31:
                addTriangleR(b.zEdge, c.zEdge, d.zEdge);
                addTriangleR(e.yEdge, b.zEdge, e.xEdge);
                addTriangleR(b.zEdge, e.yEdge, c.zEdge);
                break;
            //4 vertices finally
            case 240:
                addTriangleR(a.zEdge, b.zEdge, c.zEdge);
                addTriangleR(c.zEdge, b.zEdge, d.zEdge);
                break;
            case 232:
                addTriangleR(b.yEdge, c.xEdge, c.zEdge);
                addTriangleR(b.yEdge, c.zEdge, b.zEdge);
                addTriangleR(c.zEdge, e.yEdge, b.zEdge);
                addTriangleR(b.zEdge, e.yEdge, e.xEdge);
                break;
            case 228:
                addTriangleR(a.yEdge, e.yEdge, c.xEdge);
                addTriangleR(c.xEdge, e.yEdge, b.zEdge);
                addTriangleR(e.yEdge, e.xEdge, b.zEdge);
                addTriangleR(c.xEdge, b.zEdge, d.zEdge);
                break;
            case 226:
                addTriangleR(a.xEdge, b.yEdge, e.xEdge);
                addTriangleR(e.xEdge, b.yEdge, c.zEdge);
                addTriangleR(e.xEdge, c.zEdge, e.yEdge);
                addTriangleR(b.yEdge, d.zEdge, c.zEdge);
                break;
            case 225:
                addTriangleR(b.zEdge, d.zEdge, c.zEdge);
                addTriangleR(c.zEdge, a.xEdge, b.zEdge);
                addTriangleR(c.zEdge, a.yEdge, a.xEdge);
                addTriangleR(a.zEdge, e.yEdge, e.xEdge);//16
                break;
            case 216:
                /*addTriangleR(c.zEdge, a.zEdge, e.xEdge);
                addTriangleR(c.zEdge, e.xEdge, c.xEdge);
                addTriangleR(e.xEdge, f.yEdge, c.xEdge);
                addTriangleR(e.xEdge, f.yEdge, b.yEdge);*/
                addTriangleR(a.zEdge, e.xEdge, c.zEdge);
                addTriangleR(c.zEdge, e.xEdge, b.yEdge);
                addTriangleR(e.xEdge, f.yEdge, b.yEdge);
                addTriangleR(c.zEdge, b.yEdge, c.xEdge);
                break;
            case 212:
                addTriangleR(a.yEdge, d.zEdge, c.xEdge);
                addTriangleR(a.yEdge, a.zEdge, d.zEdge);
                addTriangleR(a.zEdge, f.yEdge, d.zEdge);
                addTriangleR(a.zEdge, e.xEdge, f.yEdge);
                break;
            case 210:
                addTriangleR(d.zEdge, a.zEdge, a.xEdge);
                addTriangleR(c.zEdge, a.zEdge, d.zEdge);
                addTriangleR(a.xEdge, b.yEdge, d.zEdge);
                addTriangleR(e.xEdge, f.yEdge, b.zEdge);//32
                break;
            case 209:
                addTriangleR(a.xEdge, e.xEdge, a.yEdge);
                addTriangleR(a.yEdge, e.xEdge, d.zEdge);
                addTriangleR(e.xEdge, f.yEdge, d.zEdge);
                addTriangleR(a.yEdge, d.zEdge, c.zEdge);
                break;
            case 204:
                addTriangleR(a.yEdge, e.yEdge, b.yEdge);
                addTriangleR(e.yEdge, f.yEdge, b.yEdge);
                break;
            case 202:
                addTriangleR(a.xEdge, c.xEdge, b.zEdge);
                addTriangleR(c.xEdge, e.yEdge, b.zEdge);
                addTriangleR(c.xEdge, c.zEdge, e.yEdge);
                addTriangleR(b.zEdge, e.yEdge, f.yEdge);
                break;
            case 201:
                addTriangleR(b.yEdge, e.yEdge, f.yEdge);
                addTriangleR(e.yEdge, b.yEdge, a.xEdge);
                addTriangleR(a.xEdge, a.zEdge, e.yEdge);
                addTriangleR(a.yEdge, c.xEdge, c.zEdge);
                break;
            case 198:
                addTriangleR(a.yEdge, e.yEdge, f.yEdge);
                addTriangleR(a.xEdge, a.yEdge, f.yEdge);
                addTriangleR(a.xEdge, f.yEdge, b.zEdge);
                addTriangleR(c.xEdge, b.yEdge, d.zEdge);
                break;
            case 197:
                addTriangleR(a.xEdge, a.zEdge, c.xEdge);
                addTriangleR(c.xEdge, a.zEdge, f.yEdge);
                addTriangleR(f.yEdge, d.zEdge, c.xEdge);
                addTriangleR(a.zEdge, e.yEdge, f.yEdge);
                break;
            case 195:
                /*
                addTriangleR(a.yEdge, b.yEdge, d.zEdge);
                addTriangleR(d.zEdge, c.zEdge, a.yEdge);
                addTriangleR(a.zEdge, e.yEdge, b.zEdge);
                addTriangleR(e.yEdge, f.yEdge, b.zEdge);*/
                addTriangle(b.zEdge, a.yEdge, a.zEdge);
                addTriangle(a.yEdge, b.zEdge, b.yEdge);
                addTriangle(e.yEdge, c.zEdge, f.yEdge);
                addTriangle(c.zEdge, d.zEdge, f.yEdge);

                break;
            case 184:
                addTriangleR(a.xEdge, b.yEdge, e.xEdge);
                addTriangleR(e.xEdge, b.yEdge, c.zEdge);
                addTriangleR(e.xEdge, c.zEdge, e.yEdge);
                addTriangleR(b.yEdge, d.zEdge, c.zEdge);
                break;
            case 180:
                addTriangleR(b.zEdge, d.zEdge, a.zEdge);
                addTriangleR(a.zEdge, d.zEdge, a.yEdge);
                addTriangleR(a.yEdge, d.zEdge, c.xEdge);
                addTriangleR(e.yEdge, c.zEdge, g.xEdge);//64
                break;
            case 178:
                addTriangleR(a.zEdge, g.xEdge, e.yEdge);
                addTriangleR(a.xEdge, g.xEdge, a.zEdge);
                addTriangleR(a.xEdge, d.zEdge, g.xEdge);
                addTriangleR(a.xEdge, b.yEdge, d.zEdge);
                break;
            case 177:
                addTriangleR(a.xEdge, e.yEdge, a.yEdge);
                addTriangleR(a.xEdge, d.zEdge, e.yEdge);
                addTriangleR(e.yEdge, d.zEdge, g.xEdge);
                addTriangleR(a.xEdge, b.zEdge, d.zEdge);
                break;
            case 172:
                addTriangleR(a.yEdge, c.zEdge, b.yEdge);
                addTriangleR(b.yEdge, c.zEdge, e.xEdge);
                addTriangleR(b.yEdge, e.xEdge, b.zEdge);
                addTriangleR(c.zEdge, g.xEdge, e.xEdge);
                break;
            case 170:
                addTriangleR(a.xEdge, c.xEdge, g.xEdge);
                addTriangleR(g.xEdge, e.xEdge, a.xEdge);
                break;
            case 169:
                addTriangleR(c.xEdge, g.xEdge, e.xEdge);
                addTriangleR(a.yEdge, c.xEdge, a.zEdge);
                addTriangleR(a.zEdge, c.xEdge, e.xEdge);
                addTriangleR(a.xEdge, b.zEdge, b.yEdge);
                break;
            case 166:
                addTriangleR(a.xEdge, g.xEdge, e.xEdge);
                addTriangleR(a.xEdge, a.yEdge, g.xEdge);
                addTriangleR(a.yEdge, c.zEdge, g.xEdge);
                addTriangleR(c.xEdge, b.yEdge, d.zEdge);
                break;
            case 165:
                /*addTriangleR(a.zEdge, g.xEdge, e.xEdge);
                addTriangleR(g.xEdge, a.zEdge, c.zEdge);
                addTriangleR(a.xEdge, d.zEdge, c.xEdge);
                addTriangleR(b.zEdge, d.zEdge, a.xEdge);*/
                addTriangle(a.xEdge, c.zEdge, a.zEdge);
                addTriangle(c.xEdge, c.zEdge, a.xEdge);
                addTriangle(g.xEdge, b.zEdge, e.xEdge);
                addTriangle(b.zEdge, g.xEdge, d.zEdge);
                break;
            case 163:
                addTriangleR(a.yEdge, b.yEdge, a.zEdge);
                addTriangleR(a.zEdge, b.yEdge, g.xEdge);
                addTriangleR(b.yEdge, d.zEdge, g.xEdge);
                addTriangleR(a.zEdge, g.xEdge, e.xEdge);
                break;
            case 156:
                addTriangleR(a.yEdge, f.yEdge, b.yEdge);
                addTriangleR(a.yEdge, a.zEdge, f.yEdge);
                addTriangleR(e.xEdge, f.yEdge, a.zEdge);
                addTriangleR(e.yEdge, c.zEdge, g.xEdge);
                break;
            case 154:
                addTriangleR(a.xEdge, c.xEdge, g.xEdge);
                addTriangleR(a.xEdge, g.xEdge, a.zEdge);
                addTriangleR(a.zEdge, g.xEdge, e.yEdge);
                addTriangleR(e.xEdge, f.yEdge, b.zEdge);
                break;
            case 153:
                /*addTriangleR(c.xEdge, g.xEdge, a.yEdge);
                addTriangleR(e.yEdge, a.yEdge, g.xEdge);
                addTriangleR(e.xEdge, b.yEdge, a.xEdge);
                addTriangleR(b.yEdge, e.xEdge, f.yEdge);*/
                addTriangle(a.xEdge, e.yEdge, e.xEdge);
                addTriangle(e.yEdge, a.xEdge, a.yEdge);
                addTriangle(g.xEdge, c.xEdge, b.yEdge);
                addTriangle(b.yEdge, f.yEdge, g.xEdge);
                break;
            case 150:
                /*addTriangleR(a.xEdge, a.yEdge, a.zEdge);
                addTriangleR(c.xEdge, b.yEdge, d.zEdge);
                addTriangleR(e.xEdge, f.yEdge, b.zEdge);
                addTriangleR(e.yEdge, c.zEdge, g.xEdge);*/
                addTriangle(a.xEdge, b.zEdge, b.yEdge);
                addTriangle(a.yEdge, c.xEdge, c.zEdge);
                addTriangle(a.zEdge, e.yEdge, e.xEdge);
                addTriangle(f.yEdge, g.xEdge, d.zEdge);

                break;
            case 149:
                addTriangleR(a.xEdge, e.xEdge, c.xEdge);
                addTriangleR(c.xEdge, e.xEdge, f.yEdge);
                addTriangleR(f.yEdge, d.zEdge, c.xEdge);
                addTriangleR(e.yEdge, c.zEdge, g.xEdge);
                break;
            case 147:
                addTriangleR(a.yEdge, b.yEdge, e.yEdge);
                addTriangleR(d.zEdge, g.xEdge, b.yEdge);
                addTriangleR(e.yEdge, b.yEdge, g.xEdge);
                addTriangleR(e.xEdge, f.yEdge, b.zEdge);
                break;
            case 142:
                addTriangleR(a.yEdge, c.zEdge, g.xEdge);
                addTriangleR(a.yEdge, g.xEdge, a.xEdge);
                addTriangleR(a.xEdge, g.xEdge, f.yEdge);
                addTriangleR(a.xEdge, f.yEdge, b.zEdge);
                break;
            case 141:
                addTriangleR(a.zEdge, a.xEdge, c.zEdge);
                addTriangleR(a.xEdge, f.yEdge, c.zEdge);
                addTriangleR(c.zEdge, f.yEdge, g.xEdge);
                addTriangleR(a.xEdge, b.yEdge, f.yEdge);
                break;
            case 139:
                addTriangleR(g.xEdge, f.yEdge, c.xEdge);
                addTriangleR(f.yEdge, a.zEdge, c.xEdge);
                addTriangleR(c.xEdge, a.zEdge, a.yEdge);
                addTriangleR(f.yEdge, b.zEdge, a.zEdge);
                break;
            case 135:
                addTriangleR(a.zEdge, c.zEdge, b.zEdge);
                addTriangleR(c.zEdge, g.xEdge, f.yEdge);
                addTriangleR(c.zEdge, f.yEdge, b.zEdge);
                addTriangleR(c.xEdge, b.yEdge, d.zEdge);
                break;

        }

    }

    private void addTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);

        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }
    private void addTriangleR(Vector3 a, Vector3 b, Vector3 c)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);

        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
    }

}
