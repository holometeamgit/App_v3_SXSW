#!/bin/bash

echo "Uploading Build Result to Firebase Distribution..."

set -x

export build_target=$(jq -r 'keys[0]' < build.json)

export build_platform=$(jq -r ".[\"${build_target}\"].artifacts" < build.json)

echo "Building $build_target for $build_platform"

export FIREBASE_BUILD="$(find -E . -regex '.*\.(ipa|apk|aab)' -print -quit)"
if [ -z "$FIREBASE_BUILD" ]; then
    echo "Could not find .ipa/.apk/.aab file"
    exit 1
else
    echo "Install Firebase Tools.."
    npm install -g firebase-tools
    echo "Firebase Distribution..."
    firebase appdistribution:distribute $FIREBASE_BUILD --app $FIREBASE_APP --release-notes $FIREBASE_RELEASE_NOTES --groups $FIREBASE_GROUPS --token $FIREBASE_TOKEN;
fi