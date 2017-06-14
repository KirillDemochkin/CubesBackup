using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveAbility : Ability {

    // Use this for initialization
    public float speed;
    public float delay;
    Vector3 expander;

    void Start () {
        expander = new Vector3(1, 1, 0);
        Destroy(gameObject, delay);
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale += expander * speed;
        transform.position += Vector3.down * speed/7.5f;
    }

    private void OnTriggerExit(Collider other)
    {
        
        if (other.tag == "Voxel" )
        {
            callOnVoxelCollided(other.transform.position, BrushMode.Empty);
            
        }
    }
}
