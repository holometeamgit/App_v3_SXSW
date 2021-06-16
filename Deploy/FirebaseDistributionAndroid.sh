
#!/bin/bash

echo "Uploading APK to Firebase Distribution..."

#Path is "/BUILD_PATH/<ORG_ID>.<PROJECT_ID>.<BUILD_TARGET_ID>/.build/last/<BUILD_TARGET_ID>/build.apk"
path="$WORKSPACE/.build/last/$BUILD_TARGET_ID.apk"

echo "Installing npm..."

npm install -g firebase-tools

echo "Firebase Distribution..."

firebase appdistribution:distribute $path --app $FIREBASE_APP --release-notes $FIREBASE_RELEASE_NOTES --groups $FIREBASE_GROUPS --token $FIREBASE_TOKEN;