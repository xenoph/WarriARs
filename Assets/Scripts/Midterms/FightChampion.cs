using UnityEngine;
using UnityEngine.UI;

public class FightChampion : MonoBehaviour {
	public int maxHealth = 1000;
	public int currentHealth = 1000;

	public MidtermFightSceneController MFSC;

	public Slider healthBar;
	public Text healthText;

	public int ChanceToHit = 100;

	private int tempHealth = 1000;

	void Start() {
		
	}
	
	void Update() {
		string health = currentHealth + "/" + maxHealth;
		healthBar.maxValue = maxHealth;
		currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
		
		tempHealth = Mathf.RoundToInt(Mathf.Lerp((float) tempHealth, (float) currentHealth, .1f));
		healthBar.value = tempHealth;
		if(healthText.text != health)
			healthText.text = health;
	}

	public void SetHealth(int health) {
		maxHealth = health;
		currentHealth = health;
	}

	public bool dealDamage(int dmg) {
		var name = transform.GetChild(1).GetComponent<Text>().text;
		if(ChanceToHit < 100) {
			var actualHit = Random.Range(1, 100);
			if(actualHit > ChanceToHit) {
				if(name == "Adrian") {
					MFSC.EffectText.text = MFSC.transform.GetChild(1).transform.GetChild(1).GetComponent<Text>().text + " misses!";
				} else {
					MFSC.EffectText.text = "Adrian misses!";
				}
				return false;
			}
		}
		currentHealth -= dmg;
		return true;
	}

	public bool CheckDead() {
		if(currentHealth <= 0) { return true; }
		return false;
	}
}
