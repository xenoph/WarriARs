using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MidtermFightSceneController : MonoBehaviour {

	[Header("UI Elements")]
	public Button Ability1;
	public Button Ability2;
	public Button Ability3;
	public Text EffectText;

	public CombatNeedleBar NeedleBar;

	[Header("Champions")]
	public GameObject InsaneSkillzChampion;
	public GameObject AdrianChampion;

	[Header("GameObjects in Scene")]
	public GameObject Adrian;
	public GameObject RainPrefab;
	public Renderer GroundRenderer;

	[Header("Spawnable Particles")]
	public GameObject AdrianDeadParticles;
	public GameObject SlashParticles;
	public GameObject IcicleExplosion;
	public GameObject FireBreath;

	[Header("SpawnableText")]
	public GameObject InfoText;
	public GameObject AdrianTextSpawnLocation;
	public GameObject InsaneTextSpawnLocation;

	[Header("Grounds")]
	public GameObject[] Grounds;

	private int _currentScene;
	private bool _playerTurn = true;
	private bool _adrianDead = false;
	private bool _wetGround = false;
	private bool _fireGround = false;
	private bool _adrianUsedGroundFire = false;

	private FightChampion _inzaneChamp;
	private FightChampion _adrianChamp;

	private Animator _aniCont;

	private PlayerCtrl _player;

	private void Awake() {
		_aniCont = transform.GetChild(0).GetComponent<Animator>();
		_inzaneChamp = transform.GetChild(1).GetComponent<FightChampion>();
		_adrianChamp = transform.GetChild(2).GetComponent<FightChampion>();
	}

	private void Start () {
		_player = GameObject.FindObjectOfType<PlayerCtrl>();
		_player.midtermFightScene = this;
		_player.register();
		SetGround("NormalGround");
		SetUpButtons();
	}

	public void SetUpScene(int fight) {
		switch(fight) {
			case 1:
				SetUpScene1();
				break;

			case 2:
				SetUpScene2();
				break;

			case 3:
				SetUpScene3();
				break;

			default:
				break;
		}
	}
	
	public void SetUpScene1() {
		_currentScene = 1;
		ResetHealths();
		ResetEffects();
		Ability1.interactable = true;
		Ability2.interactable = false;
		Ability3.interactable = false;
		StartCoroutine(PlayRound());
	}

	public void SetUpScene2() {
		_currentScene = 2;
		ResetHealths();
		ResetEffects();
		Ability1.interactable = false;
		Ability2.interactable = true;
		Ability3.interactable = false;
		StartCoroutine(PlayRound());
	}

	public void SetUpScene3() {
		_currentScene = 3;
		ResetHealths();
		ResetEffects();
		Ability1.interactable = false;
		Ability2.interactable = false;
		Ability3.interactable = true;
		StartCoroutine(PlayRound());
	}

	public void UseAbility1() {
		if(!_playerTurn) { return; }
		_aniCont.SetBool("Hide", true);
		if(_wetGround) {
			NeedleBar.StartCombat(367);
		} else if(_fireGround) {
			NeedleBar.StartCombat(167);
		} else {
			NeedleBar.StartCombat(256);
		}
	}

	public void UseAbility2() {
		if(!_playerTurn) { return; }
		_aniCont.SetBool("Hide", true);
		EffectText.text = "You are less likely to be hit for a round!";
		_inzaneChamp.ChanceToHit -= 20;
		_playerTurn = false;
		if(_currentScene > 1) {
			Ability1.interactable = true;
		}
	}

	public void UseAbility3() {
		if(!_playerTurn) { return; }
		_aniCont.SetBool("Hide", true);
		ResetEffects();
		RainPrefab.SetActive(true);
		EffectText.text = "You use Rain. The ground is wet!";
		SetGround("WetGround");
		_wetGround = true;
		Ability1.interactable = true;
		Ability2.interactable = true;
		_playerTurn = false;
	}

	public void GetPlayerDamage(int dmg) {
		if(_adrianChamp.dealDamage(dmg)) {
			var parts = Instantiate(SlashParticles, new Vector3(-3f, 430f, 1.6f), Quaternion.identity);
			Invoke("SpawnIceExplosion", 0.8f);
			StartCoroutine(SpawnInfoText(dmg, DamageFeedback(), AdrianChampion));
			if(_adrianChamp.CheckDead()) { AdrianDead(); }
		}
		_playerTurn = false;
	}

	private void SpawnIceExplosion() {
		Instantiate(IcicleExplosion, new Vector3(1.5f, 425f, 14f), Quaternion.identity);
	}

	private string DamageFeedback() {
		if(_wetGround) { return "SUPER EFFECTIVE"; }
		if(_fireGround) { return "HINDERED"; }
		else return "EFFECTIVE";
	}

	private IEnumerator PlayRound() {
		while(_playerTurn) {
			yield return new WaitForSeconds(0.1f);
		}

		if(!_adrianDead) {
			yield return new WaitForSeconds(2.5f);

			switch(_currentScene) {
				case 1:
					_inzaneChamp.dealDamage(40);
					InstantiateFireBreath();
					StartCoroutine(SpawnInfoText(40, "INEFFECTIVE", InsaneSkillzChampion));
					break;

				case 2:
					if(_adrianChamp.ChanceToHit == 100) {
						EffectText.text = "Adrian is less likely to be hit for a round!";
						_adrianChamp.ChanceToHit -= 20;
					} else {
						_adrianChamp.ChanceToHit = 100;
						if(_inzaneChamp.dealDamage(40)) {
							InstantiateFireBreath();
							StartCoroutine(SpawnInfoText(40, "Not Effective", InsaneSkillzChampion));
						}
					}
					break;

				case 3:
					if(_wetGround && _inzaneChamp.currentHealth == _inzaneChamp.maxHealth) {
						if(_inzaneChamp.dealDamage(20)) {
							InstantiateFireBreath();
							StartCoroutine(SpawnInfoText(20, "Very Not Effective", InsaneSkillzChampion));
						}
					} else if(!_fireGround && !_adrianUsedGroundFire) {
						EffectText.text = "Adrian puts the ground on fire!";
						ResetEffects();
						_adrianUsedGroundFire = true;
						SetGround("FireGround");
						_fireGround = true;
					} else if(_fireGround) {
						if(_inzaneChamp.dealDamage(80)) {
							InstantiateFireBreath();
							StartCoroutine(SpawnInfoText(80, "Effective", InsaneSkillzChampion));
						}
					} else {
						if(_inzaneChamp.dealDamage(40)) {
							InstantiateFireBreath();
							StartCoroutine(SpawnInfoText(40, "Not Effective", InsaneSkillzChampion));
						}
					}
					break;

				default:
					break;
			}

		}

		if(_adrianDead) {
			yield return new WaitForSeconds(2);
			EffectText.text = "";
			if(_currentScene != 3) {
				SceneManager.LoadScene("levelup", LoadSceneMode.Additive);
				yield return new WaitForSeconds(5);
				SceneManager.UnloadSceneAsync("levelup");
			}
			yield return new WaitForSeconds(1);
			_player.UnloadFightScene();
			yield break;
		} else {
			yield return new WaitForSeconds(2);
			RestartIenum();
		}
	}

	private void InstantiateFireBreath() {
		Instantiate(FireBreath, new Vector3(3f, 433f, -6f), Quaternion.identity);
	}

	private void ResetHealths() {
		if(_currentScene == 1) {
			_inzaneChamp.SetHealth(500);
			_adrianChamp.SetHealth(500);
		} else if (_currentScene == 2) {
			_inzaneChamp.SetHealth(750);
			_adrianChamp.SetHealth(750);
		} else {
			_inzaneChamp.SetHealth(1000);
			_adrianChamp.SetHealth(1000);
		}

		_adrianDead = false;
		_adrianChamp.ChanceToHit = 100;
		_inzaneChamp.ChanceToHit = 100;
	}

	private void SetUpButtons() {
		Ability1.onClick.AddListener(delegate { UseAbility1(); });
		Ability2.onClick.AddListener(delegate { UseAbility2(); });
		Ability3.onClick.AddListener(delegate { UseAbility3(); });
	}

	private void RestartIenum() {
		if(_inzaneChamp.ChanceToHit != 100) {
			_inzaneChamp.ChanceToHit = 100;
		}

		EffectText.text = "";
		_playerTurn = true;
		_aniCont.SetBool("Hide", false);
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

	private IEnumerator SpawnInfoText(int dmg, string effect, GameObject dragon) {
		var spawnPos = new Vector3(dragon.transform.position.x, -2f, 1f);
		Transform parent;
		if(dragon.name == "Opponent") {
			parent = AdrianTextSpawnLocation.transform;
		} else {
			parent = InsaneTextSpawnLocation.transform;
		}
		var spawnedText = Instantiate(InfoText, spawnPos, Quaternion.identity, parent);
		spawnedText.transform.localEulerAngles = new Vector3(0f, 90f, 0f);

		spawnedText.transform.GetChild(0).GetComponent<TextMeshPro>().text = dmg.ToString();
		spawnedText.transform.GetChild(1).GetComponent<TextMeshPro>().text = effect;

		yield return new WaitForSeconds(2f);
		spawnedText.GetComponent<Animator>().SetTrigger("Vanish");

		yield return new WaitForSeconds(1.5f);
		Destroy(spawnedText);
	}
}