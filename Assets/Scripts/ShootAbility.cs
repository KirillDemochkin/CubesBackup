using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAbility : Ability {

    public float speed = 0.01f;
    public float delay;
    // Use this for initialization
    void Start()
    {
        
        Destroy(gameObject, delay);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.TransformDirection(Vector3.forward) * speed;
    }
}
