using UnityEngine;
using System.Collections;
namespace Apollo
{
    [AddComponentMenu("Apollo/Modifiers/Rotation Modifier")]
    public class VisualizerRotationModifier : VisualizerObjectBase
    {
        public Vector3 RotationAxes;
        public float RotationSpeed;
        void ChangeRotation()
        {
            Vector3 rot = RotationAxes * RotationSpeed * modifier;
            transform.Rotate(rot);
        }

        void Update()
        {
            EvaluateRange();
            ChangeRotation();
        }
    }
}
