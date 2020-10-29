﻿using GoogleMaps.LocationServices;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace localhostUI.Classes.LocationClasses
{
    static class LocationInformation
    {
        private const string apiKey = "AIzaSyAZi39nB5EAaAgj3fdiCRwZTbY7lxIO-0Y";
        public static MapPoint LatLongFromString(this string address, string country = "Lithuana")
        {
            AddressData addressObject = new AddressData
            {
                Address = address,
                State = null,
                Country = country
            };

            var locationService = new GoogleLocationService(apiKey);
            return locationService.GetLatLongFromAddress(addressObject);
        }
        public static MapPoint LatLongFromAddress(string address, string country = "Lithuania")
        {
            AddressData addressObject = new AddressData
            {
                Address = address,
                State = null,
                Country = country
            };

            var locationService = new GoogleLocationService(apiKey);
            return locationService.GetLatLongFromAddress(addressObject);
        }
        public static AddressData AddressFromLatLong(double latitude, double longitude)
        {
            var locationService = new GoogleLocationService(apiKey);
            return locationService.GetAddressFromLatLang(latitude, longitude);
        }
        public static AddressData AddressFromLatLong(MapPoint latLong)
        {
            var locationService = new GoogleLocationService(apiKey);
    
            return locationService.GetAddressFromLatLang(latLong.Latitude, latLong.Longitude);
        }
        public static string FormatAddress(string address)
        {
            var locationService = new GoogleLocationService(apiKey);
            var addressData = locationService.GetAddressesListFromAddress(address);
            return addressData[0];
        }
        public static void OpenAdressInBrowser(string address)
        {
            //Deleted the "," from the array, so if error pile up, add it back in.
            char[] invalidCharacters = "!*'();:@&=+$/?%#[]".ToCharArray();
            string url = "https://www.google.lt/maps/search/";
            string temp = address.Replace(' ', '+');
            temp.Replace(",", "%2C");
            url += temp;
            //Testing output

            if (address.Length == 0)
            {
                return;
            }
            else if (address.Any(c => invalidCharacters.Contains(c)))
            {
                MessageBox.Show("Your adress contains characters which are not allowed.\nUnallowed characters: ! * ' ( ) ; : @ & = + $ / ? % # [ ]", "Illegal characters found.",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Constructing the link.


            //Searches up the location on your browser on google maps.
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        //Returned values are in meters
        public static double Distance(MapPoint user, MapPoint destination)
        {
            return Distance(user.Latitude, user.Longitude, destination.Latitude, destination.Longitude);
        }
        public static double Distance(double userLatitude, double userLongitude, double destinationLatitude, double destinationLongitude)
        {
            try
            {
                double resultLatitude = MathSupplement.DegreesToRadians(destinationLatitude - userLatitude);
                double resultLongitude = MathSupplement.DegreesToRadians(destinationLongitude - userLongitude);

                userLatitude = MathSupplement.DegreesToRadians(userLatitude);
                destinationLatitude = MathSupplement.DegreesToRadians(destinationLatitude);

                double coordinateDestination = Math.Pow(Math.Sin(resultLatitude / 2), 2) +
                                            Math.Pow(Math.Sin(resultLongitude / 2), 2) * Math.Cos(userLatitude) * Math.Cos(destinationLatitude);
                double tangededDestination = 2 * Math.Atan2(Math.Sqrt(coordinateDestination), Math.Sqrt(1 - coordinateDestination));

                return MathSupplement.EarthRadius * tangededDestination * 1000;
            }
            catch (Exception ex)
            {
                return -1;
            }

        }

        public static bool IsValidAddress(string address)
        {
            try
            {
                MapPoint location = LocationInformation.LatLongFromAddress(address);
                location.ToString();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
    }
}
