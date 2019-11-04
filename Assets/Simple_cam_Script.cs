using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simple_cam_Script : MonoBehaviour {

    float a =200,b=240,c=200;
    Vector3 Cube_centre;
	// Use this for initialization
	void Start () {
        Cube_centre = new Vector3(a/2,b/2,c/2);
	}
	
	// Update is called once per frame
	void Update () {
        transform.forward = Cube_centre - transform.position;
	}
}
