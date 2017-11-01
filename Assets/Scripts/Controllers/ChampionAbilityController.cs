using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionAbilityController : MonoBehaviour {

	public Animator AbilityEffectAnimator;

	public ParticleSystem Ability1Effect;
	public ParticleSystem Ability2Effect;
	public ParticleSystem Ability3Effect;

	public void PlayAbilityEffect(int effect) {
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
}