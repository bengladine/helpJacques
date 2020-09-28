using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PointAndClick
{
    [System.Serializable]
    public class Polygon : MonoBehaviour
    {
        [HideInInspector]
        public List<Vector2> Points = new List<Vector2>();

        public Color ControlCol = Color.red;
        public Color SegmentCol = Color.green;
        public Color SelectedSegmentCol = Color.yellow;
        public float ControlPointDiameter = 0.075f;
        public bool displayControlPoints = true;

        [HideInInspector]
        public Vector4 BoundingBox = new Vector4();

        private void Awake()
        {
            BoundingBox = CreateBoundingBox();
        }

        public Vector4 CreateBoundingBox()
        {
            float mostLeft = Mathf.Infinity;
            float mostRight = -Mathf.Infinity;
            float mostUp = -Mathf.Infinity;
            float mostDown = Mathf.Infinity;

            foreach (var point in Points)
            {
                if (point.x < mostLeft)
                    mostLeft = point.x;
                if (point.x > mostRight)
                    mostRight = point.x;
                if (point.y < mostDown)
                    mostDown = point.y;
                if (point.y > mostUp)
                    mostUp = point.y;
            }

            return new Vector4(mostLeft, mostDown, mostRight, mostUp);
        }

        public void SplitSegment(Vector2 anchorPos, int segmentIndex)
        {
            Points.Insert(segmentIndex + 1, anchorPos);
        }

        public void DeletePoint(int idx)
        {
            Points.Remove(Points[idx]);
        }

        public void MovePoint(int index, Vector2 pos)
        {
            Points[index] = pos;
        }

        // source https://stackoverflow.com/questions/217578/how-can-i-determine-whether-a-2d-point-is-within-a-polygon
        public bool IsPointInPolygon(Vector2 point)
        {
            int intersections = 0;

            Vector2 rayv1 = new Vector2(BoundingBox.x - 0.01f, point.y);
            Vector2 rayv2 = point;


            for (int i = 0; i < Points.Count; i++)
            {
                var p1 = Points[i];
                var p2 = Points[(i + 1) % Points.Count];

                if (Helpers.AreIntersecting(rayv1.x, rayv1.y, rayv2.x, rayv2.y, p1.x, p1.y, p2.x, p2.y))
                    intersections++;
            }

            if ((intersections & 1) == 1)
                return true;
            return false;
        }

        public Vector2 GetClosestPointOnEdge(Vector2 p, bool pointMustBeOut)
        {
            int vi1 = -1;
            int vi2 = -1;

            float minDist = Mathf.Infinity;

            for (int i = 0; i < Points.Count; i++)
            {
                int j = Helpers.ClampListIndex(i + 1, Points.Count);
                var dist = Helpers.DistanceToSegment(p, Points[i], Points[j]);
                if (dist < minDist)
                {
                    minDist = dist;
                    vi1 = i;
                    vi2 = j;
                }
            }

            var p1 = Points[vi1];
            var p2 = Points[vi2];

            float u = (((p.x - p1.x) * (p2.x - p1.x)) + ((p.y - p1.y) * (p2.y - p1.y))) / (((p2.x - p1.x) * (p2.x - p1.x)) + ((p2.y - p1.y) * (p2.y - p1.y)));

            Vector2 newPoint = new Vector2();

            if (u < 0)
            {
                newPoint = new Vector2(p1.x, p1.y);
            }
            else if (u > 1)
            {
                newPoint = new Vector2(p2.x, p2.y);
            }
            else
            {
                float xu = p1.x + u * (p2.x - p1.x);
                float yu = p1.y + u * (p2.y - p1.y);
                newPoint = new Vector2(xu, yu);
            }

            float angle = 0.0f;
            var originalValue = newPoint;
            float tolerance = 0.001f;
            int counter = 0;

            // sometimes the point can still be in the polygon when it's on an edge, we want to avoid that so we move the point in a radial matter.

            while (IsPointInPolygon(newPoint) == pointMustBeOut)
            {
                newPoint = originalValue;
                newPoint.x += Mathf.Cos(angle) * tolerance;
                newPoint.y += Mathf.Sin(angle) * tolerance;
                angle += Mathf.PI / 2.0f;
                counter++;
                if (counter % 4 == 0)
                    tolerance *= 2;
            }

            return newPoint;
        }


        // debug function
        public void DrawPolygon(Color color = default)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                Debug.DrawLine(Points[i], Points[(i + 1) % Points.Count], color);
            }
        }

    }


