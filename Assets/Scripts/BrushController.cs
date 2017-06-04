using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushController : MonoBehaviour {

	public enum BrushMode { Filled, Neutral, Empty};
    public event System.Action<Vector3, BrushMode> onVoxelCollided;
    public BrushMode mode;
    public int radius;
    public float speed = 2;
    public Transform brushSphere;
    

	void Start () {
        mode = BrushMode.Neutral;
        radius = 1;
	}
	
	
	void Update () {

        if (Input.GetKeyDown(KeyCode.Plus))
        {
            radius += 1;
            brushSphere.localScale = Vector3.one * radius;
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            radius -= 1;
            brushSphere.localScale = Vector3.one * radius;
        }

        if (Input.GetKey(KeyCode.Alpha1))
        {
            mode = BrushMode.Empty;
        } else if (Input.GetKey(KeyCode.Alpha2))
        {
            mode = BrushMode.Neutral;
        } else if (Input.GetKey(KeyCode.Alpha3))
        {
            mode = BrushMode.Filled;
        }

        Mathf.Clamp(radius, 0, 3);

    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("collided with voxel");
        if(other.tag == "Voxel" && mode != BrushMode.Neutral)
        {
                if (onVoxelCollided != null)
                {
                    onVoxelCollided(other.transform.position, mode);
                }
            
                     
        }

    }

}
