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
        SetTriangles();
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

            for (int z = _zSize - 1; z > 0; z--)
            {
                _vertices[v++] = new Vector3(0, y, z);
            }
        }

        for (int z = 1; z < _zSize; z++)
        {
            for (int x = 1; x < _xSize; x++)
            {
                _vertices[v++] = new Vector3(x, _ySize, z);
            }
        }
        
        for (int z = 1; z < _zSize; z++)
        {
            for (int x = 1; x < _xSize; x++)
            {
                _vertices[v++] = new Vector3(x, 0, z);
            }
        }
        
        // for (int x = 1; x < _zSize; x++)
        // {
        //     for (int z = 1; z < _xSize; z++)
        //     {
        //         _vertices[v++] = new Vector3(z, _ySize, z);
        //         _vertices[v++] = new Vector3(z, 0, z);
        //     }
        // }

        _mesh.vertices = _vertices;
    }

    private void SetTriangles()
    {
        var cellCount = _xSize * _ySize + _ySize * _zSize + _zSize * _xSize;
        cellCount *= 2;
        var triangles = new int[cellCount * 6];
        var loop = (_xSize + _zSize) * 2;
        int ti = 0, vi = 0;
        for (int y = 0; y < _ySize; y++, vi++)
        {
            for (int k = 0; k < loop - 1; k++, vi++)
            {
                ti = SetCell(triangles, ti, vi, vi + loop,
                    vi + 1, vi + loop + 1);
            }
            ti = SetCell(triangles, ti, vi, vi + loop, 
                vi - loop + 1, vi + 1);
        }

        ti = SetTop(triangles, ti, loop);
        ti = SetBottom(triangles, ti, loop);
        
        _mesh.triangles = triangles;
    }

    private int SetTop(int[] triangles, int ti, int loop)
    {
        //First row
        var v = loop * _ySize;
        for (int x = 0; x < _xSize - 1; x++, v++) 
        {
            ti = SetCell(triangles, ti, v, v + loop - 1, v + 1, v + loop);
        }
        ti = SetCell(triangles, ti, v, v + loop - 1,v + 1, v + 2);
        
        //Inner rows
        var v_out_left = loop * (_ySize + 1) - 1;
        var v_inner = v_out_left + 1;
        var v_out_right = v + 2;
        
        for (int z = 1; z < _zSize - 1; z++, v_out_left--, v_inner++, v_out_right++)
        {
            ti = SetCell(triangles, ti, v_out_left, v_out_left - 1,
                v_inner, v_inner + _xSize - 1);
            for (int x = 1; x < _xSize - 1; x++, v_inner++)
            {
                ti = SetCell(triangles, ti, v_inner, v_inner + _xSize - 1,
                    v_inner + 1, v_inner + _xSize);
            }
            ti = SetCell(triangles, ti, v_inner, v_inner + _xSize - 1,
                v_out_right, v_out_right + 1);
        }
        
        //Last row
        ti = SetCell(triangles, ti, v_out_left, v_out_left - 1, 
            v_inner, v_out_left - 2);
    
        for (int x = 1; x < _xSize - 1; x++)
        {
            v_out_left--;
            v_inner++;
            ti = SetCell(triangles, ti, v_inner - 1, v_out_left - 1, 
                v_inner, v_out_left - 2);
        }
        
        ti = SetCell(triangles, ti, v_inner, v_out_right + 2, 
            v_out_right, v_out_right + 1);
        
        return ti;
    }

    private int SetBottom(int[] triangles, int ti, int loop)
    {
        //First row
        var v = 2;
        var v_inner = _vertices.Length - (_zSize - 1) * (_xSize - 1);
        
        ti = SetCell(triangles, ti, loop - 1, 0, 
            v_inner, 1);

        for (int x = 1; x < _xSize - 1; x++, v++, v_inner++) 
        {
            ti = SetCell(triangles, ti, v_inner, v - 1, 
                v_inner + 1, v);
        }
        
        ti = SetCell(triangles, ti, v_inner, v - 1,v + 1, v);

        //Inner rows
        var v_out_left = loop - 1;
        v_inner = _vertices.Length - (_zSize - 1) * (_xSize - 1);
        var v_out_right = _xSize + 1;
        
        for (int z = 1; z < _zSize - 1; z++, v_out_left--, v_inner++, v_out_right++)
        {
            ti = SetCell(triangles, ti, v_out_left - 1, v_out_left,
                v_inner + _xSize - 1, v_inner);
            for (int x = 1; x < _xSize - 1; x++, v_inner++)
            {
                ti = SetCell(triangles, ti, v_inner + _xSize - 1, v_inner,
                    v_inner + _xSize, v_inner + 1);
            }
            ti = SetCell(triangles, ti, v_inner + _xSize - 1, v_inner,
                v_out_right + 1, v_out_right);
        }
        
        //Last row
        v_out_left--;
        v_out_right++;
        ti = SetCell(triangles, ti, v_out_left, v_out_left + 1, 
            v_out_left - 1, v_inner);
    
        for (int x = 1; x < _xSize - 1; x++)
        {
            v_out_left--;
            v_inner++;
            ti = SetCell(triangles, ti, v_out_left, v_inner - 1, 
                v_out_left - 1, v_inner);
        }
        
        ti = SetCell(triangles, ti, v_out_right + 1, v_inner, 
            v_out_right, v_out_right - 1);
        
        return ti;
    }

    private int SetCell(int[] triangles, int ti, int v00, int v01, int v10, int v11)
    {
        triangles[ti] = v00;
        triangles[ti + 1] = triangles[ti + 4] = v01;
        triangles[ti + 2] = triangles[ti + 3] = v10;
        triangles[ti + 5] = v11;
        return ti + 6;
    }
}