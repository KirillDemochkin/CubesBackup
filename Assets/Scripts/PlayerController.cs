using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed;
    // Use this for initialization
   
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }

        

        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(transform.TransformDirection(Vector3.back) * speed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(transform.TransformDirection(Vector3.forward) * speed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(transform.TransformDirection(Vector3.left) * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(transform.TransformDirection(Vector3.right) * speed * Time.deltaTime, Space.World);
           
        }

    }
}
