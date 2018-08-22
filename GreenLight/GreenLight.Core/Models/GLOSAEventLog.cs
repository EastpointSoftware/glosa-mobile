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

namespace GreenLight.Core.Models
{
    public class GLOSAEventLog
    {
        [Newtonsoft.Json.JsonProperty("id")]
        public string Id { get; set; }

        [Microsoft.WindowsAzure.MobileServices.Version]
        public string AzureVersion { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string VehicleId { get; set; }
        public DateTime DeviceTime { get; set; }
        public double TimeOffset { get; set; }
        public string IntersectionId { get; set; }
        public string Event { get; set; }
        public string GPSHistory { get; set; }
        public int Distance { get; set; }
        public string Value { get; set; }
        public bool AdvisoryEnabled { get; set; }
        public string RouteId { get; set; }
        public string RouteSession { get; set; }
        public double Speed { get; set; }
        public string CalculationAdvisory { get; set; }
        public string SPAT { get; set; }
        public double Latency { get; set; }
        public int Heading { get; set; }
        public string MAP { get; set; }
        public string Lane { get; set; }
    }
}
