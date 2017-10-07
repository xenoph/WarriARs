using UnityEngine;
using UnityEngine.UI;

public class FightChampion : MonoBehaviour {
	public int maxHealth = 1000;
	public int currentHealth = 1000;

	public Slider healthBar;
	public Text healthText;

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

	public void dealDamage(int dmg) {
		currentHealth -= dmg;
	}
}
