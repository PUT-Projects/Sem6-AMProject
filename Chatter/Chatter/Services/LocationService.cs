using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;


namespace Chatter.Services;

public class LocationService
{
    public async Task<(string?, string?)> GetCurrentLocationUrlAsync()
    {
        try {
            // Requesting location permission from the user
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted) {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted) {
                    return (null, "Location permission not granted");
                }
            }

            // Getting the current location
            var location = await Geolocation.GetLastKnownLocationAsync();
            if (location is null) {
                location = await Geolocation.GetLocationAsync(new GeolocationRequest {
                    DesiredAccuracy = GeolocationAccuracy.High,
                    Timeout = TimeSpan.FromSeconds(3)
                });
            }

            if (location is not null) {
                // Creating a Google Maps URL using the latitude and longitude
                return ($"https://maps.google.com/?q={location.Latitude.ToString(CultureInfo.InvariantCulture)},{location.Longitude.ToString(CultureInfo.InvariantCulture)}", null);
            }
            else {
                return (null, "Unable to get location");
            }
        }
        catch (Exception ex) {
            // Handle any exceptions that may occur
            return (null, $"Error: {ex.Message}");
        }
    }
}
