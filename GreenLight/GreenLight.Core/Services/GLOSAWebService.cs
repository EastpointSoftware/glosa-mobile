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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml.Serialization;
using System.Xml;

using GreenLight.Core.Contracts;
using GreenLight.Core.Handlers;
using GreenLight.Core.Helpers;
using GreenLight.Core.Models;

namespace GreenLight.Core.Services
{
    public class GLOSAWebService : IGLOSAWebService
    {
        #region Construction
        public GLOSAWebService(IDataAnalyticsService dataAnalyticsService)
        {
            _dataAnalyticsService = dataAnalyticsService;

            MAPDataLastSyncTime = new DateTime(1970, 1, 1);
            SPATRequestSyncTimes = new Dictionary<string, DateTime>();
            MAPDataStore = new Dictionary<string, MapData>();
            SPATDataStore = new Dictionary<string, SPAT>();

            MAPRequestErrors = new Dictionary<string, int>();
            SPATRequestErrors = new Dictionary<string, int>();

            _httpClient = new HttpClient(new TraceHandler());
        }
        #endregion

        #region Properties

        private DateTime MAPDataLastSyncTime { get; set; }

        private Dictionary<string, MapData> MAPDataStore { get; set; }
        private Dictionary<string, SPAT> SPATDataStore { get; set; }
        private Dictionary<string, int> MAPRequestErrors { get; set; }
        private Dictionary<string, int> SPATRequestErrors { get; set; }
        private Dictionary<string, DateTime> SPATRequestSyncTimes { get; set; }

        public string AuthorizationToken
        {
            get
            {
                string token = Settings.AuthorizationToken != string.Empty ? Settings.AuthorizationToken : null;
                return token;
            }
            set { Settings.AuthorizationToken = value; }
        }
        #endregion

        #region Implementation 

        public async Task SyncMAPSPATAsync(string intersectionId)
        {
            await SyncMAPSPATAsync(intersectionId, null);
        }

        public async Task SyncMAPSPATAsync(string intersectionId1, string intersectionId2 = null)
        {
            Task mapDataTask = MAPRequest(intersectionId1);
            Task spatDataTask = SPATRequest(intersectionId1);

            await mapDataTask;
            await spatDataTask;

            if (intersectionId2 != null)
            {
                Task mapDataTask2 = MAPRequest(intersectionId2);
                Task spatDataTask2 = SPATRequest(intersectionId2);

                await mapDataTask2;
                await spatDataTask2;
            }
        }

        public bool HasMAPSPATDataSyncedForIntersection(string intersectionId)
        {
            return HasMAPDataForIntersection(intersectionId) == true && HasSignalPhaseAndTimingForIntersection(intersectionId) == true;
        }

        public MapData MAPData(string intersectionId)
        {
            return MAPDataStore[intersectionId];
        }

        public SPAT SPATData(string intersectionId)
        {
            return SPATDataStore[intersectionId];
        }

        private async Task<T> GLOSACommunicateAsync<T>(string id, string endpoint)
        {
            T data;

            var url = endpoint + id;
            HttpResponseMessage message = await GetClient().GetAsync(url);

            if (message.IsSuccessStatusCode == true)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Envelope));

                var messageContent = await message.Content.ReadAsStringAsync();

