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

using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Graphics;
using Android.Support.V7.App;
using Android.Util;

using GreenLight.Droid.Views;

namespace GreenLight.Droid.Services
{
    [Service]
    public class GeofenceTransitionsIntentService : IntentService
    {
        protected const string TAG = "geofence-transitions-service";

        public GeofenceTransitionsIntentService() : base(TAG)
        {
        }

        protected override void OnHandleIntent(Intent intent)
        {
            var geofencingEvent = GeofencingEvent.FromIntent(intent);
            if (geofencingEvent.HasError)
            {
                Log.Error(TAG, geofencingEvent.ErrorCode.ToString());
                return;
            }

            int geofenceTransition = geofencingEvent.GeofenceTransition;

            if (geofenceTransition == Geofence.GeofenceTransitionEnter ||
                geofenceTransition == Geofence.GeofenceTransitionExit)
            {

                IList<IGeofence> triggeringGeofences = geofencingEvent.TriggeringGeofences;

                string geofenceTransitionDetails = GetGeofenceTransitionDetails(this, geofenceTransition, triggeringGeofences);

                //SendNotification(geofenceTransitionDetails);

                Intent geofenceIntent = new Intent();
                geofenceIntent.SetAction("uk.co.eastpoint.GeofenceBroadcast");
                geofenceIntent.PutExtra("GetGeofenceTransitionDetails", geofenceTransitionDetails);
                if (geofenceTransition == Geofence.GeofenceTransitionExit)
                {
                    string exitedIntersection = GetGeofenceIntersectionExited(this, triggeringGeofences);
                    geofenceIntent.PutExtra("GetGeofenceIntersectionExited", exitedIntersection);
                }

                SendBroadcast(geofenceIntent);

                Log.Info(TAG, geofenceTransitionDetails);
            }
            else
            {
                // Log the error.
                Log.Error(TAG, "transition invalid", new[] { new Java.Lang.Integer(geofenceTransition) });
            }
        }

        string GetGeofenceTransitionDetails(Context context, int geofenceTransition, IList<IGeofence> triggeringGeofences)
        {
            string geofenceTransitionString = GetTransitionString(geofenceTransition);

            var triggeringGeofencesIdsList = new List<string>();
            foreach (IGeofence geofence in triggeringGeofences)
            {
                triggeringGeofencesIdsList.Add(geofence.RequestId);
            }
            var triggeringGeofencesIdsString = string.Join(", ", triggeringGeofencesIdsList);

            return geofenceTransitionString + ": " + triggeringGeofencesIdsString;
        }

        string GetGeofenceIntersectionExited(Context context, IList<IGeofence> triggeringGeofences)
        {
            var triggeringGeofencesIdsList = new List<string>();
            foreach (IGeofence geofence in triggeringGeofences)
            {
                triggeringGeofencesIdsList.Add(geofence.RequestId);
            }
            var triggeringGeofencesIdsString = string.Join(", ", triggeringGeofencesIdsList);

            return triggeringGeofencesIdsString;
        }

        void SendNotification(string notificationDetails)
        {
            var notificationIntent = new Intent(ApplicationContext, typeof(FirstView));

            var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(this);
            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(FirstView)));
            stackBuilder.AddNextIntent(notificationIntent);

            var notificationPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

            var builder = new NotificationCompat.Builder(this);
            builder.SetSmallIcon(Resource.Drawable.greenCircle)
                .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.greenCircle))
                .SetColor(Color.Red)
                .SetContentTitle(notificationDetails)
                .SetContentText("transition notification")
                .SetContentIntent(notificationPendingIntent);

            builder.SetAutoCancel(true);

            var mNotificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            mNotificationManager.Notify(0, builder.Build());
        }

        string GetTransitionString(int transitionType)
        {
            switch (transitionType)
            {
                case Geofence.GeofenceTransitionEnter:
                    return "entered";
                case Geofence.GeofenceTransitionExit:
                    return "exited";
                default:
                    return "transition";
            }
        }
    }
}