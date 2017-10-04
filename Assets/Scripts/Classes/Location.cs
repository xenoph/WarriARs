using UnityEngine;
using System.Collections;

public class Location {
	public double Latitude { get; set; }
    public double Longitude { get; set; }

	public bool IsActive { get; set; }
    public bool Failed { get; set; }

	public Location() {}

	public void UpdateLocations() {
        Latitude = Input.location.lastData.latitude;
        Longitude = Input.location.lastData.longitude;
    }

    public void SetLocation(double lat, double lon) {
        Latitude = lat;
        Longitude = lon;
    }

    public IEnumerator StartLocationServices() {
        if(!Input.location.isEnabledByUser) {
			IsActive = false;
			Failed = true;
            yield break;
        }

        Input.location.Start();

        int waitCountdownInSeconds = 20;
        while(Input.location.status == LocationServiceStatus.Initializing && waitCountdownInSeconds > 0) {
            yield return new WaitForSeconds(1);
            waitCountdownInSeconds--;
        }

        if(waitCountdownInSeconds < 1) {
            yield break;
        }

        if(Input.location.status == LocationServiceStatus.Failed) {
			IsActive = false;
			Failed = true;
            yield break;
        }

        UpdateLocations();
        IsActive = true;
		Failed = false;

        yield break;
    }
}
