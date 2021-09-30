#!/bin/bash

echo "Uploading Build Result to Firebase Distribution..."

set -x

export YOUR_API_KEY="6c9ddff22a920c8e97a8b2449a9b366b"
export ORG_ID="10170749378847"
export PROJECT_ID="ba3333ea-4845-4d5b-a4c1-896277975428"
export BUILD_TARGET_ID="fbapkdev_buildmanifest"

http_response=$(curl -X GET -H "Content-Type: application/json" -H "Authorization: Basic $YOUR_API_KEY" https://build-api.cloud.unity3d.com/api/v1/orgs/$ORG_ID/projects/$PROJECT_ID/buildtargets/$BUILD_TARGET_ID/builds)

echo "${http_response}"

export FIREBASE_BUILD="$(find -E . -regex '.*\.(ipa|apk|aab)' -print -quit)"

export BUILD_VALUE="$(jq .<build.json)"

build_target=$(jq -r 'keys[0]' < build.json)

build_projectid=$(jq -r ".[\"${build_target}\"].projectid" < build.json)

echo "Building $build_target for $build_projectid"

project_data=$(echo $build_projectid | tr "/" "\n")

echo "${project_data[0]}"

echo "${project_data[1]}"

arrIn=(${build_projectid//// })

echo ${arrIN[0]}

echo ${arrIN[1]}

IFS='/' read -ra ADDR <<< "$build_projectid"
for i in "${ADDR[@]}"; do
  echo ${ADDR[i]}
done

echo $BUILD_VALUE

if [ -z "$FIREBASE_BUILD" ]; then
    echo "Could not find .ipa/.apk/.aab file"
    exit 1
else
    echo "Install Firebase Tools.."
    curl -s https://firebase.tools | bash
    echo "Firebase Distribution..."
    firebase appdistribution:distribute $FIREBASE_BUILD --app $FIREBASE_APP --release-notes $FIREBASE_RELEASE_NOTES --groups $FIREBASE_GROUPS --token $FIREBASE_TOKEN;
fi