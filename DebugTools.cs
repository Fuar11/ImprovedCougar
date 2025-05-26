using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ImprovedCougar
{
    internal class DebugTools
    {

        public static void CreateDebugMarker(Vector3 position, Color color, float duration)
        {
            if (Settings.CustomSettings.settings.debugPoints == false) return;

            // Create the sphere marker
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = "DebugSphere";
            marker.transform.position = position;
            marker.transform.localScale = Vector3.one * 0.3f;

            // Optional: Set color
            var renderer = marker.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = color;
            }

            // Create a vertical line (cylinder) going up into the sky
            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            line.transform.position = position + Vector3.up * 2.5f; // center the cylinder
            line.transform.localScale = new Vector3(0.05f, 2.5f, 0.05f); // thin tall cylinder
            line.transform.rotation = Quaternion.identity;

            var lineRenderer = line.GetComponent<Renderer>();
            if (lineRenderer != null)
            {
                lineRenderer.material = new Material(Shader.Find("Standard"));
                lineRenderer.material.color = color;
            }

            // Destroy both after duration
            UnityEngine.Object.Destroy(marker, duration);
            UnityEngine.Object.Destroy(line, duration);
        }

        public static void HighlightColliderBounds(Collider col)
        {

            if (Settings.CustomSettings.settings.debugColliders == false) return;

            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.transform.position = col.bounds.center;
            box.transform.localScale = col.bounds.size;
            box.GetComponent<Renderer>().material.color = new Color(1f, 1f, 0f, 0.25f); // semi-transparent yellow

            // Destroy after a few seconds
            UnityEngine.Object.Destroy(box, 1f);
        }

        public static void DrawRay(Vector3 start, Vector3 direction, float length, Color color, float duration = 1f)
        {

            //if(Settings.CustomSettings.settings.debugRays == false) return;

            GameObject go = new GameObject("DebugRay");
            LineRenderer lr = go.AddComponent<LineRenderer>();

            lr.positionCount = 2;
            lr.SetPosition(0, start);
            lr.SetPosition(1, start + direction.normalized * length);

            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = lr.endColor = color;
            lr.startWidth = lr.endWidth = 0.05f;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lr.receiveShadows = false;
            lr.useWorldSpace = true;

            GameObject.Destroy(go, duration);
        }
    }
}
