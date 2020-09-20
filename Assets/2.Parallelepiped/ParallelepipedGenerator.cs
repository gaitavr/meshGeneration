using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ParallelepipedGenerator : MonoBehaviour
{
    [SerializeField]
    private int _xSize, _ySize, _zSize;

    private Vector3[] _vertices;
    private Vector3[] _normals;
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
        SetNormals();
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
        var trianglesFront = new int[_xSize * _ySize * 6];
        var trianglesBack = new int[_xSize * _ySize * 6];
        var trianglesLeft = new int[_zSize * _ySize * 6];
        var trianglesRight = new int[_zSize * _ySize * 6];
        var trianglesBottom = new int[_xSize * _zSize * 6];
        var trianglesTop = new int[_xSize * _zSize * 6];
        
        var loop = (_xSize + _zSize) * 2;
        int vi = 0, tFront = 0, tBack = 0, tLeft = 0, tRight = 0, tBottom = 0, tTop = 0;
        for (int y = 0; y < _ySize; y++, vi++)
        {
            for (int x = 0; x < _xSize; x++, vi++)
            {
                tFront = SetCell(trianglesFront, tFront, vi, vi + loop,
                    vi + 1, vi + loop + 1);
            }

            for (int z = 0; z < _zSize; z++, vi++)
            {
                tRight = SetCell(trianglesRight, tRight, vi, vi + loop,
                    vi + 1, vi + loop + 1);
            }
            
            for (int x = _xSize; x > 0; x--, vi++)
            {
                tBack = SetCell(trianglesBack, tBack, vi, vi + loop,
                    vi + 1, vi + loop + 1);
            }
            
            for (int z = _zSize; z > 1; z--, vi++)
            {
                tLeft = SetCell(trianglesLeft, tLeft, vi, vi + loop,
                    vi + 1, vi + loop + 1);
            }
            tLeft = SetCell(trianglesLeft, tLeft, vi, vi + loop, 
                vi - loop + 1, vi + 1);
        }

        SetTop(trianglesTop, tTop, loop);
        SetBottom(trianglesBottom, tBottom, loop);

        _mesh.subMeshCount = 6;
        _mesh.SetTriangles(trianglesFront, 0);
        _mesh.SetTriangles(trianglesBack, 1);
        _mesh.SetTriangles(trianglesLeft, 2);
        _mesh.SetTriangles(trianglesRight, 3);
        _mesh.SetTriangles(trianglesBottom, 4);
        _mesh.SetTriangles(trianglesTop, 5);
        //_mesh.triangles = triangles;
    }

    private int SetTop(int[] triangles, int ti, int loop)
    {
        var v_out_right = loop * _ySize;
        var v_out_left = loop * (_ySize + 1) - 1;
        var v_inner = v_out_left + 1;
        
        //First row
        for (int x = 0; x < _xSize - 1; x++, v_out_right++) 
        {
            ti = SetCell(triangles, ti, v_out_right, v_out_right + loop - 1, 
                v_out_right + 1, v_out_right + loop);
        }
        ti = SetCell(triangles, ti, v_out_right, v_out_right + loop - 1,
            v_out_right + 1, v_out_right + 2);
        
        //Inner rows
        v_out_right += 2;
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
        v_out_left--;
        v_out_right++;
        ti = SetCell(triangles, ti, v_out_left + 1, v_out_left, 
            v_inner, v_out_left - 1);
    
        for (int x = 1; x < _xSize - 1; x++, v_out_left--, v_inner++)
        {
            ti = SetCell(triangles, ti, v_inner, v_out_left - 1, 
                v_inner + 1, v_out_left - 2);
        }
        
        ti = SetCell(triangles, ti, v_inner, v_out_right + 1, 
            v_out_right - 1, v_out_right);
        
        return ti;
    }

    private int SetBottom(int[] triangles, int ti, int loop)
    {
        var v_out_right = 0;
        var v_out_left = loop - 1;
        var innerStart = _vertices.Length - (_zSize - 1) * (_xSize - 1);
        var v_inner = innerStart;
        
        //First row
        ti = SetCell(triangles, ti, v_out_left, v_out_right, 
            v_inner, v_out_right + 1);
        v_out_right++;

        for (int x = 1; x < _xSize - 1; x++, v_out_right++, v_inner++) 
        {
            ti = SetCell(triangles, ti, v_inner, v_out_right, 
                v_inner + 1, v_out_right + 1);
        }
        
        ti = SetCell(triangles, ti, v_inner, v_out_right,
            v_out_right + 2, v_out_right + 1);

        //Inner rows
        v_inner = innerStart;
        v_out_right += 2;

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
    
        for (int x = 1; x < _xSize - 1; x++, v_out_left--, v_inner++)
        {
            ti = SetCell(triangles, ti, v_out_left - 1, v_inner, 
                v_out_left - 2, v_inner + 1);
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

    private void SetNormals()
    {
        _normals = new Vector3[_vertices.Length];
        var centerPoint = new Vector3(_xSize/2f, _ySize/2f, _zSize/2f);
        for (int i = 0; i < _vertices.Length; i++)
        {
            _normals[i] = (_vertices[i] - centerPoint).normalized;
        }
        _mesh.normals = _normals;
    }

    private void OnDrawGizmos()
    {
        if(_vertices == null)
            return;
        Gizmos.color = Color.cyan;
        foreach (var v in _vertices)
        {
            Gizmos.DrawCube(v, Vector3.one * 0.15f);
        }
    }
}