
#!/bin/bash

echo "Uploading IPA to Appstore Connect..."

#Path is "/BUILD_PATH/<ORG_ID>.<PROJECT_ID>.<BUILD_TARGET_ID>/.build/last/<BUILD_TARGET_ID>/build.ipa"
path="$WORKSPACE/.build/last/$TARGET_NAME/build.ipa"
release="Firebase Distribution Test"
token="1//0cVIYMiei3SyGCgYIARAAGAwSNwF-L9IrRwas9jSXkAepwnuw51TfbFehS0Iq7w9SZ-Q4CkUsWY9fF9TflH1Nx1YB6EHGzcaqEbg"
group="Test"
firebaseApp="1:233061171188:ios:1c742b53c13c922fcf43a1";


npm install -g firebase-tools

firebase appdistribution:distribute $path --app $firebaseApp --release-notes $release --groups $group --token $token;