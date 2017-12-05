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
		AbilityEffectAnimator = GetComponent<Animator>();
	}

	/// <summary>
	/// Play the particle effect that corresponds with the ability clicked
	/// </summary>
	/// <param name="effect"></param>
	public void PlayAbilityEffect(int effect) {
		var clip = GetRandomSound();
		if(clip != null) {
			_aSource.clip = clip;
			_aSource.Play();
		}

		PlayAnimation();
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

	/// <summary>
	/// Plays the ability animation, if it exists
	/// </summary>
	/// <param name="abNumber"></param>
	private void PlayAnimation() {
		if(AbilityEffectAnimator == null) { return; }
		AbilityEffectAnimator.SetTrigger("Attack");
	}

	/// <summary>
	/// Find a random ability sound from the Resources folder if they exist
	/// </summary>
	/// <returns></returns>
	private AudioClip GetRandomSound() {
		var allSounds = Resources.LoadAll("Sounds/AbilityVFX", typeof(AudioClip));
		if(allSounds.Length > 0) {
			AudioClip sound = allSounds[Random.Range(0, allSounds.Length)] as AudioClip;
			return sound;
		}
		return null;
	}
}