using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpController : MonoBehaviour {

	public GameObject LevelUpTopPanel;
	public GameObject LevelUpMidPanel;
	public GameObject LevelUpBottomPanel;

	public TextMeshProUGUI TopHeaderText;
	public Image ExperienceBar;
	public TextMeshProUGUI ExperienceText;

	public TextMeshProUGUI MidLevelText;

	public TextMeshProUGUI StatAttackText;
	public TextMeshProUGUI StatDefenseText;
	public TextMeshProUGUI StatHealthText;

	public void SetTopPanel(int current, int total, string champion) {
		ExperienceBar.fillAmount = (float)current / (float)total;
		ExperienceText.text = "XP " + current.ToString() + "/" + total.ToString();
	}

	public void SetMidPanel(string level) {
		MidLevelText.text = level;
	}

	public void SetBottomPanel(int atk, int atkNew, int def, int defNew, int hp, int hpNew) {
		StatAttackText.text = "Attack: " + atk + "(" + atkNew + ")";
		StatDefenseText.text = "Defense: " + def + "(" + defNew + ")";
		StatHealthText.text = "Health: " + hp + "(" + hpNew + ")";
	}	
}