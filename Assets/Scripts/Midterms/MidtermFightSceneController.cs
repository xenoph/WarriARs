using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MidtermFightSceneController : MonoBehaviour {

	[Header("UI Elements")]
	public Button Ability1;
	public Button Ability2;
	public Button Ability3;
	public Text EffectText;

	[Header("Champions")]
	public FightChampion InsaneSkillzChampion;
	public FightChampion AdrianChampion;

	[Header("GameObjects in Scene")]
	public GameObject Adrian;
	public GameObject RainPrefab;
	public Renderer GroundRenderer;

	[Header("Spawnable GameObjects")]
	public GameObject AdrianDeadParticles;

	[Header("Grounds")]
	public GameObject[] Grounds;

	private int _currentScene;
	private bool _playerTurn = true;
	private bool _adrianDead = false;
	private bool _wetGround = false;
	private bool _fireGround = false;
	private bool _adrianUsedGroundFire = false;

	void Start () {
		SetGround("NormalGround");
		SetUpButtons();
		SetUpScene1();
		StartCoroutine(PlayRound());
	}
	
	public void SetUpScene1() {
		_currentScene = 1;
		ResetHealths();
		ResetEffects();
		Ability1.interactable = true;
		Ability2.interactable = false;
		Ability3.interactable = false;
	}

	public void SetUpScene2() {
		_currentScene = 2;
		ResetHealths();
		ResetEffects();
		Ability1.interactable = false;
		Ability2.interactable = true;
		Ability3.interactable = false;
	}

	public void SetUpScene3() {
		_currentScene = 3;
		ResetHealths();
		ResetEffects();
		Ability1.interactable = false;
		Ability2.interactable = false;
		Ability3.interactable = true;
	}

	public void UseAbility1() {
		if(!_playerTurn) { return; }
		if(_wetGround) {
			AdrianChampion.dealDamage(367);
			EffectText.text = "Ground is Wet! You deal increased damage!";
		} else if(_fireGround) {
			AdrianChampion.dealDamage(147);
			EffectText.text = "Ground on Fire! You deal decreased damage!";
		} else {
			EffectText.text = "You deal 256 damage!";
			AdrianChampion.dealDamage(256);
		}
		if(AdrianChampion.CheckDead()) { AdrianDead(); }
		_playerTurn = false;
	}

	public void UseAbility2() {
		if(!_playerTurn) { return; }
		EffectText.text = "You are less likely to be hit for a round!";
		InsaneSkillzChampion.ChanceToHit -= 20;
		_playerTurn = false;
		if(_currentScene > 1) {
			Ability1.interactable = true;
		}
	}

	public void UseAbility3() {
		if(!_playerTurn) { return; }
		ResetEffects();
		RainPrefab.SetActive(true);
		EffectText.text = "You use Rain. The ground is wet!";
		SetGround("WetGround");
		_wetGround = true;
		Ability1.interactable = true;
		Ability2.interactable = true;
		_playerTurn = false;
	}

	private IEnumerator PlayRound() {
		while(_playerTurn) {
			yield return new WaitForSeconds(1);
		}

		if(!_adrianDead) {
			yield return new WaitForSeconds(2);

			switch(_currentScene) {
				case 1:
					InsaneSkillzChampion.dealDamage(40);
					EffectText.text = "Adrian deals 40 damage!";
					break;

				case 2:
					if(AdrianChampion.ChanceToHit == 100) {
						EffectText.text = "Adrian is less likely to be hit for a round!";
						AdrianChampion.ChanceToHit -= 20;
					} else {
						AdrianChampion.ChanceToHit = 100;
						if(InsaneSkillzChampion.dealDamage(40)) {
							EffectText.text = "Adrian deals 40 damage!";
						}
					}
					break;

				case 3:
					if(_wetGround && InsaneSkillzChampion.currentHealth == InsaneSkillzChampion.maxHealth) {
						if(InsaneSkillzChampion.dealDamage(20)) {
							EffectText.text = "Ground is Wet! Adrian deals decreased damage!";
						}
					} else if(!_fireGround && !_adrianUsedGroundFire) {
						EffectText.text = "Adrian puts the ground on fire!";
						ResetEffects();
						_adrianUsedGroundFire = true;
						SetGround("FireGround");
						_fireGround = true;
					} else if(_fireGround) {
						if(InsaneSkillzChampion.dealDamage(80)) {
							EffectText.text = "Ground on Fire! Adrian deals increased damage!";
						}
					} else {
						if(InsaneSkillzChampion.dealDamage(40)) {
							EffectText.text = "Adrian deals 40 damage!";
						}
					}
					break;

				default:
					break;
			}

		}

		if(_adrianDead) {
			if(_currentScene == 3) {
				yield break;
			}
			yield return new WaitForSeconds(2);
			EffectText.text = "";
			SceneManager.LoadScene("levelup", LoadSceneMode.Additive);
			yield return new WaitForSeconds(5);
			Adrian.SetActive(true);
			SceneManager.UnloadSceneAsync("levelup");
		} else {
			yield return new WaitForSeconds(2);
		}

		RestartIenum();
	}

	private void ResetHealths() {
		if(_currentScene == 1) {
			InsaneSkillzChampion.SetHealth(500);
			AdrianChampion.SetHealth(500);
		} else if (_currentScene == 2) {
			InsaneSkillzChampion.SetHealth(750);
			AdrianChampion.SetHealth(750);
		} else {
			InsaneSkillzChampion.SetHealth(1000);
			AdrianChampion.SetHealth(1000);
		}

		AdrianChampion.ChanceToHit = 100;
		InsaneSkillzChampion.ChanceToHit = 100;
	}

	private void SetUpButtons() {
		Ability1.onClick.AddListener(delegate { UseAbility1(); });
		Ability2.onClick.AddListener(delegate { UseAbility2(); });
		Ability3.onClick.AddListener(delegate { UseAbility3(); });
	}

	private void RestartIenum() {
		if(_adrianDead) {
			if(_currentScene == 1) {
				SetUpScene2();
			} else if (_currentScene == 2) {
				SetUpScene3();
			}
			_adrianDead = false;
		}

		if(InsaneSkillzChampion.ChanceToHit != 100) {
			InsaneSkillzChampion.ChanceToHit = 100;
		}

		EffectText.text = "";
		_playerTurn = true;
		StartCoroutine(PlayRound());
	}

	private void SpawnAdrianDeadParticles() {
		Instantiate(AdrianDeadParticles, new Vector3(1f, 2f, 1f), Quaternion.identity);
	}

	private void AdrianDead() {
		Adrian.SetActive(false);
		SpawnAdrianDeadParticles();
		_adrianDead = true;
	}

	private void ResetEffects() {
		SetGround("NormalGround");
		_wetGround = false;
		_fireGround = false;
		RainPrefab.SetActive(false);
	}

	private void SetGround(string ground) {
		foreach(var g in Grounds) {
			if(g.name == ground) {
				g.SetActive(true);
			} else {
				g.SetActive(false);
			}
		}
	}
}