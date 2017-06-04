using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed;
    // Use this for initialization
    public SmoothMouseLook mouse;
    private Rigidbody rb;
   
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        /*if (Input.GetKey(KeyCode.Q))
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }*/
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * speed, ForceMode.Impulse);
        }

        

        if (Input.GetKey(KeyCode.S))
        {
            
            rb.AddForce(transform.TransformDirection(Vector3.back) * speed * Time.deltaTime, ForceMode.Impulse);
            //transform.Translate(transform.TransformDirection(Vector3.back) * speed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.TransformDirection(Vector3.forward) * speed * Time.deltaTime, ForceMode.Impulse);
            //transform.Translate(transform.TransformDirection(Vector3.forward) * speed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(transform.TransformDirection(Vector3.left) * Time.deltaTime, ForceMode.Impulse);
            //transform.Translate(transform.TransformDirection(Vector3.left) * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(transform.TransformDirection(Vector3.right) * speed * Time.deltaTime, ForceMode.Impulse);
            //transform.Translate(transform.TransformDirection(Vector3.right) * speed * Time.deltaTime, Space.World);
           
        }

    }
}
