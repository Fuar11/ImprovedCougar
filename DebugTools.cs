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

        public static void CreateDebugMarker(Vector3 position, Color color)
        {

            if (Settings.CustomSettings.settings.debug == false) return;

            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.transform.position = position;
            marker.transform.localScale = Vector3.one * 0.3f;

            // Optional: Set color
            var renderer = marker.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = color;
            }

            // Auto-destroy after a few seconds
            UnityEngine.Object.Destroy(marker, 10f);
        }

        public static void HighlightColliderBounds(Collider col)
        {

            if (Settings.CustomSettings.settings.debug == false) return;

            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.transform.position = col.bounds.center;
            box.transform.localScale = col.bounds.size;
            box.GetComponent<Renderer>().material.color = new Color(1f, 1f, 0f, 0.25f); // semi-transparent yellow

            // Destroy after a few seconds
            UnityEngine.Object.Destroy(box, 10f);
        }

    }
}
