using UnityEngine;
using UnityEngine.UI;

public class FightChampion : MonoBehaviour {
	public int maxHealth = 1000;
	public int currentHealth = 1000;
	[Range(0, 100)]
	public int lowPercent = 25;

	public MidtermFightSceneController MFSC;

	public Slider healthBar;
	public Text healthText;

	public int ChanceToHit = 100;

	private int tempHealth = 1000;
	private Image healthFill;
	private Color healthBarColor;
	private float healthBlinkDuration = 0.40f;
	private float colorLerpTime = 0f;

	void Start() {
		if(healthBar != null) {
			healthFill = healthBar.fillRect.GetComponent<Image>();
			healthBarColor = healthFill.color;
		}
			
	}
	
	void Update() {
		string health = currentHealth + "/" + maxHealth;
		healthBar.maxValue = maxHealth;
		currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
		float percent = ((float) currentHealth / (float) maxHealth) * 100f;

		if(percent <= lowPercent) {
			if(colorLerpTime < 1f) {
				healthFill.color = Color.Lerp(healthBarColor, new Color(healthBarColor.r, healthBarColor.g, healthBarColor.b, 0f), colorLerpTime);
				colorLerpTime += Time.deltaTime / healthBlinkDuration;
			} else if(colorLerpTime > 1f && colorLerpTime < 2f) {
				healthFill.color = Color.Lerp(healthBarColor, new Color(healthBarColor.r, healthBarColor.g, healthBarColor.b, 0f), 2f - colorLerpTime);
				colorLerpTime += Time.deltaTime / healthBlinkDuration;
				if(colorLerpTime >= 2f)
					colorLerpTime -= 2f;
			}
		} else {
			healthFill.color = healthBarColor;
		}

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
