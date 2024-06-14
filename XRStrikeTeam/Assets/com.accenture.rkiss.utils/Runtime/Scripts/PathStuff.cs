using System.Collections.Generic;
using UnityEngine;

namespace Accenture.rkiss.PathGeneration
{
    public static class PathGenerationFunctions
    {
        /// <summary>
        /// generates a set of waypoints along a cubic Bezier spline defined by a start point, an end point, and two control point modifiers.
        /// </summary>
        /// <param name="startPoint">The starting position of the spline.</param>
        /// <param name="endPoint">The ending position of the spline.</param>
        /// <param name="controlPointModifier1">A vector used to modify the first control point.</param>
        /// <param name="controlPointModifier2">A vector used to modify the second control point.</param>
        /// <param name="controlPointModifier">A scalar value that scales the control point modifiers.</param>
        /// <returns></returns>
        public static Vector3[] GetWaypointsAlongSpline(Vector3 startPoint, Vector3 endPoint, Vector3 controlPointModifier1, Vector3 controlPointModifier2, float controlPointModifier, bool isNoiseEnabled = false, float noiseStrength = 1.0f, int numPts=10)
        {
            List<Vector3> waypoints = new List<Vector3>();

            Vector3 controlPoint1 = startPoint + (endPoint - startPoint) / 3 + (controlPointModifier1 * controlPointModifier);
            Vector3 controlPoint2 = endPoint - (endPoint - startPoint) / 3 + (controlPointModifier2 * controlPointModifier);

            float inc = 1.0f / (float)numPts;
            for (float t = 0; t <= 1; t += inc)
            {
                float x = (float)((1 - t) * (1 - t) * (1 - t) * startPoint.x + 3 * (1 - t) * (1 - t) * t * controlPoint1.x + 3 * (1 - t) * t * t * controlPoint2.x + t * t * t * endPoint.x);
                float y = (float)((1 - t) * (1 - t) * (1 - t) * startPoint.y + 3 * (1 - t) * (1 - t) * t * controlPoint1.y + 3 * (1 - t) * t * t * controlPoint2.y + t * t * t * endPoint.y);
                float z = (float)((1 - t) * (1 - t) * (1 - t) * startPoint.z + 3 * (1 - t) * (1 - t) * t * controlPoint1.z + 3 * (1 - t) * t * t * controlPoint2.z + t * t * t * endPoint.z);

                Vector3 waypoint = new Vector3(x, y, z);

                if (isNoiseEnabled)
                {
                    Vector3 noise = new Vector3(
                        Mathf.PerlinNoise(t * 10, 0) - 0.5f,
                        Mathf.PerlinNoise(t * 10, 1) - 0.5f,
                        Mathf.PerlinNoise(t * 10, 2) - 0.5f
                    ) * noiseStrength;

                    waypoint += noise;
                }

                waypoints.Add(waypoint);
            }

            waypoints.Add(endPoint);

            return waypoints.ToArray();
        }

        /// <summary>
        /// Divides a vector to equal segments.
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="divisionFactor">number of segments to create</param>
        /// <returns>A set of equally divided points along a vector</returns>
        public static Vector3[] DivideVector(Vector3 startPoint, Vector3 endPoint, int divisionFactor, bool isNoiseEnabled = false, float noiseStrength = 1.0f)
        {
            if (divisionFactor < 1)
            {
                throw new System.ArgumentException("Number of divisions must be at least 1.");
            }

            Vector3[] points = new Vector3[divisionFactor + 1];
            Vector3 direction = endPoint - startPoint;

            for (int i = 0; i <= divisionFactor; i++)
            {
                float t = i / (float)divisionFactor;
                Vector3 point = startPoint + t * direction;

                if (isNoiseEnabled)
                {
                    Vector3 noise = new Vector3(
                        Mathf.PerlinNoise(t * 10, 0) - 0.5f,
                        Mathf.PerlinNoise(t * 10, 1) - 0.5f,
                        Mathf.PerlinNoise(t * 10, 2) - 0.5f
                    ) * noiseStrength;

                    point += noise;
                }

                points[i] = point;
            }

            return points;
        }


