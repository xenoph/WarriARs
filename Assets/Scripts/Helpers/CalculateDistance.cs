using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;

public static class CalculateDistance {

	public static int ReturnCalculatedDistance(double lat1, double lng1, double lat2, double lng2){
        double a = (lat1-lat2)*DistancePerLatitude(lat1);
        double b = (lng1-lng2)*DistancePerLongitude(lat1);
	    double result = Math.Sqrt(a * a + b * b);
        return (int) Math.Round(result);
    }

    private static double DistancePerLongitude(double lat){
    return 0.0003121092*Math.Pow(lat, 4)
            +0.0101182384*Math.Pow(lat, 3)
                -17.2385140059*lat*lat
            +5.5485277537*lat+111301.967182595;
    }

    private static double DistancePerLatitude(double lat){
            return -0.000000487305676*Math.Pow(lat, 4)
                -0.0033668574*Math.Pow(lat, 3)
                +0.4601181791*lat*lat
                -1.4558127346*lat+110579.25662316;
    }
}