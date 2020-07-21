using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PointAndClick
{
    public class Node
    {
        public Vector3 Position;
        public Node(Vector3 position)
        {
            Position = position;
        }
    }
    public class Graph : MonoBehaviour
    {
        public List<Polygon> Polygons = new List<Polygon>();

        private List<Vertex> _concaveVertices = new List<Vertex>();

        // Start is called before the first frame update
        void Start()
        {
            bool firstPoly = true;
            for (int i = 0; i < Polygons.Count; i++)
            {
                if (Polygons[i].Points.Count > 2)
                {
                    for (int j = 0; j < Polygons[i].Points.Count; j++)
                    {
                        if (Helpers.IsVertexConcave(Polygons[i].Points, j) == firstPoly)
                        {
                            _concaveVertices.Add(Polygons[i].Points[j]);
                        }
                    }
                }

                firstPoly = false;
            }
        }


        private bool IsInLineOfSight(Vertex start, Vertex end)
        {
            float epsilon = 0.5f;

            // if start or end is outside of polygon
            if (!Polygons[0].IsPointInPolygon(start.Position) || !Polygons[0].IsPointInPolygon(end.Position))
                return false;

            // if start and end are the same or too close to eachother
            if (Vector2.Distance(start.Position, end.Position) < epsilon)
                return false;

            // not in LOS if any edge is internsected by the start-end line segment
            bool isInSight = true;
            foreach (var polygon in Polygons)
            {
                for (int i = 0; i < polygon.Points.Count; i++)
                {
                    Vector2 v1 = polygon.Points[i].Position;
                    Vector2 v2 = polygon.Points[Helpers.ClampListIndex(i + 1, polygon.Points.Count)].Position;
                    if (Helpers.LineSegmentsCross(start.Position, end.Position, v1, v2))
                    {
                        if (Helpers.DistanceToSegment(start.Position, v1, v2) > 0.5f && Helpers.DistanceToSegment(end.Position, v1, v2) > 0.5f)
                        {
                            return false;
                        }
                    }
                }
            }

            //Middle point in segment determines if in LOS or not
            Vector2 v = (start.Position + end.Position) / 2.0f;
            bool isInside = Polygons[0].IsPointInPolygon(v);
            for (int i = 1; i < Polygons.Count; i++)
            {
                if (Polygons[i].IsPointInPolygon(v, false))
                {
                    return false;
                }
            }
            return true;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

