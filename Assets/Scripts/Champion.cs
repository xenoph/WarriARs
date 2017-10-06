using UnityEngine;

public class Champion : MonoBehaviour {
	public ChampionTypes type = ChampionTypes.Water;
	public string championName, englishChampionName;
	public int health, attack, defence, speed;

	void Start() {
		
	}
	
	void Update() {
		
	}

	public enum ChampionTypes {
		Wood, Earth, Water, Fire, Metal
	}
}
