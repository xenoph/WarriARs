using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConvertChampionNumberToString {

	public static string GetChampionPrefab(int num) {
		switch (num) {
			case 0:
				return "PRE_Champion_Fire";

			case 1:
				return "PRE_Champion_Water";

			case 2:
				return "PRE_Champion_Wood";

			case 3:
				return "PRE_Champion_Earth";

			case 4:
				return "PRE_Champion_Metal";

			default:
				return null;
		}
	}
}