using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class Mesh_Create : MonoBehaviour
{
    public int no_slices = 100; // Indicates the no of slices to be stacked in Volume Renderer  
    public float dPlaneIncr = 1; // Distance between planes
    private float a =200,b=240,c=200; // The dimensions of the bounding box
    public float max=1000;
    public float min=400;
    Vector4[] vecVertices = new Vector4[8]; // Bounding box vertices
   
    private Renderer rend;

    public Camera cam;
    public float dPlaneStart = 30;  // Starting distance of the plane
    private Texture3D texture;
    private Texture2D texture_map;
    private Vector3 vecView;
    private Vector4 vecView_local;
    private Vector4 vecTranslate;
    public int frontIdx;

	void Start()
    {
        Vector3 vecView = new Vector3(-a, -b, -c); // Initializing the viewing direction
        // Note that if local cam is at (x,y,z) vecTranslate should be (-x,-y,-z)
        Vector4 vecTranslate = new Vector4(-a, -b, -c, 0); // Initializing the point to which the origin needs to be translated
        frontIdx = 0;
        texture = CreateTexture3D(320,320,200);

        rend = GetComponent<Renderer>();
        rend.material.shader = Shader.Find("Unlit/Volume_Render"); // Attach this to Volume_render Shader
        // Initializing the vertices of a cuboid with dimensions a,b,c
        vecVertices[0] = new Vector4(a, b, c, 1);
        vecVertices[1] = new Vector4(a, b, 0, 1);
        vecVertices[2] = new Vector4(a, 0, c, 1);
        vecVertices[3] = new Vector4(0, b, c, 1);
        vecVertices[4] = new Vector4(0, b, 0, 1);
        vecVertices[5] = new Vector4(a, 0, 0, 1);
        vecVertices[6] = new Vector4(0, 0, c, 1);
        vecVertices[7] = new Vector4(0, 0, 0, 1);

        rend.material.SetTexture("_3dTexture", texture); // Passing the texture to the shader
        rend.material.SetTexture("_transfer_function", texture_map);// Passing the transfer function to the shader
        rend.material.SetVectorArray("_vecVertices", vecVertices);// Passing the vertices of the bounding box to the shader

    }
    int GetfrontIdx() // Given the position and direction of tool, get the front index of bounding box
    {
        int frontIdx;   
        float[] distances = new float[8];
        for (int i = 0; i < 8;i++)
        {
            distances[i] = Vector3.Distance(vecVertices[i],this.transform.InverseTransformVector(cam.transform.position));   // Finding the nearest vertex of bounding box
            // This distance is computed by taking positions of cam and bounding box in local coord
        }
        frontIdx = distances.ToList().IndexOf(distances.Min()); // find the min 
        //Debug.Log(frontIdx);
        dPlaneStart = distances[frontIdx]; // This distance is in the local coord system
        //Debug.Log(dPlaneStart);
        return frontIdx;
    }

    Texture3D CreateTexture3D(int w, int h, int d) // Creating a 3d Texture
    {
        FileStream fs = new FileStream("sample_dicom.raw", FileMode.Open);
        BinaryReader reader = new BinaryReader(fs);

        Color[] colorArray = new Color[w * h * d];
        texture = new Texture3D(w, h, d, TextureFormat.RGBA32, true);
        float temp_var;
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                for (int z = 0; z < d; z++)
                {
                    int i = x + (y * w) + (z * w * h);
                    temp_var =reader.ReadSingle()/4493.0f;
                    //temp_var = reader.ReadSingle();
                    colorArray[i] = new Color(temp_var,0.0f,0.0f,1.0f);
                    //Debug.Log(temp_var);
                }
            }
        }
        texture.SetPixels(colorArray);
        texture.Apply();
        return texture;
    }
    //Texture2D Transfer_func_post_classification(int w) 
    //{

    //    Color[] colorArray = new Color[w];
    //    texture_map = new Texture2D(w, 1);
    //    for (int x = 0; x < w; x++)
    //    {
            
    //        if(x<max && x>min)
    //        {
    //            colorArray[x] = new Color(1.0f, 0.0f, 0.0f, 1.0f);
    //        }
    //        else
    //        {
    //            colorArray[x] = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    //        }
    //    }
    //    texture_map.SetPixels(colorArray);
    //    texture_map.Apply();
    //    return texture_map;
    //}
  
    void Update()
    {
        // Initializing the vertices of the bounding box as per dimensions
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        Vector3[] newVertices = new Vector3[6 * no_slices];
        Vector3[] newNormals = new Vector3[6 * no_slices];

        int[] newTriangles = new int[12 * no_slices];

         //Initialize the vertices  and Triangles of polygons
        for (int i = 0; i < no_slices; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                newVertices[i * 6 + j].Set(j, i, 0);
                newNormals[i * 6 + j].Set(-cam.transform.forward.x, -cam.transform.forward.y, -cam.transform.forward.z);
            }
            newTriangles[i * 12 + 0] = i * 6;
            newTriangles[i * 12 + 1] = i * 6 + 1;
            newTriangles[i * 12 + 2] = i * 6 + 2;
            newTriangles[i * 12 + 3] = i * 6;
            newTriangles[i * 12 + 4] = i * 6 + 2;
            newTriangles[i * 12 + 5] = i * 6 + 3;
            newTriangles[i * 12 + 6] = i * 6;
            newTriangles[i * 12 + 7] = i * 6 + 3;
            newTriangles[i * 12 + 8] = i * 6 + 4;
            newTriangles[i * 12 + 9] = i * 6;
            newTriangles[i * 12 + 10] = i * 6 + 4;
            newTriangles[i * 12 + 11] = i * 6 + 5;

        }

        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
        mesh.normals = newNormals;
        rend.material.SetVector("_vecView",this.transform.InverseTransformVector(cam.transform.forward).normalized);

        Vector4 vecTranslate = new Vector4(-this.transform.InverseTransformVector(cam.transform.position).x, -this.transform.InverseTransformVector(cam.transform.position).y, -this.transform.InverseTransformVector(cam.transform.position).z, 0);

        rend.material.SetVector("_vecTranslate", vecTranslate);

        rend.material.SetInt("_frontIdx", GetfrontIdx());

        rend.material.SetFloat("_dPlaneStart", dPlaneStart);

        rend.material.SetFloat("_dPlaneIncr", dPlaneIncr);
       
    }
}
