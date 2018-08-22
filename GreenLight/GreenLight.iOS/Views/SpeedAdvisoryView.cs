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
    public partial class SpeedAdvisoryView : MvxViewController
    {
        protected SpeedAdvisoryViewModel SpeedAdvisoryViewModel => ViewModel as SpeedAdvisoryViewModel;

        public SpeedAdvisoryView(IntPtr handle) : base(handle)
        {
              
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<SpeedAdvisoryView, SpeedAdvisoryViewModel>();
            set.Bind(TextFieldTTLMessage).To(vm => vm.GLOSAMessage);
            set.Bind(LabelTTLTime).To(vm => vm.SignalCountDownTime);
            set.Bind(LabelAdvisoryMessage).To(vm => vm.AdvisorySpeedMessage);
            set.Bind(LabelSpeed).To(vm => vm.CurrentSpeed);
            set.Bind(LabelIntersection).To(vm => vm.Intersection);
            set.Apply();
        }
    }
}