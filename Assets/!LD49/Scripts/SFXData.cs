using UnityEngine;

[CreateAssetMenu(fileName = "New SFX Data", menuName = "SFX Data", order = 0)]
public class SFXData : ScriptableObject
{
	public AudioClip[] AudioClips;
	public SoundType Type;
	public float Volume = 1f;
}