        public static Vector3[] GenerateSpiralPath(Vector3 startPoint, Vector3 targetPoint, float spiralRadius, int numPoints = 100, bool isNoiseEnabled = false, float noiseStrength = 1.0f)
        {
            List<Vector3> spiralPath = new List<Vector3>();

            Vector3 direction = (targetPoint - startPoint).normalized;
            float distance = Vector3.Distance(startPoint, targetPoint);

            Vector3 ortho = Vector3.Cross(direction, Vector3.up).normalized;
            if (ortho == Vector3.zero)
            {
                ortho = Vector3.Cross(direction, Vector3.right).normalized;
            }

            Vector3 ortho2 = Vector3.Cross(direction, ortho).normalized;

            for (int i = 0; i < numPoints; i++)
            {
                float t = (float)i / (numPoints - 1);
                float currentRadius = spiralRadius * t;
                float angle = 2 * Mathf.PI * 5 * t;

                Vector3 offset = currentRadius * (Mathf.Cos(angle) * ortho + Mathf.Sin(angle) * ortho2);
                Vector3 pointOnLine = Vector3.Lerp(startPoint, targetPoint, t);
                Vector3 spiralPosition = pointOnLine + offset;

                if (isNoiseEnabled)
                {
                    Vector3 noise = new Vector3(
                        Mathf.PerlinNoise(t * 10, 0) - 0.5f,
                        Mathf.PerlinNoise(t * 10, 1) - 0.5f,
                        Mathf.PerlinNoise(t * 10, 2) - 0.5f
                    ) * noiseStrength;

                    spiralPosition += noise;
                }

                spiralPath.Add(spiralPosition);
            }

            return spiralPath.ToArray();
        }

        public static Vector3[] GenerateHyperbolicSpiralPath(Vector3 startPoint, Vector3 targetPoint, float spiralRadius, int numPoints = 100, bool isNoiseEnabled = false, float noiseStrength = 1.0f)
        {
            List<Vector3> spiralPath = new List<Vector3>();

            Vector3 direction = (targetPoint - startPoint).normalized;
            float distance = Vector3.Distance(startPoint, targetPoint);
            Vector3 ortho = Vector3.Cross(direction, Vector3.up).normalized;
            if (ortho == Vector3.zero)
            {
                ortho = Vector3.Cross(direction, Vector3.right).normalized;
            }
            Vector3 ortho2 = Vector3.Cross(direction, ortho).normalized;

            float a = spiralRadius;

            for (int i = 1; i <= numPoints; i++)
            {
                float t = (float)i / numPoints;
                float phi = 2 * Mathf.PI * 5 * t;
                float currentRadius = a / phi;

                Vector3 offset = currentRadius * (Mathf.Cos(phi) * ortho + Mathf.Sin(phi) * ortho2);
                Vector3 pointOnLine = Vector3.Lerp(startPoint, targetPoint, t);
                Vector3 spiralPosition = pointOnLine + offset;

                if (isNoiseEnabled)
                {
                    Vector3 noise = new Vector3(
                        Mathf.PerlinNoise(t * 10, 0) - 0.5f,
                        Mathf.PerlinNoise(t * 10, 1) - 0.5f,
                        Mathf.PerlinNoise(t * 10, 2) - 0.5f
                    ) * noiseStrength;

                    spiralPosition += noise;
                }

                spiralPath.Add(spiralPosition);
            }

            return spiralPath.ToArray();
        }

