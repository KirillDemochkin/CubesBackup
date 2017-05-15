using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel {

    public bool state;
    public Vector3 xEdge, yEdge, zEdge;
    public Vector3 position;
    //public MeshRenderer meshRenderer;

    

    public Voxel(int x, int y, int z, float size)
    {
        position.x = (x + 0.5f) * size;
        position.y = (y + 0.5f) * size;
        position.z = (z + 0.5f) * size;
        state = false;

        xEdge = position;
        xEdge.x += size * 0.5f;
        yEdge = position;
        yEdge.y += size * 0.5f;
        zEdge = position;
        zEdge.z += size * 0.5f;
    }

    /*private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("collided with voxel");
        if (other.tag == "Brush")
        {
            if(other.transform.parent.gameObject.GetComponent<BrushController>().mode == BrushController.BrushMode.Filled)
            {
                state = true;
                meshRenderer.material.color = Color.red;
                Debug.Log(position.x + ", " + position.y + ", " + position.z + ", " + " is On");

            } else if(other.transform.parent.gameObject.GetComponent<BrushController>().mode == BrushController.BrushMode.Empty)
            {
                state = false;
                meshRenderer.material.color = Color.white;
                Debug.Log(position.x + ", " + position.y + ", " + position.z + ", " + " is Off");
            }
        }
    }*/

}
