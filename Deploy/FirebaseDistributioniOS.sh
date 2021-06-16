
#!/bin/bash

echo "Uploading IPA to Firebase Distribution..."

#Path is "/BUILD_PATH/<ORG_ID>.<PROJECT_ID>.<BUILD_TARGET_ID>/.build/last/<BUILD_TARGET_ID>/build.ipa"
path="$WORKSPACE/.build/last/$BUILD_TARGET_ID/build.ipa"

echo "Installing npm..."

npm install -g firebase-tools

echo "Firebase Distribution..."

firebase appdistribution:distribute $path --app $FIREBASE_APP --release-notes $FIREBASE_RELEASE_NOTES --groups $FIREBASE_GROUPS --token $FIREBASE_TOKEN;