using GameNetcodeStuff;
using UnityEngine;

namespace Lethal_Battle.codes
{
    public class ScrapIndicator : MonoBehaviour
    {
        private GrabbableObject? grabbable;
        private GameObject? circleObject;
        private bool circleCreated;

        private void Awake()
        {
            grabbable = GetComponent<GrabbableObject>();
        }

        private void Start()
        {
            UpdateIndicator();
        }

        private void Update()
        {
            if (grabbable == null)
                return;

            UpdateIndicator();
        }

        private void UpdateIndicator()
        {
            bool shouldShow = grabbable != null && !grabbable.isHeld;

            if (shouldShow && !circleCreated)
            {
                CreateYellowCircle();
            }
            else if (!shouldShow && circleCreated)
            {
                DestroyYellowCircle();
            }
        }

        private void CreateYellowCircle()
        {
            if (circleObject != null)
                return;

            circleObject = new GameObject("ScrapIndicator");
            circleObject.transform.SetParent(transform);
            circleObject.transform.localPosition = new Vector3(0f, -0.1f, 0f);
            circleObject.transform.localRotation = Quaternion.identity;

            LineRenderer line = circleObject.AddComponent<LineRenderer>();

            line.useWorldSpace = false;
            line.loop = true;
            line.positionCount = 64;

            line.startWidth = 0.04f;
            line.endWidth = 0.04f;

            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = Color.yellow;
            line.endColor = Color.yellow;

            float radius = 0.6f;

            for (int i = 0; i < 64; i++)
            {
                float angle = i * Mathf.PI * 2f / 64f;
                line.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius));
            }

            circleCreated = true;
        }

        private void DestroyYellowCircle()
        {
            if (circleObject != null)
            {
                Destroy(circleObject);
                circleObject = null;
            }

            circleCreated = false;
        }
    }
}