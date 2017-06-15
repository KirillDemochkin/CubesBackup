using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType { Boom, Shoot, Wave, Wall, Jump}

[System.Serializable]
public class Ability : MonoBehaviour {

    public AbilityType type;
    public BrushMode mode;
    public event System.Action<Vector3, BrushMode> onVoxelCollided;
    // Use this for initialization
    
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setMode(BrushMode mode)
    {
        this.mode = mode;
    }

    public void callOnVoxelCollided(Vector3 position, BrushMode mode)
    {
        if (onVoxelCollided != null)
        {
            onVoxelCollided(position, mode);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("collided with voxel");
        if (other.tag == "Voxel" && mode != BrushMode.Neutral)
        {
            if (onVoxelCollided != null)
            {
                onVoxelCollided(other.transform.position, mode);
                //Debug.Log("collided");
            }


        } 

    }

}
