using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample_mesh : MonoBehaviour {
    private Renderer rend;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        rend = GetComponent<Renderer>();
        Vector3[] newVertices = new Vector3[6];
        newVertices[0].Set(0,0,0);
        newVertices[1].Set(10, 10, 1);
        newVertices[2].Set(20, 10, 2);
        newVertices[3].Set(30, 0, 3);
        newVertices[4].Set(20, -10, 4);
        newVertices[5].Set(10, -10, 5);


        int[] newTriangles = new int[12];
        newTriangles[0] = 0;
        newTriangles[1] = 1;
        newTriangles[2] = 2;
        newTriangles[3] = 0;
        newTriangles[4] = 2;
        newTriangles[5] = 3;
        newTriangles[6] = 0;
        newTriangles[7] = 3;
        newTriangles[8] = 4;
        newTriangles[9] = 0;
        newTriangles[10] = 4;
        newTriangles[11] = 5;
        //GetComponent<MeshFilter>().mesh.Clear();
        mesh.vertices = newVertices;

        mesh.triangles = newTriangles;
	}
}
