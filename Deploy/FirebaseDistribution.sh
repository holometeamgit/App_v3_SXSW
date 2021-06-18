
#!/bin/bash

echo "Uploading Build Result to Firebase Distribution..."

#Path is "/BUILD_PATH/<ORG_ID>.<PROJECT_ID>.<BUILD_TARGET_ID>/.build/last/<BUILD_TARGET_ID>/build.ipa"
#path="$WORKSPACE/.build/last/$BUILD_TARGET_ID/build.ipa"

set -x

export FIREBASE_BUILD="$(find -E . -regex '.*\.(ipa|apk)' -print -quit)"
if [ -z "$FIREBASE_BUILD" ]; then
  echo "Could not find .ipa/.apk file"
  exit 1
else

#echo "Installing npm..."

#npm install -g firebase-tools

echo "Firebase Distribution..."

firebase appdistribution:distribute $FIREBASE_BUILD --app $FIREBASE_APP --release-notes $FIREBASE_RELEASE_NOTES --groups $FIREBASE_GROUPS --token $FIREBASE_TOKEN;
fi