                Envelope envelope = serializer.Deserialize(await message.Content.ReadAsStreamAsync()) as Envelope;
                data = (T)Convert.ChangeType(envelope, typeof(T));
            }
            else
            {
                throw new Exception(message.ToString());
            }

            return data;
        }

        private async Task MAPRequest(string intersectionId)
        {
            if (_mapRequestActive == true)
                return;

            if (MAPRequestErrors.ContainsKey(intersectionId) && MAPRequestErrors[intersectionId] > 5)
                return;

            _mapRequestActive = true;
            var before = DateTime.Now;
            var requestMethod = "GET";
            var URL = Constants.API_GLOSA_MAP_ENDPPOINT_URL + intersectionId;
            int statusCode = 0;
            string value = null;
            try
            {
                var map = MAPDataStore.ContainsKey(intersectionId) ? MAPDataStore[intersectionId] : null;
                if (map == null)
                {
                    Envelope envelope = await GLOSACommunicateAsync<Envelope>(intersectionId, Constants.API_GLOSA_MAP_ENDPPOINT_URL);
                    MapData data = envelope.Body.MapData;
                    statusCode = 200;
                    if (data != null)
                    {
                        if (MAPDataStore.ContainsKey(intersectionId))
                            MAPDataStore[intersectionId] = data;
                        else
                            MAPDataStore.Add(intersectionId, data);

                        MAPDataLastSyncTime = DateTime.Now;
                    }
                }
            }
            catch (Exception e)
            {
                if (MAPRequestErrors.ContainsKey(intersectionId))
                    MAPRequestErrors[intersectionId]++;
                else
                    MAPRequestErrors.Add(intersectionId, 1);

                var result = new GLOSAResult();
                result.Errors = GLOSAErrors.WebServiceError;
                if (e.GetType() == typeof(XmlException))
                {
                    result.Errors = GLOSAErrors.WebServiceXMLParsingError;
                }
                else
                {
                    statusCode = 500;
                    value = e.Message;
                }
            }
            finally
            {
                if (statusCode > 0)
                {
                    var after = DateTime.Now;
                    double latency = (after - before).TotalMilliseconds;

                    var log = new GLOSAMonitoringLog()
                    {
                        URL = URL,
                        StatusCode = statusCode,
                        Method = requestMethod,
                        Latency = latency,
                        Value = value,
                    };

                    if (_dataAnalyticsService != null)
                    {
                        Logger.LogMonitoring(_dataAnalyticsService, log);
                    }
                }

                _mapRequestActive = false;
            }
        }

        private async Task SPATRequest(string intersectionId)
        {
            // Should request every 10 seconds
            if (ShouldSyncSPATData(intersectionId) == false)
                return;

            if (_spatRequestActive == true)
                return;

            if (SPATRequestErrors.ContainsKey(intersectionId) && SPATRequestErrors[intersectionId] > 5)
                return;

            var before = DateTime.Now;

            _spatRequestActive = true;
            var requestMethod = "GET";
            var URL = Constants.API_GLOSA_SPAT_ENDPPOINT_URL + intersectionId;
            int statusCode = 0;
            string value = null;
            try
            {
                Envelope envelope = await GLOSACommunicateAsync<Envelope>(intersectionId, Constants.API_GLOSA_SPAT_ENDPPOINT_URL);
                SPAT data = envelope.Body.SPAT;
                if (data != null)
                {
                    if (SPATDataStore.ContainsKey(intersectionId))
                        SPATDataStore[intersectionId] = data;
                    else
                        SPATDataStore.Add(intersectionId, data);

                    UpdateSPATSyncTime(intersectionId);
                }
                statusCode = 200;
            }
            catch (Exception e)
            {
                if (SPATRequestErrors.ContainsKey(intersectionId))
                    SPATRequestErrors[intersectionId]++;
                else
                    SPATRequestErrors.Add(intersectionId, 1);

                var result = new GLOSAResult();
                result.Errors = GLOSAErrors.WebServiceError;
                if (e.GetType() == typeof(XmlException))
                {
                    result.Errors = GLOSAErrors.WebServiceXMLParsingError;
                }
                else
                {
                    statusCode = 500;
                    value = e.Message;
                }
            }
            finally
            {
                if (statusCode > 0)
                {
                    var after = DateTime.Now;
                    double latency = (after - before).TotalMilliseconds;

                    var log = new GLOSAMonitoringLog()
                    {
                        URL = URL,
                        StatusCode = statusCode,
                        Method = requestMethod,
                        Latency = latency,
                        Value = value,
                    };

                    if (_dataAnalyticsService != null)
                    {
                        Logger.LogMonitoring(_dataAnalyticsService, log);
                    }
                }
                _spatRequestActive = false;
            }
        }

        private HttpClient GetClient()
        {
            return _httpClient;
        }

        private bool HasMAPDataForIntersection(string intersectionId)
        {
            return MAPDataStore != null && MAPDataStore.ContainsKey(intersectionId) == true;
        }

        private bool HasSignalPhaseAndTimingForIntersection(string intersectionId)
        {
            return SPATDataStore != null && SPATDataStore.ContainsKey(intersectionId) == true;
        }

        private bool ShouldSyncSPATData(string intersectionId)
        {
            var valid = true;

            DateTime ofLastSync = DateTime.Now;

            if (SPATRequestSyncTimes.ContainsKey(intersectionId) == true)
            {
                ofLastSync = SPATRequestSyncTimes[intersectionId];
            }
            else
            {
                ofLastSync = ofLastSync.AddSeconds(-11);
            }

            var secondsExpired = Math.Abs((DateTime.Now - ofLastSync).TotalSeconds);

            valid = secondsExpired > Constants.SPAT_MESSAGE_VALIDATION_PERIOD_SECONDS;

            return valid;
        }

        private void UpdateSPATSyncTime(string intersectionId)
        {

            if (SPATRequestSyncTimes.ContainsKey(intersectionId) == true)
            {
                SPATRequestSyncTimes[intersectionId] = DateTime.Now;
            }
            else
            {
                SPATRequestSyncTimes.Add(intersectionId, DateTime.Now);
            }
        }

        public bool HasSPATDataSyncedForIntersection(string intersectionId)
        {
            return HasSignalPhaseAndTimingForIntersection(intersectionId);
        }

        public bool HasMAPDataSyncedForIntersection(string intersectionId)
        {
            return HasMAPDataForIntersection(intersectionId);
        }

        #endregion

        #region Member Variables
        private bool _mapRequestActive = false;
        private bool _spatRequestActive = false;
        private IDataAnalyticsService _dataAnalyticsService;
        private HttpClient _httpClient;

        #endregion
    }
}
