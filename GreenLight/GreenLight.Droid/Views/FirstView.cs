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

using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;

using MvvmCross.Droid.Views;

using GreenLight.Core;
using GreenLight.Core.ViewModels;

namespace GreenLight.Droid.Views
{
    [Activity(Label = "Welcome to Green Light Optimal Speed Advisory", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class FirstView : MvxActivity<FirstViewModel>
    {

        #region Construction
        #endregion

        #region App Life-Cycle

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //A great place to initialize Xamarin.Insights and Dependency Services!
            AppCenter.Start(Constants.AZURE_APP_CENTER_ANDROID_KEY, typeof(Analytics), typeof(Crashes), typeof(Distribute));

            SetContentView(Resource.Layout.FirstView);

            Window.SetFlags(WindowManagerFlags.KeepScreenOn, Android.Views.WindowManagerFlags.KeepScreenOn);

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

            this.ViewModel.LocationPermissionGranted = false;

            //https://blog.xamarin.com/requesting-runtime-permissions-in-android-marshmallow/
            RequestLocationPermissions();
        }

        protected override void OnStart()
        {
            base.OnStart();
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
                this.ViewModel.LocationPermissionGranted = true;
                return;
            }

            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, permission))
            {
                var layout = FindViewById<LinearLayout>(Resource.Layout.FirstView);
                //Explain to the user why we need to read the contacts
                Snackbar.Make(layout, "Location access is required to use the GLOSA App.", Snackbar.LengthIndefinite)
                        .SetAction("OK", v => ActivityCompat.RequestPermissions(this, PermissionsLocation, RequestLocationId))
                        .Show();

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
                        var layout = FindViewById<RelativeLayout>(Resource.Layout.FirstView);
                        if (grantResults[0] == Permission.Granted)
                        {
                            //Permission granted
                            this.ViewModel.LocationPermissionGranted = true;
                            if (layout != null)
                            {
                                var snack = Snackbar.Make(layout, "Location permission is available, getting lat/long.", Snackbar.LengthShort);
                                snack.Show();
                            }
                        }
                        else
                        {
                            //Permission Denied :(
                            //Disabling location functionality
                            if (layout != null)
                            {
                                var snack = Snackbar.Make(layout, "Location permission is denied.", Snackbar.LengthShort);
                                snack.Show();
                            }
                        }
                    }
                    break;
            }
        }

        #endregion
    }
}
