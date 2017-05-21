using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel {

    public bool state;
    public Vector3 xEdge, yEdge, zEdge;
    public Vector3 position;
    //public MeshRenderer meshRenderer;

    public Voxel() {}    

    public Voxel(int x, int y, int z, float size, bool state)
    {
        position.x = (x + 0.5f) * size;
        position.y = (y + 0.5f) * size;
        position.z = (z + 0.5f) * size;
        this.state = state;

        xEdge = position;
        xEdge.x += size * 0.5f;
        yEdge = position;
        yEdge.y += size * 0.5f;
        zEdge = position;
        zEdge.z += size * 0.5f;
    }

    public void becomeXDummyOf(Voxel voxel, float offset)
    {
        state = voxel.state;
        position = voxel.position;
        xEdge = voxel.xEdge;
        yEdge = voxel.yEdge;
        zEdge = voxel.zEdge;
        position.x += offset;
        xEdge.x += offset;
        yEdge.x += offset;
        zEdge.x += offset;
    }

    public void becomeYDummyOf(Voxel voxel, float offset)
    {
        state = voxel.state;
        position = voxel.position;
        xEdge = voxel.xEdge;
        yEdge = voxel.yEdge;
        zEdge = voxel.zEdge;
        position.y += offset;
        xEdge.y += offset;
        yEdge.y += offset;
        zEdge.y += offset;
    }
    public void becomeZDummyOf(Voxel voxel, float offset)
    {
        state = voxel.state;
        position = voxel.position;
        xEdge = voxel.xEdge;
        yEdge = voxel.yEdge;
        zEdge = voxel.zEdge;
        position.z += offset;
        xEdge.z += offset;
        yEdge.z += offset;
        zEdge.z += offset;
    }
    public void becomeXYDummyOf(Voxel voxel, float offset)
    {
        state = voxel.state;
        position = voxel.position;
        xEdge = voxel.xEdge;
        yEdge = voxel.yEdge;
        zEdge = voxel.zEdge;
        position.y += offset;
        xEdge.y += offset;
        yEdge.y += offset;
        zEdge.y += offset;
        position.x += offset;
        xEdge.x += offset;
        yEdge.x += offset;
        zEdge.x += offset;
    }

    public void becomeXZDummyOf(Voxel voxel, float offset)
    {
        state = voxel.state;
        position = voxel.position;
        xEdge = voxel.xEdge;
        yEdge = voxel.yEdge;
        zEdge = voxel.zEdge;
        position.x += offset;
        xEdge.x += offset;
        yEdge.x += offset;
        zEdge.x += offset;
        position.z += offset;
        xEdge.z += offset;
        yEdge.z += offset;
        zEdge.z += offset;
    }

    public void becomeYZDummyOf(Voxel voxel, float offset)
    {
        state = voxel.state;
        position = voxel.position;
        xEdge = voxel.xEdge;
        yEdge = voxel.yEdge;
        zEdge = voxel.zEdge;
        position.y += offset;
        xEdge.y += offset;
        yEdge.y += offset;
        zEdge.y += offset;
        position.z += offset;
        xEdge.z += offset;
        yEdge.z += offset;
        zEdge.z += offset;
    }
    public void becomeXYZDummyOf(Voxel voxel, float offset)
    {
        state = voxel.state;
        position = voxel.position;
        xEdge = voxel.xEdge;
        yEdge = voxel.yEdge;
        zEdge = voxel.zEdge;
        position += Vector3.one * offset;
        xEdge += Vector3.one * offset;
        yEdge += Vector3.one * offset;
        zEdge += Vector3.one * offset;
    }

}
