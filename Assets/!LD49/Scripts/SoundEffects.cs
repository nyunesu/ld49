using System.Collections.Generic;
using ADEAudio;
using ADEUtility;
using UnityEngine;

public class SoundEffects : MonoSingleton<SoundEffects>
{
	[SerializeField]
	private SFXData[] sounds;

	private static readonly Dictionary<SoundType, AudioClip[]> SoundTypeToAudioClips =
		new Dictionary<SoundType, AudioClip[]>();

	private static readonly Dictionary<SoundType, float> SoundTypeToVolume
		= new Dictionary<SoundType, float>();

	protected override void Awake()
	{
		base.Awake();
		SoundTypeToAudioClips.Clear();

		foreach (SFXData data in sounds)
		{
			SoundTypeToAudioClips[data.Type] = data.AudioClips;
			SoundTypeToVolume[data.Type] = data.Volume;
		}
	}

	public static void Play(SoundType soundType)
	{
		AudioClip[] audioClips = SoundTypeToAudioClips[soundType];
		AudioClip randomClip = audioClips[Random.Range(0, audioClips.Length)];
		SoundManager.Play(randomClip, SoundTypeToVolume[soundType]);
	}
}
