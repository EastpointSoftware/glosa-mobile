// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace GreenLight.iOS.Views
{
    [Register ("SpeedAdvisoryView")]
    partial class SpeedAdvisoryView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelAdvisoryMessage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelIntersection { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelSpeed { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelTTLTime { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView TextFieldTTLMessage { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (LabelAdvisoryMessage != null) {
                LabelAdvisoryMessage.Dispose ();
                LabelAdvisoryMessage = null;
            }

            if (LabelIntersection != null) {
                LabelIntersection.Dispose ();
                LabelIntersection = null;
            }

            if (LabelSpeed != null) {
                LabelSpeed.Dispose ();
                LabelSpeed = null;
            }

            if (LabelTTLTime != null) {
                LabelTTLTime.Dispose ();
                LabelTTLTime = null;
            }

            if (TextFieldTTLMessage != null) {
                TextFieldTTLMessage.Dispose ();
                TextFieldTTLMessage = null;
            }
        }
    }
}