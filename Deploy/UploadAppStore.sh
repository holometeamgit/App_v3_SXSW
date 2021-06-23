#!/bin/bash
 
echo "Uploading IPA to Appstore Connect..."

set -x

export FIREBASE_BUILD="$(find -E . -regex '.*\.(ipa|apk)' -print -quit)"

if [ -z "$FIREBASE_BUILD" ]; then
    echo "Could not find .ipa/.apk file"
    exit 1
else
    if xcrun altool --upload-app -f $FIREBASE_BUILD -u $ITUNES_USERNAME -p $ITUNES_PASSWORD ; then
        echo "Upload IPA to Appstore Connect finished with success"
    else
        echo "Upload IPA to Appstore Connect failed"
    fi
fi