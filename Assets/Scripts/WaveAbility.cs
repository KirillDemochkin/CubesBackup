using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveAbility : Ability {

    // Use this for initialization
    public float speed;
    public float delay;
    Vector3 expander;
    
   

    void Start () {
        expander = new Vector3(1, 1, 0.1f);
        
        Destroy(gameObject, delay);
        
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale += expander * speed;
       
        
        
    }

   

    private void OnTriggerEnter(Collider other)
    {
        
        //Debug.Log("collided with voxel");
        if (other.tag == "Voxel" && mode != BrushMode.Neutral)
        {      
            callOnVoxelCollided(other.transform.position, mode);
        } 
    }
}
