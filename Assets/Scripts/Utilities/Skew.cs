using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[AddComponentMenu("Unity Tools/Skew")]
public class Skew : MonoBehaviour
{
    //--Variables--
    //-Public-
    //Parameters
    public Vector3 skewDirection;
    public Vector3 skewPosition;
    //-Private-
    //References
    Mesh mesh;
    Vector3[] startVertices;
    Vector3[] vertices;

    //--Methods--
    void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        startVertices = mesh.vertices;
        vertices = startVertices;
    }
    void Update()
    {
        mesh.vertices = startVertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = startVertices[i] + (skewDirection * startVertices[i].y);
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        vertices = startVertices;
    }
}