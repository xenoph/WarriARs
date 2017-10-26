using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatNeedleBar : MonoBehaviour {
	public float NeedleSpeed;
	public MidtermFightSceneController MFSceneController;

	private GameObject _needle;
	private Animator _anim;

	private bool _runningCombat = false;
	private bool _moveNeedle = false;
	private bool _moveLeft;

	private Vector3 _leftEndPos = new Vector3(-440, 0, 0);
	private Vector3 _rightEndPos = new Vector3(440, 0, 0);
	private Vector3 _midPos = new Vector3(0, 0, 0);

	private RectTransform _needleRectTransform;

	private bool _insideShake;

	private void Awake() {
		_needle = transform.GetChild(1).gameObject;
		_anim = GetComponent<Animator>();
		_needleRectTransform = transform.GetChild(1).GetComponent<RectTransform>();
	}

	private void Update() {
		if(_runningCombat) {
			if(Input.GetMouseButtonUp(0)) {
				_runningCombat = false;
				_moveNeedle = false;
				if(_insideShake) {
					_anim.SetTrigger("Shake");
				}
				_anim.SetBool("Show", false);
				GameController.instance.battleController.SendAbility(CalculatePercentageHit());
				Invoke("ShowAbilityBar", 1f);
			}
		}
		if(_moveNeedle) {
			if(_moveLeft) {
				_needleRectTransform.anchoredPosition = Vector3.Lerp(_needleRectTransform.anchoredPosition, _leftEndPos, Time.deltaTime * NeedleSpeed);
				if(Vector3.Distance(_needleRectTransform.anchoredPosition, _leftEndPos) < 1f) { _moveLeft = false; }
			} else {
				_needleRectTransform.anchoredPosition = Vector3.Lerp(_needleRectTransform.anchoredPosition, _rightEndPos, Time.deltaTime * NeedleSpeed);
				if(Vector3.Distance(_needleRectTransform.anchoredPosition, _rightEndPos) < 1f) { _moveLeft = true; }
			}
		}
	}

	public void StartNeedle() {
		_anim.SetBool("Show", true);
		_moveLeft = (Random.value > 0.5f);
		_moveNeedle = true;
		StartCoroutine(EnableCombat());
	}

	private IEnumerator EnableCombat() {
		yield return new WaitForSeconds(1);
		_runningCombat = true;
	}

	private int CalculatePercentageHit() {
		Vector3 currentSide;
		if(_needleRectTransform.anchoredPosition.x < 0) {
			currentSide = _leftEndPos;
		} else {
			currentSide = _rightEndPos;
		}
		
		var fullLength = Vector3.Distance(currentSide, _midPos);
		var remainingLength = fullLength - Vector3.Distance(_needleRectTransform.anchoredPosition, _midPos);
		var hitPercentage = remainingLength / fullLength;
		return Mathf.RoundToInt(hitPercentage * 100);
	}

	private void ShowAbilityBar() {
		GameController.instance.InterfaceController.AbilityBarAnimator.SetBool("Hide", false);
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if(other.name == "BarNeedle") {
			_insideShake = true;
		}
	}

	private void OnTriggerExit2D(Collider2D other) {
		if(other.name == "BarNeedle") {
			_insideShake = false;
		}
	}
}