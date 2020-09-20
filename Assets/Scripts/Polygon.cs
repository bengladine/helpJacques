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
        [SerializeField, HideInInspector]
        public List<Vector2> Points = new List<Vector2>();

        public Color ControlCol = Color.red;
        public Color SegmentCol = Color.green;
        public Color SelectedSegmentCol = Color.yellow;
        public float ControlPointDiameter = 0.075f;
        public bool displayControlPoints = true;

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

        public bool IsPointInPolygon(Vector2 point, bool toleranceOutside = true)
        {
            bool isInside = false;
            // if polygon has less than three points it's always outside
            if (Points.Count < 3)
                return false;

            Vector2 oldPoint = Points[Points.Count - 1];
            float oldSqDist = Helpers.DistanceSquared(oldPoint, point);

            for (int i = 0; i < Points.Count; i++)
            {
                Vector2 newPoint = Points[i];
                float newSqDist = Helpers.DistanceSquared(newPoint, point);

                if (oldSqDist + newSqDist + 2.0f * Mathf.Sqrt(oldSqDist * newSqDist) - Helpers.DistanceSquared(newPoint, oldPoint) < float.Epsilon)
                    return toleranceOutside;

                Vector2 left;
                Vector2 right;

                if (newPoint.x > oldPoint.x)
                {
                    left = oldPoint;
                    right = newPoint;
                }
                else
                {
                    left = newPoint;
                    right = oldPoint;
                }

                if (left.x < point.x && point.x <= right.x && (point.y - left.y) * (right.x - left.x) < (right.y - left.y) * (point.x - left.x))
                {
                    isInside = !isInside;
                }

                oldPoint = newPoint;
                oldSqDist = newSqDist;
            }

            return isInside;
        }

        public Vector2 GetClosestPointOnEdge(Vector2 p)
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

            if (u < 0) return new Vector2(p1.x, p1.y);
            else if (u > 1) return new Vector2(p2.x, p2.y);

            float xu = p1.x + u * (p2.x - p1.x);
            float yu = p1.y + u * (p2.y - p1.y);

            return new Vector2(xu, yu);
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

        float _segmentSelectDistanceThreshold;
        int selectedSegmentIndex = -1;

        private void OnEnable()
        {
            _polygonCreator = (Polygon)target;
            _points = _polygonCreator.Points;
            _segmentSelectDistanceThreshold = 0.05f;
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
            serializedObject.ApplyModifiedProperties();

            Draw();
            Selection.activeGameObject = _polygonCreator.gameObject;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            SerializedProperty vertices = this.serializedObject.FindProperty("Points");
            EditorGUILayout.PropertyField(vertices);
            if (vertices.isExpanded)
            {
                EditorGUIUtility.labelWidth = 65;
                EditorGUI.indentLevel += 1;
                for (int i = 0; i < vertices.arraySize; i++)
                {
                    SerializedProperty vertexRef = vertices.GetArrayElementAtIndex(i);
                    SerializedProperty position = vertexRef.FindPropertyRelative("Position");
                    EditorGUILayout.PropertyField(position, new GUIContent($"Point {i}: "));
                }
                EditorGUI.indentLevel -= 1;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif