#!/bin/bash

echo "Uploading Build Result to Firebase Distribution..."

set -x

export FIREBASE_BUILD="$(find -E . -regex '.*\.(ipa|apk|aab)' -print -quit)"

if [ -z "$FIREBASE_BUILD" ]; then
    echo "Could not find .ipa/.apk/.aab file"
    exit 1
else
    echo "Install Firebase Tools.."
    curl -sL https://firebase.tools | bash
    echo "Firebase Distribution..."
    firebase appdistribution:distribute $FIREBASE_BUILD --app $FIREBASE_APP --release-notes $FIREBASE_RELEASE_NOTES --groups $FIREBASE_GROUPS --token $FIREBASE_TOKEN;
fi