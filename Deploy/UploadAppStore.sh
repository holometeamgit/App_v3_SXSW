#!/bin/bash
 
echo "Uploading IPA to Appstore Connect..."

set -x

export APPSTORE_BUILD="$(find -E . -regex '.*\.(ipa)' -print -quit)"

if [ -z "$APPSTORE_BUILD" ]; then
    echo "Could not find .ipa file"
    exit 1
else
    if xcrun altool --upload-app -f $APPSTORE_BUILD --type ios -u $ITUNES_USERNAME -p $ITUNES_PASSWORD ; then
        echo "Upload IPA to Appstore Connect finished with success"
    else
        echo "Upload IPA to Appstore Connect failed"
    fi
fi