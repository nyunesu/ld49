using UnityEngine;
using System.Collections;
namespace Apollo
{
    [AddComponentMenu("Apollo/Modifiers/Color Modifier")]
    [RequireComponent(typeof(Renderer))]
    public class VisualizerColorModifier : VisualizerObjectBase
    {

        [Range(0.0f, 2)]
        public float sensitivity;
        //[Range(0.0f, 10)]	public float	colorSpeed;
        public Color[] Available_colors;
        //	public	float	Trigger_value;
        public enum yesno { Yes, No };
        public yesno Random_Color;
        private Renderer myRenderer;
        private Color colorToChange;

        private float delta_Modifier;
        private float old_Modifier;
        private bool changedColor;
        private int ActualColor = 0;


        private void Awake()
        {
            myRenderer = GetComponent<Renderer>();
            colorToChange = Available_colors[0];
            //Range_ToFollow = Ranges.Bass_1;
        }

        void EvaluateTrigger()
        {
            EvaluateRange();
            delta_Modifier = Mathf.Abs(modifier - old_Modifier);

            if (delta_Modifier > sensitivity)
            {
                switch (Random_Color)
                {
                    case yesno.Yes:
                        if (!changedColor)
                        {
                            int index = Random.Range(0, Available_colors.Length);
                            //Debug.Log(index);

                            colorToChange = Available_colors[index];
                            //changedColor=true;

                            //myRenderer.material.color= Available_colors[index];
                            changedColor = true;
                        }
                        break;
                    case yesno.No:
                        if (!changedColor)
                        {
                            myRenderer.material.color = Available_colors[ActualColor];
                            ActualColor++;
                            if (ActualColor >= Available_colors.Length)
                            {
                                ActualColor = 0;
                            }
                            colorToChange = Available_colors[ActualColor];

                            changedColor = true;
                        }
                        break;
                }
            }
            else {
                changedColor = false;
            }


            //myRenderer.material.color = Color.Lerp(myRenderer.material.color, colorToChange,Mathf.PingPong(Time.deltaTime*colorSpeed,0.1f) );

            myRenderer.material.color = colorToChange;

            old_Modifier = modifier;
        }


        // Update is called once per frame
        void Update()
        {
            EvaluateTrigger();
            if (changedColor)
            {
                myRenderer.material.color = Color.Lerp(myRenderer.material.color, colorToChange, 1);
            }
        }
    }
}