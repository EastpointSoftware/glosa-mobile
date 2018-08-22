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

using MvvmCross.iOS.Views;
using MvvmCross.Binding.BindingContext;

using GreenLight.Core;
using GreenLight.Core.ViewModels;
using GreenLight.Core.Helpers;

namespace GreenLight.iOS.Views
{
    [MvxFromStoryboard]
    public partial class SettingsView : MvxTableViewController
    {
        protected SettingsViewModel SettingsViewModel => ViewModel as SettingsViewModel;

        public SettingsView(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            var set = this.CreateBindingSet<SettingsView, SettingsViewModel>();
            set.Bind(SwitchEnableGLOSA).To(vm => vm.EnableGLOSAAdvisory);
            set.Bind(SwitchEnableTextToSpeech).To(vm => vm.EnableTextToSpeech);
            set.Bind(TextFieldVehicleId).To(vm => vm.VehicleId).TwoWay();
            set.Bind(SwitchEnableTestRoute).To(vm => vm.EnableTestRoute);
            set.Bind(LabelSpeedLimit).To(vm => vm.SpeedLimit);
            set.Bind(SwitchEnableIntersectionMode).To(vm => vm.EnableIntersectionMode);
            set.Bind(TextFieldIntersectionId).To(vm => vm.IntersectionId);
            set.Bind(SwitchSyncSNTP).To(vm => vm.EnableGPSTime);
            set.Apply();

            _UpdateDirectionsSegmentControl();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        private void _UpdateDirectionsSegmentControl()
        {
            this.SegmentSimulatedDirection.ValueChanged += ButtonGroupDirections_ValueChanged;

            int directionId = 0;
            switch (Settings.RouteDirectionOption)
            {
                case Constants.SETTINGS_ROUTE_DIRECTION_ANY:
                    directionId = 4;
                    break;
                case Constants.SETTINGS_ROUTE_DIRECTION_NS:
                    directionId = 0;
                    break;
                case Constants.SETTINGS_ROUTE_DIRECTION_SN:
                    directionId = 1;
                    break;
                case Constants.SETTINGS_ROUTE_DIRECTION_EW:
                    directionId = 2;
                    break;
                case Constants.SETTINGS_ROUTE_DIRECTION_WE:
                    directionId = 3;
                    break;
                default:
                    break;
            }

            this.SegmentSimulatedDirection.SelectedSegment = directionId;
        }

        void ButtonGroupDirections_ValueChanged(object sender, EventArgs e)
        {
            int directionId = 0;
            switch (this.SegmentSimulatedDirection.SelectedSegment)
            {
                case 4:
                    directionId = Constants.SETTINGS_ROUTE_DIRECTION_ANY;
                    break;
                case 0:
                    directionId = Constants.SETTINGS_ROUTE_DIRECTION_NS;
                    break;
                case 1:
                    directionId = Constants.SETTINGS_ROUTE_DIRECTION_SN;
                    break;
                case 2:
                    directionId = Constants.SETTINGS_ROUTE_DIRECTION_EW;
                    break;
                case 3:
                    directionId = Constants.SETTINGS_ROUTE_DIRECTION_WE;
                    break;
                default:
                    break;
            }

            Settings.RouteDirectionOption = directionId;
        }

    }
}