        public static Vector3[] GenerateFermatsSpiralPath(Vector3 startPoint, Vector3 targetPoint, float spiralRadius, int numPoints = 100, bool isNoiseEnabled = false, float noiseStrength = 1.0f)
        {
            List<Vector3> spiralPath = new List<Vector3>();

            Vector3 direction = (targetPoint - startPoint).normalized;
            float distance = Vector3.Distance(startPoint, targetPoint);
            Vector3 ortho = Vector3.Cross(direction, Vector3.up).normalized;
            if (ortho == Vector3.zero)
            {
                ortho = Vector3.Cross(direction, Vector3.right).normalized;
            }
            Vector3 ortho2 = Vector3.Cross(direction, ortho).normalized;

            float a = spiralRadius;

            for (int i = 0; i < numPoints; i++)
            {
                float t = (float)i / numPoints;
                float phi = 2 * Mathf.PI * 5 * t;
                float currentRadius = a * Mathf.Sqrt(phi);

                Vector3 offset = currentRadius * (Mathf.Cos(phi) * ortho + Mathf.Sin(phi) * ortho2);
                Vector3 pointOnLine = Vector3.Lerp(startPoint, targetPoint, t);
                Vector3 spiralPosition = pointOnLine + offset;

                if (isNoiseEnabled)
                {
                    Vector3 noise = new Vector3(
                        Mathf.PerlinNoise(t * 10, 0) - 0.5f,
                        Mathf.PerlinNoise(t * 10, 1) - 0.5f,
                        Mathf.PerlinNoise(t * 10, 2) - 0.5f
                    ) * noiseStrength;

                    spiralPosition += noise;
                }

                spiralPath.Add(spiralPosition);
            }

            return spiralPath.ToArray();
        }

        public static Vector3[] GenerateLituusSpiralPath(Vector3 startPoint, Vector3 targetPoint, float spiralRadius, int numPoints = 100, bool isNoiseEnabled = false, float noiseStrength = 1.0f)
        {
            List<Vector3> spiralPath = new List<Vector3>();

            Vector3 direction = (targetPoint - startPoint).normalized;
            float distance = Vector3.Distance(startPoint, targetPoint);
            Vector3 ortho = Vector3.Cross(direction, Vector3.up).normalized;
            if (ortho == Vector3.zero)
            {
                ortho = Vector3.Cross(direction, Vector3.right).normalized;
            }
            Vector3 ortho2 = Vector3.Cross(direction, ortho).normalized;

            float a = spiralRadius;

            for (int i = 1; i <= numPoints; i++)
            {
                float t = (float)i / numPoints;
                float phi = 2 * Mathf.PI * 5 * t;
                float currentRadius = a / Mathf.Sqrt(phi);

                Vector3 offset = currentRadius * (Mathf.Cos(phi) * ortho + Mathf.Sin(phi) * ortho2);
                Vector3 pointOnLine = Vector3.Lerp(startPoint, targetPoint, t);
                Vector3 spiralPosition = pointOnLine + offset;

                if (isNoiseEnabled)
                {
                    Vector3 noise = new Vector3(
                        Mathf.PerlinNoise(t * 10, 0) - 0.5f,
                        Mathf.PerlinNoise(t * 10, 1) - 0.5f,
                        Mathf.PerlinNoise(t * 10, 2) - 0.5f
                    ) * noiseStrength;

                    spiralPosition += noise;
                }

                spiralPath.Add(spiralPosition);
            }

            return spiralPath.ToArray();
        }

