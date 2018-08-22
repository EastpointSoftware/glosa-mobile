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
    [Register ("SettingsView")]
    partial class SettingsView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelSpeedLimit { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl SegmentLaneManever { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl SegmentSimulatedDirection { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch SwitchEnableGLOSA { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch SwitchEnableIntersectionMode { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch SwitchEnableTestRoute { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch SwitchEnableTextToSpeech { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch SwitchSyncSNTP { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField TextFieldIntersectionId { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField TextFieldVehicleId { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (LabelSpeedLimit != null) {
                LabelSpeedLimit.Dispose ();
                LabelSpeedLimit = null;
            }

            if (SegmentLaneManever != null) {
                SegmentLaneManever.Dispose ();
                SegmentLaneManever = null;
            }

            if (SegmentSimulatedDirection != null) {
                SegmentSimulatedDirection.Dispose ();
                SegmentSimulatedDirection = null;
            }

            if (SwitchEnableGLOSA != null) {
                SwitchEnableGLOSA.Dispose ();
                SwitchEnableGLOSA = null;
            }

            if (SwitchEnableIntersectionMode != null) {
                SwitchEnableIntersectionMode.Dispose ();
                SwitchEnableIntersectionMode = null;
            }

            if (SwitchEnableTestRoute != null) {
                SwitchEnableTestRoute.Dispose ();
                SwitchEnableTestRoute = null;
            }

            if (SwitchEnableTextToSpeech != null) {
                SwitchEnableTextToSpeech.Dispose ();
                SwitchEnableTextToSpeech = null;
            }

            if (SwitchSyncSNTP != null) {
                SwitchSyncSNTP.Dispose ();
                SwitchSyncSNTP = null;
            }

            if (TextFieldIntersectionId != null) {
                TextFieldIntersectionId.Dispose ();
                TextFieldIntersectionId = null;
            }

            if (TextFieldVehicleId != null) {
                TextFieldVehicleId.Dispose ();
                TextFieldVehicleId = null;
            }
        }
    }
}