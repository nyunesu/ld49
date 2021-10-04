using UnityEngine;
using System.Collections;
namespace Apollo
{
    [AddComponentMenu("Apollo/Modifiers/Sprite Color Modifier")]
    [RequireComponent(typeof(SpriteRenderer))]
    public class VisualizerSpriteColorModifier : VisualizerObjectBase
    {

        [Range(0.0f, 2)]
        public float sensitivity;
        [Range(0.0f, 10)]
        public float TransitionSpeed;
        public Color[] Available_colors;
        //public	float			Trigger_value;
        private float delta_Modifier;
        private bool changedColor;
        public enum yesno { Yes, No };
        public yesno Random_Color;
        private Color colorToLerp;
        private SpriteRenderer myRenderer;
        private int ActualColor = 0;

        private void Awake()
        {
            myRenderer = GetComponent<SpriteRenderer>();
            //Range_ToFollow = Ranges.Bass_1;
            colorToLerp = myRenderer.color;
        }

        void EvaluateTrigger()
        {
            EvaluateRange();

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

                            colorToLerp = Available_colors[index];
                            changedColor = true;
                        }
                        break;
                    case yesno.No:
                        if (!changedColor)
                        {
                            colorToLerp = Available_colors[ActualColor];
                            ActualColor++;
                            if (ActualColor >= Available_colors.Length)
                            {
                                ActualColor = 0;
                            }
                            changedColor = true;
                        }
                        break;
                }
            }
            else {
                changedColor = false;
            }
            myRenderer.color = Color.Lerp(myRenderer.color, colorToLerp, TransitionSpeed * Time.deltaTime);
        }


        // Update is called once per frame
        void Update()
        {
            EvaluateTrigger();
        }
    }
}