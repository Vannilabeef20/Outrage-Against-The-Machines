using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    public static class AngleSceneCamera
    {
        static bool active;
        static bool previousOrtho;
        static Quaternion previousRotation;

        static AngleSceneCamera()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        static void OnSceneGUI(SceneView sceneView)
        {
            Handles.BeginGUI();

            // Choose a rectangle in the top bar region.
            var rect = new Rect(5, 5, 60, 20); // x,y,w,h in pixels
            if (GUI.Button(rect, "2.5D"))
            {
                active = sceneView.orthographic && sceneView.rotation == Quaternion.Euler(45, 0, 0);

                if (active)
                {
                    // Example: snap to a custom orthographic view
                    sceneView.orthographic = previousOrtho;
                    sceneView.rotation = previousRotation;
                    sceneView.Repaint();
                }
                else
                {
                    // Example: snap to a custom orthographic view
                    previousOrtho = sceneView.orthographic;
                    sceneView.orthographic = true;
                    previousRotation = sceneView.rotation;
                    sceneView.rotation = Quaternion.Euler(45, 0, 0);
                    sceneView.Repaint();
                }
            }

            Handles.EndGUI();
        }
    }
}