using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PointAndClick
{
    public class Path : MonoBehaviour
    {
        [SerializeField] private bool _debugRenderOn = false;

        public Polygon WalkablePoly = null;
        public List<Polygon> ObstaclePolygons = new List<Polygon>();

        private void Update()
        {
            if (_debugRenderOn && Application.isPlaying && Application.isEditor)
            {
                WalkablePoly.DrawPolygon(Color.green);
                foreach (var poly in ObstaclePolygons)
                {
                    poly.DrawPolygon(new Color(1.0f, 1.0f, 0.0f));
                }
            }
        }


    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Path))]
    public class PathEditor : Editor
    {
        Path _path;
        static List<Polygon> _polygons;

        private void OnEnable()
        {
            _path = (Path)target;
            _polygons = new List<Polygon>(_path.ObstaclePolygons);
            _polygons.Insert(0, _path.WalkablePoly);
        }

        [DrawGizmo(GizmoType.NotInSelectionHierarchy)]
        static private void DrawPolys(Path path, GizmoType gizmoType)
        {
            try
            {
                for (int i = 0; i < _polygons.Count; i++)
                {
                    Handles.color = i == 0 ? Color.green : Color.red;
                    var poly = _polygons[i];
                    for (int j = 0; j < poly.Points.Count; j++)
                    {
                        Handles.DrawLine(poly.Points[j], poly.Points[(j + 1) % poly.Points.Count]);
                    }
                }
            }
            catch { }
        }

        private void OnSceneGUI()
        {
            try
            {
                DrawPolys(_path, GizmoType.NotInSelectionHierarchy);
            }
            catch
            {

            }
        }

        private void OnPreviewGUI()
        {

        }
    }
#endif
}

