using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ParallelepipedGenerator : MonoBehaviour
{
    [SerializeField]
    private int _xSize, _ySize, _zSize;

    private Vector3[] _vertices;
    private Mesh _mesh;

    private void Start()
    {
        Generate();
    }
    
    private void Generate()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
        _mesh.name = "Parallelepiped";

        //Generate logic
    }
}