using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JumpAbility : Ability {

    public float speed = 0.01f;
    public float delay;
    
	// Use this for initialization
	void Start () {
        Destroy(gameObject, delay);
        
    }
	
	// Update is called once per frame
	void Update () {
        transform.position += Vector3.up * speed;
    }

  
}
