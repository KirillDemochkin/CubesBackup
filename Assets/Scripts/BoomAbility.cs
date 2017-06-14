using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomAbility : Ability {

    public float speed;
    public float delay;
    public float counter;
    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, delay*2);
    }

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;
        if (counter < delay/2)
        {
            transform.position += transform.TransformDirection(Vector3.forward) * speed/2;
        } else
        {
            transform.localScale += Vector3.one * 0.1f;
        }
    }
}
