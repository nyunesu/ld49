using UnityEngine;
using System.Collections;
namespace Apollo
{
    [AddComponentMenu("Apollo/Modifiers/Local Position Modifier")]
    public class VisualizerLocalPositionModifier : VisualizerObjectBase
    {
        public bool UseBasePosition;
        private Vector3 BasePosition;


        public Vector3 V3modifier;

        public void Awake()
        {
            BasePosition = transform.localPosition;
        }

        void ChangePosition()
        {

            EvaluateRange();

            Vector3 mod = V3modifier * modifier;
            if (UseBasePosition)
            {
                mod += BasePosition;
            }
            if (mod.x == 0)
            {
                mod.x = transform.localPosition.x;
            }
            if (mod.y == 0)
            {
                mod.y = transform.localPosition.y;
            }
            if (mod.z == 0)
            {
                mod.z = transform.localPosition.z;
            }

            transform.localPosition = mod;
        }

        // Update is called once per frame
        void Update()
        {
            ChangePosition();
        }
    }
}