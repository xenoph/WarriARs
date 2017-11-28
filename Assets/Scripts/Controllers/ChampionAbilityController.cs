using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionAbilityController : MonoBehaviour {

	public Animator AbilityEffectAnimator;

	public ParticleSystem Ability1Effect;
	public ParticleSystem Ability2Effect;
	public ParticleSystem Ability3Effect;

	private AudioSource _aSource;

	private void Awake() {
		_aSource = GetComponent<AudioSource>();
	}

	public void PlayAbilityEffect(int effect) {
		var clip = GetRandomSound();
		if(clip != null) {
			_aSource.clip = clip;
			_aSource.Play();
		}

		switch (effect) {
			case 0:
				Ability1Effect.Play();
				break;

			case 1:
				Ability2Effect.Play();
				break;

			case 2:
				Ability3Effect.Play();
				break;

			default:
				break;
		}
	}

	private void PlayAnimation(int abNumber) {
		if(AbilityEffectAnimator == null) { return; }
	}

	private AudioClip GetRandomSound() {
		var allSounds = Resources.LoadAll("Sounds/AbilityVFX", typeof(AudioClip));
		AudioClip sound = allSounds[Random.Range(0, allSounds.Length)] as AudioClip;
		return sound;
	}
}