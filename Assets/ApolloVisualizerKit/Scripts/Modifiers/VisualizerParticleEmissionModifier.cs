using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Apollo;
[RequireComponent(typeof(ParticleSystem))]
public class VisualizerParticleEmissionModifier : VisualizerObjectBase
{
    private ParticleSystem part;
    [Range(0,10)]
    public float maxAmountToEmit;
    public Gradient color;
    private void Awake()
    {
        part = GetComponent<ParticleSystem>();        
    }
    void EmitParticles()
    {
        part.Emit(Mathf.RoundToInt(maxAmountToEmit*modifier));
    }
    // Update is called once per frame
    void Update()
    {
        EvaluateRange();
        EmitParticles();
    }
}
