using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

	public AudioSource MainAudioSource;
	public AudioSource BattleAudioSource;

	public float FadeTimer;

	private bool _fadeBattleOut;
	private bool _fadeMainOut;

	private float _startVolume;

	private AudioSource _currentAudio;

	private void Start() {
		_currentAudio = MainAudioSource;
	}

	public void SwitchAudioSource() {
		StartCoroutine (SwapAudio ());
	}

	/// <summary>
	/// Swaps the audio from what's currently playing to the other one. Not very expendable-friendly.
	/// </summary>
	/// <returns></returns>
	private IEnumerator SwapAudio() {
			_startVolume = _currentAudio.volume;
			while (_currentAudio.volume > 0) {
				_currentAudio.volume -= _startVolume * Time.deltaTime / FadeTimer;
				yield return null;
			}

			_currentAudio.Stop ();
			_currentAudio.volume = _startVolume;
			if (_currentAudio == MainAudioSource) {
				_currentAudio = BattleAudioSource;

			} else {
				_currentAudio = BattleAudioSource;
			}
			_currentAudio.Play ();
	}
}