using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAbilitty : Ability {

    public float speed;
    public float delay;
    
    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, delay);
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.localScale += Vector3.up * speed;
    }
}
