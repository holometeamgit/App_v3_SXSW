#!/bin/bash

echo "Uploading Build Result to Firebase Distribution..."

set -x

export FIREBASE_BUILD="$(find -E . -regex '.*\.(ipa|apk|aab)' -print -quit)"

export BUILD_MANIFEST = "$(find -E . -regex '.*\UnityCloudBuildManifest.json)' -print -quit)"

echo $BUILD_MANIFEST

export BUILD_VALUE = "$(jq .<build.json)

echo $BUILD_VALUE

if [ -z "$FIREBASE_BUILD" ]; then
    echo "Could not find .ipa/.apk/.aab file"
    exit 1
else
    echo "Install Firebase Tools.."
    npm install -g firebase-tools
    echo "Firebase Distribution..."
    firebase appdistribution:distribute $FIREBASE_BUILD --app $FIREBASE_APP --release-notes-file $FIREBASE_RELEASE_NOTES --groups $FIREBASE_GROUPS --token $FIREBASE_TOKEN;
fi