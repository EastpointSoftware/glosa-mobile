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

namespace GreenLight.Core
{
    public class Constants
    {
        // SPAT/MAP
        // Celluar : SPAT Message sent every 5s and Valid for 60s. MAP Message sent every 300s and valid for 600s
        // 802.11g/n : SPAT Message sent every 1s and Valid for 60s. MAP Message sent every 300s and valid for 600s
        // For real-world usage we set SPAT to 10s
        public static int MAP_MESSAGE_VALIDATION_PERIOD_SECONDS = 600;
        public static int SPAT_MESSAGE_VALIDATION_PERIOD_SECONDS = 10;

        public static string MovementEventStateGreen = "protected-Movement-Allowed";
        public static string MovementEventStateAmber = "permissive-clearance";
        public static string MovementEventStateRed = "stop-And-Remain";
        public static string MovementEventStateRedAmber = "pre-Movement";

        public static ulong MANEUVER_DIRECTION_AHEAD = 100000100000;
        public static ulong MANEUVER_DIRECTION_LEFT = 010000000000;
        public static ulong MANEUVER_DIRECTION_RIGHT = 001000000000;

        public static int SPAT_MOVEMENT_EVENT_TIMING_UNKNOWN = 36002;
        public static int SPAT_MOVEMENT_EVENT_TIMING_UNIT = 10;

        public static double MAPCoordinateIntConverterUnit = 10000000.0000000;

        //GPS
        public static int GPS_HISTORY_MINIMUM_WINDOW_SIZE = 3;

        //Advisory Speed
        public static int AdvisorySpeedCalculatorProcessingIntervalMilliseconds = 1000;

        // Distance
        public static int GeofenceDefaultRadiusMeters = 40;
        public static double JUNCTION_COMPLETED_SEARCH_RADIUS = 35; // in meters
        public static double MeterToKiloMeterConstant = 0.001;

        // TEST Values
        public static string TEST_INTERSECTION_ID = "2111";

        //Control Text Values
        public static string ALERT_BUTTON_CLOSE_TEXT = "Close";

        // Advisory minimum distance check. This prevents the calculator from advising of speed when too close to junction
        public static int CALCULATOR_MINIMUM_DISTANCE_CHECK_METERS = 300;
        public static int CALCULATOR_MAXIMUM_DISTANCE_CHECK_METERS = 500;

        //Settings
        public const int SETTINGS_TIMINGS_OPTION_MIN_END = 1;
        public const int SETTINGS_TIMINGS_OPTION_START_END = 2;

        public const int SETTINGS_ROUTE_DIRECTION_ANY = 0;
        public const int SETTINGS_ROUTE_DIRECTION_NS = 1;
        public const int SETTINGS_ROUTE_DIRECTION_SN = 2;
        public const int SETTINGS_ROUTE_DIRECTION_EW = 3;
        public const int SETTINGS_ROUTE_DIRECTION_WE = 4;

        // Data Analytics
        public static string AZURE_MOBILE_SERVICE_CLIENT_URI = "AZURE_MOBILE_SERVICE_CLIENT_URI_SECRET";
        // GLOSA Web Service
        public static string API_GLOSA_MAP_ENDPPOINT_URL = "API_GLOSA_MAP_ENDPPOINT_URL_URI_SECRET";
        public static string API_GLOSA_SPAT_ENDPPOINT_URL = "API_GLOSA_SPAT_ENDPPOINT_URL_URI_SECRET";
        public static string API_GLOSA_CAM_ENDPPOINT_URL = "API_GLOSA_CAM_ENDPPOINT_UR_URI_SECRETL";

        public static string AZURE_APP_CENTER_IOS_KEY = "AZURE_APP_CENTER_IOS_KEY_URI_SECRET";
        public static string AZURE_APP_CENTER_ANDROID_KEY = "AZURE_APP_CENTER_ANDROID_KEY_URI_SECRET";
    }
}
