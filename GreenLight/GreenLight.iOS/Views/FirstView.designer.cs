// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace GreenLight.iOS.Views
{
    [Register ("FirstView")]
    partial class FirstView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ButtonSettings { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ButtonSpeedAdvisory { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelAppVersion { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelLocationPermission { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ButtonSettings != null) {
                ButtonSettings.Dispose ();
                ButtonSettings = null;
            }

            if (ButtonSpeedAdvisory != null) {
                ButtonSpeedAdvisory.Dispose ();
                ButtonSpeedAdvisory = null;
            }

            if (LabelAppVersion != null) {
                LabelAppVersion.Dispose ();
                LabelAppVersion = null;
            }

            if (LabelLocationPermission != null) {
                LabelLocationPermission.Dispose ();
                LabelLocationPermission = null;
            }
        }
    }
}