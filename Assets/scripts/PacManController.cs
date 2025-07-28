using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacManController : MonoBehaviour {
    //public Rigidbody2D rigidbody;
    public float speed;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Rigidbody2D rigidbody = gameObject.GetComponent<Rigidbody2D>();
        float velHorz = (Input.GetKey(KeyCode.A) ? -1 : 0) + (Input.GetKey(KeyCode.D) ?  1 : 0);
        float velVert = (Input.GetKey(KeyCode.W) ?  1 : 0) + (Input.GetKey(KeyCode.S) ? -1 : 0);
        rigidbody.velocity += new Vector2(velHorz, velVert) * speed;
    }
}
