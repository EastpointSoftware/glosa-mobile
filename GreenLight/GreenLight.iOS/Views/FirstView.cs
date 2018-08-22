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

using UIKit;

using MvvmCross;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;

using GreenLight.Core.ViewModels;

namespace GreenLight.iOS.Views
{
    [MvxFromStoryboard]
    public partial class FirstView : MvxViewController
    {
        protected FirstViewModel FirstViewModel => ViewModel as FirstViewModel;

        public FirstView(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            var set = this.CreateBindingSet<FirstView, FirstViewModel>();
            set.Bind(ButtonSpeedAdvisory).To(vm => vm.ShowSpeedAdvisorykCommand);
            set.Bind(ButtonSettings).To(vm => vm.ShowSettingsCommand);
            set.Bind(LabelLocationPermission).To(vm => vm.LocationPermissionStatus);
            set.Apply();

			FirstViewModel.LocationPermissionStatus = "";
        }

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			FirstViewModel.LocationPermissionStatus = "";
		}

		private void ButtonSendBroadcast_TouchUpInside(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
