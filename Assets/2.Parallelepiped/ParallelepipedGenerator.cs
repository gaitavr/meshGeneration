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
        SetVertices();
    }

    private void SetVertices()
    {
        var cornerCount = 8;
        var edgeCount = (_xSize + _ySize + _zSize - 3) * 4;
        var faceCount = (_xSize - 1) * (_ySize - 1) + (_xSize - 1) * (_zSize - 1) +
            (_ySize - 1) * (_zSize - 1);
        faceCount *= 2;
        _vertices = new Vector3[cornerCount + edgeCount + faceCount];

        var v = 0;
        for (int y = 0; y <= _ySize; y++)
        {
            for (int x = 0; x <= _xSize; x++)
            {
                _vertices[v++] = new Vector3(x, y, 0);
            }

            for (int z = 1; z <= _zSize; z++)
            {
                _vertices[v++] = new Vector3(_xSize, y, z);
            }

            for (int x = _xSize - 1; x >= 0; x--)
            {
                _vertices[v++] = new Vector3(x, y, _zSize);
            }

            for (int z = _zSize - 1; z < 0; z--)
            {
                _vertices[v++] = new Vector3(0, y, z);
            }
        }

        for (int x = 1; x < _xSize; x++)
        {
            for (int z = 1; z < _zSize; z++)
            {
                _vertices[v++] = new Vector3(x, _ySize, z);
                _vertices[v++] = new Vector3(x, 0, z);
            }
        }
    }
}