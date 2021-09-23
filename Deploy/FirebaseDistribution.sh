
#!/bin/bash

echo "Uploading Build Result to Firebase Distribution..."

set -x

build_json = $(jq . < build.json)

build_target=$(jq -r 'keys[0]' < build.json)

build_platform=$(jq -r ".[\"${build_target}\"].platform" < build.json)

echo "Building $build_target for $build_platform"

echo "Build Json $build_json" 

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