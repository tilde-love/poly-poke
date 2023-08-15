using System;
using UnityEngine;

namespace _Poly.Code
{
    public class CollideAndSlide : MonoBehaviour
    {
        [SerializeField] private float skinWidth = 0.01f;
        new Collider collider;

        private void Awake()
        {
            collider = GetComponent<Collider>();
        }

        void Update()
        {
            Bounds bounds = collider.bounds;
            bounds.Expand(-2 * skinWidth);
        }

        public static Vector3 ProjectCollideAndSlide(Vector3 position, Vector3 velocity)
        {
            var ray = new Ray(position, velocity.normalized);
            float distance = velocity.magnitude;
            
            while (distance > 0f)
            {
                if (Physics.Raycast(ray, out var hit, distance))
                {
                    var normal = hit.normal;
                    var tangent = Vector3.Cross(normal, Vector3.up);
                    var slide = Vector3.Cross(normal, tangent);
                    var dot = Vector3.Dot(velocity, slide);
                    
                    velocity = slide * dot;
                    distance -= hit.distance;
                    
                    ray.origin = hit.point + velocity.normalized * 0.001f;
                    ray.direction = velocity.normalized;
                }
                else
                {
                    break;                     
                }
            }

            return ray.origin + ray.direction * distance;
        }  
    }
}