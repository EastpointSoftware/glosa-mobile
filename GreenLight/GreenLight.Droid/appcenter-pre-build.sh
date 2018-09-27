#!/usr/bin/env bash

echo "Started pre-build script"

if [ ! -n "$AZURE_MOBILE_SERVICE_CLIENT_URI" ]
then
    echo "You need define the API_URL variable in App Center"
    exit
fi

echo "Running pre-build script - Get Source directory"

echo "$APPCENTER_SOURCE_DIRECTORY"

APP_CONSTANT_FILE=$APPCENTER_SOURCE_DIRECTORY/GreenLight/GreenLight.Core/Constants.cs

echo "Running pre-build script - Found Source directory! $APPCENTER_SOURCE_DIRECTORY"

if [ -e "$APP_CONSTANT_FILE" ]
then
    echo "Updating values $AZURE_MOBILE_SERVICE_CLIENT_URI in Constants.cs"
	
	sed -i '' "s/AZURE_MOBILE_SERVICE_CLIENT_URI_SECRET/$AZURE_MOBILE_SERVICE_CLIENT_URI/g" $APP_CONSTANT_FILE
	sed -i '' "s/API_GLOSA_MAP_ENDPPOINT_URL_SECRET/$API_GLOSA_MAP_ENDPPOINT_URL/g" $APP_CONSTANT_FILE
	sed -i '' "s/API_GLOSA_SPAT_ENDPPOINT_URL_SECRET/$API_GLOSA_SPAT_ENDPPOINT_URL/g" $APP_CONSTANT_FILE
	sed -i '' "s/API_GLOSA_CAM_ENDPPOINT_URL_SECRET/$API_GLOSA_CAM_ENDPPOINT_URL/g" $APP_CONSTANT_FILE
	sed -i '' "s/AZURE_APP_CENTER_IOS_KEY_SECRET/$AZURE_APP_CENTER_IOS_KEY/g" $APP_CONSTANT_FILE
	sed -i '' "s/AZURE_APP_CENTER_ANDROID_KEY_SECRET/$AZURE_APP_CENTER_ANDROID_KEY/g" $APP_CONSTANT_FILE

    echo "File content:"
    cat $APP_CONSTANT_FILE
fi