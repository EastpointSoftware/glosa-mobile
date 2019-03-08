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
using System;

using Android.Content;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Gms.Common;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;

using MvvmCross.Droid.Views;

using GreenLight.Core.ViewModels;
using MvvmCross.Platform;
using GreenLight.Core.Contracts;

namespace GreenLight.Droid.Views
{
	/// <summary>
	/// Display speed advisory.
	/// </summary>
	[Activity(Label = "SpeedAdvisoryView", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
	public class SpeedAdvisoryView : MvxActivity<SpeedAdvisoryViewModel>
    {
		/// <summary>
		/// Called when the activity is starting.
		/// </summary>
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.SpeedAdvisoryViewBasic);

            Window.SetFlags(Android.Views.WindowManagerFlags.KeepScreenOn, Android.Views.WindowManagerFlags.KeepScreenOn);

            //_broadcastReceiver = new GeofenceBroadcastReceiver();
            //_broadcastReceiver.GeofenceBroadcastEventHandler += BroadcastReceiver_GeofenceBroadcastEventHandler;

            //_geofenceService = Mvx.Resolve<IGeofenceService>();
            //_geofenceService.RegisterServiceForPlaform(this);
        }

        private void BroadcastReceiver_GeofenceBroadcastEventHandler(object sender, GeofenceBroadcastEvenEventArgs e)
        {
            //ViewModel.GeoFenceUpdateEvent(e.IntersectionId, e.DidEnter, e.DidExit);
        }

        protected override void OnResume()
        {
            base.OnResume();
            RequestLocationPermissions();
            ViewModel.Refresh();
            //RegisterReceiver(_broadcastReceiver, new IntentFilter("uk.co.eastpoint.GeofenceBroadcast"));
            // Code omitted for clarity
        }

        protected override void OnPause()
        {
           // UnregisterReceiver(_broadcastReceiver);
            // Code omitted for clarity
            base.OnPause();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        readonly string[] PermissionsLocation =
        {
          Manifest.Permission.AccessCoarseLocation,
          Manifest.Permission.AccessFineLocation
        };

        const int RequestLocationId = 0;

        void RequestLocationPermissions()
        {
            const string permission = Manifest.Permission.AccessFineLocation;
            if (ContextCompat.CheckSelfPermission(this, permission) == (int)Permission.Granted)
            {
                Mvx.Resolve<ILocationService>().Start();
                return;
            }

            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, permission))
            {
                return;
            }

            ActivityCompat.RequestPermissions(this, PermissionsLocation, RequestLocationId);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestLocationId:
                    {
                        if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                        {
                            Mvx.Resolve<ILocationService>().Start();
                            //Permission granted
                        }
                        else
                        {
                            //Permission Denied :(
                            //Disabling location functionality
                        }
                    }
                    break;
            }
        }

        #region Implementation

        protected override void OnStart()
        {
            base.OnStart();
            //if (_geofenceService != null)
            //{
            //    _geofenceService.Connect();
            //}
        }

        protected override void OnStop()
        {
            base.OnStop();
            //if (_geofenceService != null)
            //{
            //    _geofenceService.Disconnect();
            //}
        }

        public void OnConnected(Bundle connectionHint)
        {
            
        }

        public void OnConnectionSuspended(int cause)
        {
            
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            
        }

        #endregion

        #region Member Variables
        //IGeofenceService _geofenceService;
        //GeofenceBroadcastReceiver _broadcastReceiver;

        //bool _didEnterIntersection = false;
        //bool _didExitIntersection = false;
        //string _intersectionId = "";
        #endregion
    }
}

[BroadcastReceiver(Enabled = true)]
[IntentFilter(new[] { "uk.co.eastpoint.GeofenceBroadcast" })]
public class GeofenceBroadcastReceiver : BroadcastReceiver
{
    public event System.EventHandler<GeofenceBroadcastEvenEventArgs> GeofenceBroadcastEventHandler;

    public override void OnReceive(Context context, Intent intent)
    {
        string getGeofenceTransitionDetails = intent.GetStringExtra("GetGeofenceTransitionDetails");
        Toast.MakeText(
                context,
                getGeofenceTransitionDetails,
                ToastLength.Short
            ).Show();

        if (intent.HasExtra("GetGeofenceIntersectionExited"))
        {
            string exitedIntersection = intent.GetStringExtra("GetGeofenceIntersectionExited");
            GeofenceBroadcastEventHandler?.Invoke(this, new GeofenceBroadcastEvenEventArgs() { IntersectionId = exitedIntersection, DidExit = true });
        }
    }
}

public class GeofenceBroadcastEvenEventArgs : EventArgs
{
    public string IntersectionId { get; set; }
    public bool DidEnter { get; set; }
    public bool DidExit { get; set; }
}