        public static Vector3[] GenerateSphericalSpiralPath(Vector3 center, float radius, int numPoints = 100, bool isNoiseEnabled = false, float noiseStrength = 1.0f)
        {
            List<Vector3> spiralPath = new List<Vector3>();

            for (int i = 0; i < numPoints; i++)
            {
                float t = (float)i / (numPoints - 1); // Parameter t from 0 to 1
                float theta = 2 * Mathf.PI * 5 * t; // 5 is the number of spiral turns
                float phi = Mathf.Acos(1 - 2 * t); // Interpolating from 0 to Pi

                // Spherical to Cartesian coordinates
                float x = radius * Mathf.Sin(phi) * Mathf.Cos(theta);
                float y = radius * Mathf.Sin(phi) * Mathf.Sin(theta);
                float z = radius * Mathf.Cos(phi);

                // Translate from sphere's origin to center
                Vector3 spiralPosition = new Vector3(x, y, z) + center;

                if (isNoiseEnabled)
                {
                    Vector3 noise = new Vector3(
                        Mathf.PerlinNoise(i * 0.1f, 0) - 0.5f,
                        Mathf.PerlinNoise(i * 0.1f, 1) - 0.5f,
                        Mathf.PerlinNoise(i * 0.1f, 2) - 0.5f
                    ) * noiseStrength;
                    spiralPosition += noise;
                }

                spiralPath.Add(spiralPosition);
            }

            return spiralPath.ToArray();
        }

        public static Vector3[] GenerateParabolicPath(Vector3 startPoint, Vector3 endPoint, float height, int numPoints = 100, bool isNoiseEnabled = false, float noiseStrength = 1.0f)
        {
            Vector3[] path = new Vector3[numPoints];
            Vector3 midPoint = (startPoint + endPoint) / 2 + new Vector3(0, height, 0);
            for (int i = 0; i < numPoints; i++)
            {
                float t = (float)i / (numPoints - 1);
                Vector3 p0 = Vector3.Lerp(startPoint, midPoint, t);
                Vector3 p1 = Vector3.Lerp(midPoint, endPoint, t);
                Vector3 point = Vector3.Lerp(p0, p1, t);

                if (isNoiseEnabled)
                {
                    Vector3 noise = new Vector3(
                        Mathf.PerlinNoise(i * 0.1f, 0) - 0.5f,
                        Mathf.PerlinNoise(i * 0.1f, 1) - 0.5f,
                        Mathf.PerlinNoise(i * 0.1f, 2) - 0.5f
                    ) * noiseStrength;
                    point += noise;
                }

                path[i] = point;
            }
            return path;
        }

        public static Vector3[] GenerateSinusoidalPath(Vector3 startPoint, Vector3 endPoint, float amplitude, int numPoints = 100, bool isNoiseEnabled = false, float noiseStrength = 1.0f)
        {
            Vector3[] path = new Vector3[numPoints];
            for (int i = 0; i < numPoints; i++)
            {
                float t = (float)i / (numPoints - 1);
                float angle = Mathf.PI * 2 * t;
                Vector3 linearPoint = Vector3.Lerp(startPoint, endPoint, t);
                Vector3 point = linearPoint + new Vector3(0, amplitude * Mathf.Sin(angle), 0);

                if (isNoiseEnabled)
                {
                    Vector3 noise = new Vector3(
                        Mathf.PerlinNoise(i * 0.1f, 0) - 0.5f,
                        Mathf.PerlinNoise(i * 0.1f, 1) - 0.5f,
                        Mathf.PerlinNoise(i * 0.1f, 2) - 0.5f
                    ) * noiseStrength;
                    point += noise;
                }

                path[i] = point;
            }
            return path;
        }

        public static Vector3[] GenerateRandomWalkPath(Vector3 startPoint, int numPoints = 100, float stepSize = 1.0f, bool isNoiseEnabled = false, float noiseStrength = 1.0f)
        {
            Vector3[] path = new Vector3[numPoints];
            path[0] = startPoint;
            for (int i = 1; i < numPoints; i++)
            {
                Vector3 step = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                ).normalized * stepSize;

                Vector3 nextPoint = path[i - 1] + step;

                if (isNoiseEnabled)
                {
                    Vector3 noise = new Vector3(
                        Mathf.PerlinNoise(i * 0.1f, 0) - 0.5f,
                        Mathf.PerlinNoise(i * 0.1f, 1) - 0.5f,
                        Mathf.PerlinNoise(i * 0.1f, 2) - 0.5f
                    ) * noiseStrength;
                    nextPoint += noise;
                }

                path[i] = nextPoint;
            }
            return path;
        }


