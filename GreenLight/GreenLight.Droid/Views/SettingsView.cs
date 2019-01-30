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

using Android.App;
using Android.OS;
using Android.Widget;

using MvvmCross.Droid.Views;

using GreenLight.Core;
using GreenLight.Core.ViewModels;
using GreenLight.Core.Helpers;

namespace GreenLight.Droid.Views
{
    [Activity(Label = "SettingsView", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SettingsView : MvxActivity<SettingsViewModel>
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SettingsView);

            // Maximum invoke distance
            //var seekMaxiumumDistance = this.FindViewById<SeekBar>(Resource.Id.seek_invoke_distance);
            //seekMaxiumumDistance.Max = Constants.CALCULATOR_MAXIMUM_DISTANCE_CHECK_METERS - Constants.CALCULATOR_MINIMUM_DISTANCE_CHECK_METERS;

            _activatedInternally = true;

            // Direction
            _UpdateDirectionsRadioGroup();

            _activatedInternally = false;
        }

        private void _UpdateDirectionsRadioGroup()
        {
            var buttonGroupDirections = this.FindViewById<RadioGroup>(Resource.Id.radio_group_direction);
            buttonGroupDirections.CheckedChange += Directions_ButtonGroup_CheckedChange;

            int buttonDirectionId = Resource.Id.radio_IB;
            switch (Settings.RouteDirectionOption)
            {
                case Constants.SETTINGS_ROUTE_DIRECTION_InBound:
                    buttonDirectionId = Resource.Id.radio_IB;
                    break;
                case Constants.SETTINGS_ROUTE_DIRECTION_OutBound:
                    buttonDirectionId = Resource.Id.radio_OB;
                    break;
                    break;
                default:
                    break;
            }

            var button = FindViewById<RadioButton>(buttonDirectionId);
            button.Checked = true;
        }

        private void Directions_ButtonGroup_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            int directionId = Constants.SETTINGS_ROUTE_DIRECTION_ANY;
            switch (e.CheckedId)
            {
                case Resource.Id.radio_IB:
                    directionId = Constants.SETTINGS_ROUTE_DIRECTION_InBound;
                    break;
                case Resource.Id.radio_OB:
                    directionId = Constants.SETTINGS_ROUTE_DIRECTION_OutBound;
                    break;
                default:
                    break;
            }

            Settings.RouteDirectionOption = directionId;
        }

        private bool _activatedInternally = false;
    }
}