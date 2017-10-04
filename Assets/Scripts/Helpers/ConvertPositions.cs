using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;
using Mapbox.Unity.Map;

public static class ConvertPositions {

	/// <summary>
	/// Converts a Geo location to a Vector3 position on an abstract map
	/// </summary>
	/// <param name="loc">GPS Location</param>
	/// <param name="map">Loaded Abstract map</param>
	/// <returns></returns>
	public static Vector3 ConvertLocationToVector3(Location loc, AbstractMap map) {
		var llpos = new Vector2d(loc.Latitude, loc.Longitude);
		var worldPos = Conversions.GeoToWorldPosition(llpos, map.CenterMercator, map.WorldRelativeScale);
		return new Vector3((float)worldPos.x, 0.375f, (float)worldPos.y);
	}
}
