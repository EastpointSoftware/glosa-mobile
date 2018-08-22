/*
GLOSA Mobile. Green Light Optimal Speed Adviosry Mobile Application

Copyright © 2017 Eastpoint Software Limited

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

 */

using System.Collections.Generic;
using System.Security;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Util;

using GreenLight.Core;
using GreenLight.Core.Contracts;
using GreenLight.Core.Helpers;
using GreenLight.Core.Models;

namespace GreenLight.Droid.Services
{
    public class GeofenceService : Java.Lang.Object, IGeofenceService, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        private void PopulateGeofenceList()
        {
            IList<kmlDocumentPlacemark> placemarks = KMLHelper.GLOSATestRoute();
            foreach (var placemark in placemarks)
            {
                // https://developers.google.com/kml/documentation/kmlreference?csw=1#coordinates
                var coordinatesString = placemark.Point.coordinates;
                string[] parts = coordinatesString.Split(',');
                double longitude = double.Parse(parts[0]);
                double latitude = double.Parse(parts[1]);

                _geoList.Add(new GeofenceBuilder().SetRequestId(placemark.name).SetCircularRegion(latitude, longitude, Constants.GeofenceDefaultRadiusMeters).SetExpirationDuration(Geofence.NeverExpire).SetTransitionTypes(Geofence.GeofenceTransitionEnter | Geofence.GeofenceTransitionExit).Build());
            }
        }

        public void Connect()
        {
            _googleApiClient.Connect();
        }

        public void Disconnect()
        {
            _googleApiClient.Disconnect();
        }

        public void RegisterServiceForPlaform(object sender)
        {
            _geoList = new List<IGeofence>();
            _geofencePendingIntent = null;

            PopulateGeofenceList();

            _context = sender as Context;
            GoogleApiClient.IConnectionCallbacks connectionCallback = this as GoogleApiClient.IConnectionCallbacks;
            GoogleApiClient.IOnConnectionFailedListener connectionFailedCallback = sender as GoogleApiClient.IOnConnectionFailedListener;
            _googleApiClient = new GoogleApiClient.Builder(_context)
                .AddConnectionCallbacks(connectionCallback)
                .AddOnConnectionFailedListener(connectionFailedCallback)
                .AddApi(LocationServices.API)
                .Build();
        }

        private async void _addGeoFences()
        {
            if (!_googleApiClient.IsConnected)
            {
                Toast.MakeText(_context, "not connect", ToastLength.Short).Show();
                return;
            }

            try
            {
                var status = await LocationServices.GeofencingApi.AddGeofencesAsync(_googleApiClient, GetGeofencingRequest(),
                    GetGeofencePendingIntent());
                HandleResult(status);
            }
            catch (SecurityException securityException)
            {
                throw securityException;
            }
        }

        private async void _removeGeofences()
        {
            if (!_googleApiClient.IsConnected)
            {
                Toast.MakeText(_context, "not connect", ToastLength.Short).Show();
                return;
            }
            try
            {
                var status = await LocationServices.GeofencingApi.RemoveGeofencesAsync(_googleApiClient,
                    GetGeofencePendingIntent());
                HandleResult(status);
            }
            catch (SecurityException securityException)
            {
                throw securityException;
            }
        }

        GeofencingRequest GetGeofencingRequest()
        {
            var builder = new GeofencingRequest.Builder();
            builder.SetInitialTrigger(GeofencingRequest.InitialTriggerEnter);
            builder.AddGeofences(_geoList);

            return builder.Build();
        }
        PendingIntent GetGeofencePendingIntent()
        {
            if (_geofencePendingIntent != null)
            {
                return _geofencePendingIntent;
            }
            var intent = new Intent(_context, typeof(GeofenceTransitionsIntentService));
            return PendingIntent.GetService(_context, 0, intent, PendingIntentFlags.UpdateCurrent);
        }

        public void HandleResult(Statuses status)
        {
            if (status.IsSuccess)
            {
                Toast.MakeText(
                    _context,
                    "success",
                    ToastLength.Short
                ).Show();
            }
            else
            {
                Log.Error("geofence", status.StatusCode.ToString());
            }
        }

        public void OnConnected(Bundle connectionHint)
        {
            Log.Info("GeoFence", "Connected to GoogleApiClient");

            _removeGeofences();
            _addGeoFences();
        }

        public void OnConnectionSuspended(int cause)
        {
            Log.Info("GeoFence", "Connection suspended");
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            Log.Info("GeoFence", "Connection failed: ConnectionResult.getErrorCode() = " + result.ErrorCode);
        }

        #region Member Variables
        Context _context;
        GoogleApiClient _googleApiClient;
        PendingIntent _geofencePendingIntent;
        List<IGeofence> _geoList;
        #endregion
    }
}