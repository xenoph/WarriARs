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

	private int _initDamage;

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
				var dmgDone = CalculateNewDamage();
				if(dmgDone == _initDamage) {
					_anim.SetTrigger("Shake");
				}
				_anim.SetBool("Show", false);
				MFSceneController.GetPlayerDamage(dmgDone);
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

	public void StartCombat(int initDamage) {
		_anim.SetBool("Show", true);
		_initDamage = initDamage;
		_moveLeft = (Random.value > 0.5f);
		_moveNeedle = true;
		StartCoroutine(EnableCombat());
	}

	private IEnumerator EnableCombat() {
		yield return new WaitForSeconds(1);
		_runningCombat = true;
	}

	private int CalculateNewDamage() {
		var maxDmg = _initDamage;
		Vector3 currentSide;
		if(_moveLeft) {
			currentSide = _leftEndPos;
		} else {
			currentSide = _rightEndPos;
		}
		var hitPercentage = Vector3.Distance(_needleRectTransform.anchoredPosition, _midPos) / Vector3.Distance(currentSide, _midPos);
		var dmgToRemove = hitPercentage * _initDamage;
		var actualDmg = _initDamage - dmgToRemove;
		return Mathf.RoundToInt(actualDmg);
	}
}