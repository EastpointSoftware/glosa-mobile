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

using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace GreenLight.Core.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string SettingsKey = "settings_key";
        private const string TokenSettingsKey = "token_settings_key";
        private const string TextToSpeechSettingsKey = "textToSpeech_settings_key";
        private const string SPATTimeIntervalConfidenceSettingsKey = "spatTimeIntervalConfidence_settings_key";
        private const string UniqueVehicleDeviceAppIdSettingsKey = "unique_vehicle_device_app_id_settings_key";
        private const string AdvancedCalculatorEnabledSettingsKey = "advanced_calculator_enabled_settings_key";
        private const string MaximumInvokeDistanceSettingsKey = "maxium_invoke_distance_settings_key";
        private const string TestRouteEnabledSettingsKey = "test_route_enabled_settings_key";
        private const string SpeedLimitSettingsKey = "speed_limit_settings_key";
        private const string TestIntersectionModeEnabledSettingsKey = "test_intersection_enabled_settings_key";
        private const string IntersectionIdSettingsKey = "intersectionid_enabled_settings_key";
        private const string VehicleDirectionManeuverSettingsKey = "vehicle_direction_maneuver_settings_key";
        private const string RouteDirectionSettingsKey = "route_direction_settings_key";
        private const string SNTPSyncEnabledSettingsKey = "sntp_sync_enabled_settings_key";
        private const string GLOSAAdvisroyEnabledSettingsKey = "glosa_advisory_enabled_settings_key";

        private const string EnableWiFiModeSettingsKey = "enable_wifi_mode_settings_key";
        private const string EnableWiFiModeOnlySettingsKey = "enable_wifi_mode_only_settings_key";
        private const string WiFiNetwrokNameSettingsKey = "wifi_network_settings_key";
        private const string WiFiNetworkPasswordSettingsKey = "wifi_network_password_settings_key";

        private static readonly string SettingsDefault = string.Empty;
        private static readonly bool SettingBoolDefault = false;
        private static readonly int SettingsIntDefault = 0;
        #endregion

        public static string GeneralSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault(SettingsKey, SettingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(SettingsKey, value);
            }
        }

        public static string AuthorizationToken
        {
            get
            {
                return AppSettings.GetValueOrDefault(TokenSettingsKey, SettingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(TokenSettingsKey, value);
            }
        }

        public static bool EnableTextToSpeech
        {
            get
            {
                return AppSettings.GetValueOrDefault(TextToSpeechSettingsKey, SettingBoolDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(TextToSpeechSettingsKey, value);
            }
        }

        public static bool EnableAdvancedCalculator
        {
            get
            {
                return AppSettings.GetValueOrDefault(AdvancedCalculatorEnabledSettingsKey, SettingBoolDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(AdvancedCalculatorEnabledSettingsKey, value);
            }
        }

        public static int SPATTimeIntervalConfidence
        {
            get
            {
                return AppSettings.GetValueOrDefault(SPATTimeIntervalConfidenceSettingsKey, SettingsIntDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(SPATTimeIntervalConfidenceSettingsKey, value);
            }
        }

        public static string UniqueVehicleDeviceAppId
        {
            get
            {
                return AppSettings.GetValueOrDefault(UniqueVehicleDeviceAppIdSettingsKey, SettingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(UniqueVehicleDeviceAppIdSettingsKey, value);
            }
        }

        public static int MaximumInvokeDistance
        {
            get
            {
                return AppSettings.GetValueOrDefault(MaximumInvokeDistanceSettingsKey, 0);
            }
            set
            {
                AppSettings.AddOrUpdateValue(MaximumInvokeDistanceSettingsKey, value);
            }
        }

        public static bool EnableTestRoute
        {
            get
            {
                return AppSettings.GetValueOrDefault(TestRouteEnabledSettingsKey, true);
            }
            set
            {
                AppSettings.AddOrUpdateValue(TestRouteEnabledSettingsKey, value);
            }
        }

        public static int SpeedLimit
        {
            get
            {
                return AppSettings.GetValueOrDefault(SpeedLimitSettingsKey, 40);
            }
            set
            {
                AppSettings.AddOrUpdateValue(SpeedLimitSettingsKey, value);
            }
        }

        public static bool EnableIntersectionMode
        {
            get
            {
                return AppSettings.GetValueOrDefault(TestIntersectionModeEnabledSettingsKey, false);
            }
            set
            {
                AppSettings.AddOrUpdateValue(TestIntersectionModeEnabledSettingsKey, value);
            }
        }

        public static string IntersectionId
        {
            get
            {
                return AppSettings.GetValueOrDefault(IntersectionIdSettingsKey, Constants.TEST_INTERSECTION_ID);
            }
            set
            {
                AppSettings.AddOrUpdateValue(IntersectionIdSettingsKey, value);
            }
        }

        public static ulong VehicleManeuverDirection
        {
            get
            {
                return (ulong)AppSettings.GetValueOrDefault(VehicleDirectionManeuverSettingsKey, System.Convert.ToDecimal(Constants.MANEUVER_DIRECTION_AHEAD));
            }
            set
            {
                AppSettings.AddOrUpdateValue(VehicleDirectionManeuverSettingsKey, System.Convert.ToDecimal(value));
            }
        }

        public static int RouteDirectionOption
        {
            get
            {
                return AppSettings.GetValueOrDefault(RouteDirectionSettingsKey, 0);
            }
            set
            {
                AppSettings.AddOrUpdateValue(RouteDirectionSettingsKey, value);
            }
        }

        public static bool SNTPSyncEnabled
        {
            get
            {
                return AppSettings.GetValueOrDefault(SNTPSyncEnabledSettingsKey, true);
            }
            set
            {
                AppSettings.AddOrUpdateValue(SNTPSyncEnabledSettingsKey, value);
            }
        }

        public static bool GLOSAAdvisoryEnabled
        {
            get
            {
                return AppSettings.GetValueOrDefault(GLOSAAdvisroyEnabledSettingsKey, true);
            }
            set
            {
                AppSettings.AddOrUpdateValue(GLOSAAdvisroyEnabledSettingsKey, value);
            }
        }

        public static bool EnableWiFiMode
        {
            get
            {
                return AppSettings.GetValueOrDefault(EnableWiFiModeSettingsKey, false);
            }
            set
            {
                AppSettings.AddOrUpdateValue(EnableWiFiModeSettingsKey, value);
            }
        }

        public static bool EnableWiFiModeOnly
        {
            get
            {
                return AppSettings.GetValueOrDefault(EnableWiFiModeOnlySettingsKey, false);
            }
            set
            {
                AppSettings.AddOrUpdateValue(EnableWiFiModeOnlySettingsKey, value);
            }
        }

        public static string WiFiNetworkName
        {
            get
            {
                return AppSettings.GetValueOrDefault(WiFiNetwrokNameSettingsKey, "CITS");
            }
            set
            {
                AppSettings.AddOrUpdateValue(WiFiNetwrokNameSettingsKey, value);
            }
        }

        public static string WiFiNetworkPassword
        {
            get
            {
                return AppSettings.GetValueOrDefault(WiFiNetworkPasswordSettingsKey, "");
            }
            set
            {
                AppSettings.AddOrUpdateValue(WiFiNetworkPasswordSettingsKey, value);
            }
        }
    }
}