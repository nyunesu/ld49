using UnityEngine;
using System.Collections;
namespace Apollo
{
    [AddComponentMenu("Apollo/Modifiers/Scale Modifier")]
    public class VisualizerScaleModifier : VisualizerObjectBase
    {
        public bool UseBaseScale;
        private Vector3 BaseScale;
        //Drop down menu
        //multiplier for the range value
        public Vector3 V3modifier;

        public void Awake()
        {
            BaseScale = transform.localScale;
        }

        void ChangeScale()
        {

            EvaluateRange();

            Vector3 mod = V3modifier * modifier;
            if (UseBaseScale)
            {
                mod += BaseScale;
            }
            if (mod.x == 0)
            {
                mod.x = transform.localScale.x;
            }
            if (mod.y == 0)
            {
                mod.y = transform.localScale.y;
            }
            if (mod.z == 0)
            {
                mod.z = transform.localScale.z;
            }
            transform.localScale = mod;
        }

        // Update is called once per frame
        void Update()
        {
            ChangeScale();
        }
    }
}