        public static Vector3[] GenerateHelixPath(Vector3 center, float radius, float pitch, int numPoints = 100, bool isNoiseEnabled = false, float noiseStrength = 1.0f)
        {
            Vector3[] path = new Vector3[numPoints];
            for (int i = 0; i < numPoints; i++)
            {
                float t = (float)i / (numPoints - 1);
                float angle = 2 * Mathf.PI * 5 * t; // 5 turns
                float z = pitch * t;
                Vector3 point = center + new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), z);

                if (isNoiseEnabled)
                {
                    Vector3 noise = new Vector3(
                        Mathf.PerlinNoise(i * 0.1f, 0) - 0.5f,
                        Mathf.PerlinNoise(i * 0.1f, 1) - 0.5f,
                        Mathf.PerlinNoise(i * 0.1f, 2) - 0.5f
                    ) * noiseStrength;
                    point += noise;
                }

                path[i] = point;
            }
            return path;
        }

        public static Vector3[] GenerateLissajousCurve(Vector3 center, float A, float B, float a, float b, float delta, int numPoints = 100, bool isNoiseEnabled = false, float noiseStrength = 1.0f)
        {
            Vector3[] path = new Vector3[numPoints];
            for (int i = 0; i < numPoints; i++)
            {
                float t = (float)i / (numPoints - 1);
                Vector3 point = center + new Vector3(A * Mathf.Sin(a * t + delta), B * Mathf.Sin(b * t), 0);

                if (isNoiseEnabled)
                {
                    Vector3 noise = new Vector3(
                        Mathf.PerlinNoise(i * 0.1f, 0) - 0.5f,
                        Mathf.PerlinNoise(i * 0.1f, 1) - 0.5f,
                        Mathf.PerlinNoise(i * 0.1f, 2) - 0.5f
                    ) * noiseStrength;
                    point += noise;
                }

                path[i] = point;
            }
            return path;
        }

        public static Vector3[] GenerateBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int numPoints = 100, bool isNoiseEnabled = false, float noiseStrength = 1.0f)
        {
            Vector3[] path = new Vector3[numPoints];
            for (int i = 0; i < numPoints; i++)
            {
                float t = (float)i / (numPoints - 1);
                float u = 1 - t;
                Vector3 point = u * u * u * p0 + 3 * u * u * t * p1 + 3 * u * t * t * p2 + t * t * t * p3;

                if (isNoiseEnabled)
                {
                    Vector3 noise = new Vector3(
                        Mathf.PerlinNoise(i * 0.1f, 0) - 0.5f,
                        Mathf.PerlinNoise(i * 0.1f, 1) - 0.5f,
                        Mathf.PerlinNoise(i * 0.1f, 2) - 0.5f
                    ) * noiseStrength;
                    point += noise;
                }

                path[i] = point;
            }
            return path;
        }

        public static Vector3[] BlendPaths(Vector3[] path1, Vector3[] path2, int transitionPoints)
        {
            if (transitionPoints <= 0 || transitionPoints > path1.Length || transitionPoints > path2.Length)
            {
                Debug.LogError("Invalid number of transition points.");
                return null;
            }

            List<Vector3> blendedPath = new List<Vector3>();

            // Add the initial part of the first path
            for (int i = 0; i < path1.Length - transitionPoints; i++)
            {
                blendedPath.Add(path1[i]);
            }

            // Create the transition segment
            for (int i = 0; i < transitionPoints; i++)
            {
                float t = (float)i / (transitionPoints - 1);
                Vector3 interpolatedPoint = Vector3.Lerp(path1[path1.Length - transitionPoints + i], path2[i], t);
                blendedPath.Add(interpolatedPoint);
            }

            // Add the remaining part of the second path
            for (int i = transitionPoints; i < path2.Length; i++)
            {
                blendedPath.Add(path2[i]);
            }

            return blendedPath.ToArray();
        }
    }
}