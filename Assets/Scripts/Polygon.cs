using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class Polygon : MonoBehaviour
{
    [SerializeField]
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
    }
}
#endif