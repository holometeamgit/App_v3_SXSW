
#!/bin/bash

echo "Uploading IPA to Appstore Connect..."

path = "WORKSPACE/.build/last/$TARGET_NAME/build.ipa"

(npm bin)/firebase appdistribution:distribute $path --app 1:233061171188:ios:1c742b53c13c922fcf43a1 --release-notes "Firebase Distribution Test" --groups "Test"