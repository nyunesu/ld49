using UnityEngine;
using System.Collections;
namespace Apollo
{
    [AddComponentMenu("Apollo/Modifiers/Light Modifier")]
    [RequireComponent(typeof(Light))]
    public class VisualizerLightsModifier : VisualizerObjectBase
    {

       
        public bool AffectIntensity;
        public float IntensityMultiplier;
        [Range(0.01f, 3)]
        public float intensityTransitionSpeed;
        
        private Light myLight;

        //Color
        public enum yesno { Yes, No };
        public bool AffectColor;
        [Range(0.0f, 2)]
        public float sensitivity;
        [Range(0, 5)]
        public float ColorTransitionSpeed;
        public Color[] Available_colors;
        public yesno Random_Color;
        private Color colorToLerp1;
        private Color colorToLerp2;
        private float delta_Modifier;
        private bool changedColor;
        private int ActualColor1 = 0;


        void Start()
        {

            myLight = GetComponent<Light>();
            //baseRange = myLight.range;
        }

        void ModifyLights()
        {
            EvaluateRange();
            if (AffectIntensity)
                CalculateIntensity();
            if (AffectColor)
                CalculateColor();
        }
        void CalculateIntensity()
        {
            myLight.intensity = Mathf.Lerp(myLight.intensity, modifier * IntensityMultiplier, Time.deltaTime * intensityTransitionSpeed);
        }


        void CalculateColor()
        {
            delta_Modifier = Mathf.Abs(modifier);

            if (delta_Modifier < sensitivity)
            {
                switch (Random_Color)
                {
                    case yesno.Yes:
                        if (!changedColor)
                        {
                            int index = Random.Range(0, Available_colors.Length);
                            //Debug.Log(index);

                            colorToLerp1 = Available_colors[index];
                            if (index + 2 >= Available_colors.Length) index = Available_colors.Length - 3;
                            colorToLerp2 = Available_colors[index + 1];
                            changedColor = true;
                        }
                        break;
                    case yesno.No:
                        if (!changedColor)
                        {
                            colorToLerp1 = Available_colors[ActualColor1];

                            ActualColor1++;
                            if (ActualColor1 >= Available_colors.Length) ActualColor1 = 0;

                            changedColor = true;
                        }
                        break;
                }
            }
            else
            {
                changedColor = false;
            }
            myLight.color = Color.Lerp(myLight.color, colorToLerp1, ColorTransitionSpeed * Time.deltaTime);


            //lr.material.color = Color.Lerp(lr.material.color, colorToLerp, ColorTransitionSpeed * Time.deltaTime);
            //myRenderer.color = Color.Lerp(myRenderer.color,colorToLerp,TransitionSpeed*Time.deltaTime);
        }

        // Update is called once per frame
        void Update()
        {
            ModifyLights();
        }
    }
}
