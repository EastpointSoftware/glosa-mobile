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

#define OFFLINE_SYNC_ENABLED
using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

using Microsoft.WindowsAzure.MobileServices;

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
#endif

using GreenLight.Core.Models;
using GreenLight.Core.Contracts;

namespace GreenLight.Core.Services
{
    public class DataAnalyticsService : IDataAnalyticsService
    {
        #region Construction
        public DataAnalyticsService()
        {
            try
            {
                MobileService = new MobileServiceClient(Constants.AZURE_MOBILE_SERVICE_CLIENT_URI);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

#if OFFLINE_SYNC_ENABLED
            try
            {
                filename = Path.Combine(MobileServiceClient.DefaultDatabasePath, filename);
                var store = new MobileServiceSQLiteStore(filename);
                store.DefineTable<GLOSAEventLog>();
                store.DefineTable<GLOSAMonitoringLog>();

                // Uses the default conflict handler, which fails on conflict
                // To use a different conflict handler, pass a parameter to InitializeAsync.
                // For more details, see http://go.microsoft.com/fwlink/?LinkId=521416
                MobileService.SyncContext.InitializeAsync(store);
                _eventLogTable = MobileService.GetSyncTable<GLOSAEventLog>();
                _monitoringLogTable = MobileService.GetSyncTable<GLOSAMonitoringLog>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

#else
            _eventLogTable = MobileService.GetTable<GLOSAEventLog>();
            _monitoringLogTable = MobileService.GetTable<GLOSAMonitoringLog>();
#endif
        }
        

        #endregion

        #region Properties

        public MobileServiceClient MobileService { get; set; }

        public bool IsOfflineEnabled
        {
            get { return _eventLogTable is IMobileServiceSyncTable<GLOSAEventLog>; }
        }
        #endregion

        #region Implementation

#if OFFLINE_SYNC_ENABLED
        private async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await MobileService.SyncContext.PushAsync();
                
            }
            catch (MobileServicePushFailedException exc)
            {
                if (exc.PushResult != null)
                {
                    syncErrors = exc.PushResult.Errors;
                }
            }

            if (syncErrors != null)
            {
                foreach (var error in syncErrors)
                {
                    if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
                    {
                        // Update failed, revert to server's copy
                        await error.CancelAndUpdateItemAsync(error.Result);
                    }
                    else
                    {
                        // Discard local change
                        await error.CancelAndDiscardItemAsync();
                    }

                    Debug.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.", error.TableName, error.Item["id"]);
                }
            }
        }
#endif

        public async Task SaveEventLogAsync(GLOSAEventLog item)
        {
            try
            {
                // Insert the new item into the local store.
                await _eventLogTable.InsertAsync(item);
#if OFFLINE_SYNC_ENABLED
                // Send changes to the mobile app backend.
                await SyncAsync();
#endif
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message.ToString());
            }
        }
        public async Task SaveMonitoringLogAsync(GLOSAMonitoringLog item)
        {
            try
            {
                // Insert the new item into the local store.
                await _monitoringLogTable.InsertAsync(item);
#if OFFLINE_SYNC_ENABLED
                // Send changes to the mobile app backend.
                await SyncAsync();
#endif
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message.ToString());
            }
        }
        #endregion

        #region Member Variables

#if OFFLINE_SYNC_ENABLED
        private IMobileServiceSyncTable<GLOSAEventLog> _eventLogTable;
        private IMobileServiceSyncTable<GLOSAMonitoringLog> _monitoringLogTable;
        private string filename = "syncstore.db";
#else
        private IMobileServiceTable<GLOSAEventLog> _eventLogTable;
        private IMobileServiceTable<GLOSAMonitoringLog> _monitoringLogTable;
#endif

        #endregion

    }
}
