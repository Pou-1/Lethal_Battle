using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Lethal_Battle.codes
{
    internal static class PathSystem
    {
        public static Queue<Vector3> GeneratePathPoints(Vector3 start, Vector3 end, float spacing)
        {
            Queue<Vector3> points = new Queue<Vector3>();

            NavMeshPath path = new NavMeshPath();

            if (!NavMesh.CalculatePath(start, end, NavMesh.AllAreas, path))
                return points;

            if (path.status != NavMeshPathStatus.PathComplete)
                return points;

            Vector3[] corners = path.corners;

            if (corners.Length < 2)
                return points;

            bool left = true;
            float baseOffset = 0.18f;

            for (int i = 0; i < corners.Length - 1; i++)
            {
                Vector3 from = corners[i];
                Vector3 to = corners[i + 1];

                Vector3 dir = (to - from).normalized;
                Vector3 side = Vector3.Cross(Vector3.up, dir).normalized;

                float dist = Vector3.Distance(from, to);

                if (dist <= 0.01f)
                    continue;

                float traveled = 0f;

                while (traveled < dist)
                {
                    float t = traveled / dist;

                    Vector3 pos = Vector3.Lerp(from, to, t);

                    float offsetNoise = Random.Range(-0.04f, 0.04f);
                    float sideOffset = baseOffset + offsetNoise;

                    Vector3 offset = (left ? -side : side) * sideOffset;

                    float forwardNoise = Random.Range(-0.05f, 0.05f);
                    pos += dir * forwardNoise;

                    points.Enqueue(pos + offset);

                    left = !left;

                    float step = spacing + Random.Range(-0.12f, 0.12f);
                    traveled += step;
                }
            }

            return points;
        }

        public static void SpawnFootprint(GameObject footprintPrefab, Vector3 position, Vector3 direction, float lifetime)
        {
            float rotNoise = Random.Range(-12f, 12f);

            Quaternion rotation =
                Quaternion.LookRotation(direction) *
                Quaternion.Euler(0f, rotNoise, 0f);

            GameObject footprint = Object.Instantiate(
                footprintPrefab,
                position,
                rotation
            );

            Rigidbody? rb = footprint.GetComponent<Rigidbody>();

            if (rb != null)
                rb.isKinematic = true;

            Object.Destroy(footprint, lifetime);
        }
    }
}