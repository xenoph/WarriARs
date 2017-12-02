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

	public Button CloseButton;

	/// <summary>
	/// Set the top part of the level up panel which consists of the experience text + bar
	/// </summary>
	/// <param name="current"></param>
	/// <param name="total"></param>
	/// <param name="champion"></param>
	public void SetTopPanel(int current, int total, string champion) {
		ExperienceBar.fillAmount = (float)current / (float)total;
		ExperienceText.text = "XP " + current.ToString() + "/" + total.ToString();
	}

	/// <summary>
	/// Sets the middle part of the level up panel, which is just a big number with the current level
	/// </summary>
	/// <param name="level"></param>
	public void SetMidPanel(string level) {
		MidLevelText.text = level;
	}

	/// <summary>
	/// Sets the bottom part of the level up panel, which showcases the stats
	/// </summary>
	/// <param name="atk"></param>
	/// <param name="atkNew"></param>
	/// <param name="def"></param>
	/// <param name="defNew"></param>
	/// <param name="hp"></param>
	/// <param name="hpNew"></param>
	public void SetBottomPanel(int atk, int atkNew, int def, int defNew, int hp, int hpNew) {
		StatAttackText.text = "Attack: " + atk + "(" + atkNew + ")";
		StatDefenseText.text = "Defense: " + def + "(" + defNew + ")";
		StatHealthText.text = "Health: " + hp + "(" + hpNew + ")";
	}	

	/// <summary>
	/// Closes the level up panel when the Player clicks the button
	/// </summary>
	public void CloseLevelUp() {
		GameController.instance.InterfaceController.ToggleLevelUp();
	}
}