#if UNITY_EDITOR
    [CustomEditor(typeof(Polygon))]
    public class PolygonEditor : Editor
    {
        Polygon _polygonCreator;
        List<Vector2> _points;
        Vector4 _boundingBox;
        float _segmentSelectDistanceThreshold;
        int selectedSegmentIndex = -1;

        bool showPoints = false;

        private void OnEnable()
        {
            _polygonCreator = (Polygon)target;
            _points = _polygonCreator.Points;
            _segmentSelectDistanceThreshold = 0.05f;
            _boundingBox = _polygonCreator.BoundingBox;
        }

        private void Input()
        {
            Event guiEvent = Event.current;
            Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;
            // adding points
            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
            {
                if (selectedSegmentIndex != -1)
                {
                    Undo.RecordObject(_polygonCreator, "Split Point");
                    _polygonCreator.SplitSegment(mousePos, selectedSegmentIndex);
                }
                else
                {
                    Undo.RecordObject(_polygonCreator, "Add Point");
                    _points.Add(mousePos);
                }
            }

            // deleting points on segment
            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1)
            {
                int idxClosestAnchor = -1;
                float closestDst = _segmentSelectDistanceThreshold;
                for (int i = 0; i < _points.Count; i++)
                {
                    float dst = Vector2.Distance(mousePos, _points[i]);
                    if (dst < closestDst)
                    {
                        closestDst = dst;
                        idxClosestAnchor = i;
                    }
                }

                if (idxClosestAnchor != -1)
                {
                    Undo.RecordObject(_polygonCreator, "Delete Point");
                    _polygonCreator.DeletePoint(idxClosestAnchor);
                }
            }


            // inserting points on a segment

            if (guiEvent.type == EventType.MouseMove)
            {
                float minDstToSegment = _segmentSelectDistanceThreshold;
                int newSelectedSegmentIdx = -1;

                for (int i = 0; i < _points.Count; i++)
                {
                    float currentClstDst = HandleUtility.DistancePointToLineSegment(mousePos, _points[i], _points[Helpers.ClampListIndex(i + 1, _points.Count)]);
                    if (currentClstDst < minDstToSegment)
                    {
                        minDstToSegment = currentClstDst;
                        newSelectedSegmentIdx = i;
                    }
                }

                if (newSelectedSegmentIdx != selectedSegmentIndex)
                {
                    selectedSegmentIndex = newSelectedSegmentIdx;
                    HandleUtility.Repaint();
                }
            }

        }

        private void Draw()
        {
            for (int i = 0; i < _points.Count; i++)
            {
                // draws points
                Color segmentCol = (i == selectedSegmentIndex && Event.current.shift) ? _polygonCreator.SelectedSegmentCol : _polygonCreator.SegmentCol;

                if (_polygonCreator.displayControlPoints)
                {
                    Handles.color = _polygonCreator.ControlCol;
                    Vector2 newPos = Handles.FreeMoveHandle(_points[i], Quaternion.identity, _polygonCreator.ControlPointDiameter, Vector2.zero, Handles.CylinderHandleCap);
                    if (_points[i] != newPos)
                    {
                        Undo.RecordObject(_polygonCreator, "Move Points");
                        _polygonCreator.MovePoint(i, newPos);
                    }
                }
                // draw lines
                Handles.color = segmentCol;
                Handles.DrawLine(_points[i], _points[Helpers.ClampListIndex(i + 1, _points.Count)]);
            }
        }

        private void OnSceneGUI()
        {
            // detect if LMB is pressed down
            Input();
            UpdateBoundingBox();
            serializedObject.ApplyModifiedProperties();

            Draw();
            Selection.activeGameObject = _polygonCreator.gameObject;
        }

        private void UpdateBoundingBox()
        {
            _boundingBox = _polygonCreator.CreateBoundingBox();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            SerializedProperty vertices = this.serializedObject.FindProperty("Points");
         //   EditorGUILayout.PropertyField(vertices);
            showPoints = EditorGUILayout.Foldout(showPoints, "Points");
            if (showPoints)
            {
                EditorGUIUtility.labelWidth = 65;
                EditorGUI.indentLevel += 1;
                for (int i = 0; i < vertices.arraySize; i++)
                {
                    SerializedProperty vertexRef = vertices.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(vertexRef, new GUIContent($"Point {i}: "));
                }
                EditorGUI.indentLevel -= 1;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif