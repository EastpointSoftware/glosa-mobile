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

            int buttonDirectionId = Resource.Id.radio_ANY;
            switch (Settings.RouteDirectionOption)
            {
                case Constants.SETTINGS_ROUTE_DIRECTION_ANY:
                    buttonDirectionId = Resource.Id.radio_ANY;
                    break;
                case Constants.SETTINGS_ROUTE_DIRECTION_NS:
                    buttonDirectionId = Resource.Id.radio_NS;
                    break;
                case Constants.SETTINGS_ROUTE_DIRECTION_SN:
                    buttonDirectionId = Resource.Id.radio_SN;
                    break;
                case Constants.SETTINGS_ROUTE_DIRECTION_EW:
                    buttonDirectionId = Resource.Id.radio_EW;
                    break;
                case Constants.SETTINGS_ROUTE_DIRECTION_WE:
                    buttonDirectionId = Resource.Id.radio_WE;
                    break;
                default:
                    break;
            }

            var button = FindViewById<RadioButton>(buttonDirectionId);
            button.Checked = true;
        }

        private void Directions_ButtonGroup_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            int directionId = Resource.Id.radio_ANY;
            switch (e.CheckedId)
            {
                case Resource.Id.radio_ANY:
                    directionId = Constants.SETTINGS_ROUTE_DIRECTION_ANY;
                    break;
                case Resource.Id.radio_NS:
                    directionId = Constants.SETTINGS_ROUTE_DIRECTION_NS;
                    break;
                case Resource.Id.radio_SN:
                    directionId = Constants.SETTINGS_ROUTE_DIRECTION_SN;
                    break;
                case Resource.Id.radio_EW:
                    directionId = Constants.SETTINGS_ROUTE_DIRECTION_EW;
                    break;
                case Resource.Id.radio_WE:
                    directionId = Constants.SETTINGS_ROUTE_DIRECTION_WE;
                    break;
                default:
                    break;
            }

            Settings.RouteDirectionOption = directionId;
        }

        private bool _activatedInternally = false;
    }
}