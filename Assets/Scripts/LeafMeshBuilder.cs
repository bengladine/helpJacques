//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[RequireComponent(typeof(MeshFilter))]
//public class LeafMeshBuilder : MonoBehaviour
//{
//    Mesh _mesh;

//    Vector3[] _vertices;
//    Vector2[] _UV;

//    private LeafGrowth _leafGrowth;

//    private void Start()
//    {
//        _leafGrowth = GetComponent<LeafGrowth>();

//        EarClipping earClipper = new EarClipping();
//        _vertices = _leafGrowth.GetVertices().ToArray();
//        _vertices = MirrorMesh(_vertices);
//        List<Triangle> triangles = earClipper.TriangulateConcavePolygon(new List<Vector3>(_vertices));

//        List<int> indices = new List<int>();
//        foreach (var triangle in triangles)
//        {
//            indices.Add(triangle.V1.Index);
//            indices.Add(triangle.V2.Index);
//            indices.Add(triangle.V3.Index);
//        }

//        _mesh = new Mesh();
//        _mesh.Clear();
//        _mesh.vertices = _vertices;
//        _mesh.triangles = indices.ToArray();
//        //_mesh.RecalculateNormals();
//        //_mesh.RecalculateBounds();
//        //_mesh.uv = _UV;
//        GetComponent<MeshFilter>().mesh = _mesh;
//    }

//    private Vector3[] MirrorMesh(Vector3[] vertices)
//    {
//        List<Vector3> mirroredMesh = new List<Vector3>(vertices);
//        for (int i = vertices.Length - 2; i > 0 - 1; i--)
//        {
//            mirroredMesh.Add(new Vector3(-vertices[i].x, vertices[i].y, vertices[i].z));
//        }

//        return mirroredMesh.ToArray();
//    }
//}
