
#!/bin/bash

echo "Uploading IPA to Appstore Connect..."

path = "WORKSPACE/.build/last/$TARGET_NAME/build.ipa"

if xcrun altool --upload-app -f $path -u $ITUNES_USERNAME -p $ITUNES_PASSWORD; then 
	echo "Upload IPA to Appstore Connect finished with success"
else
	echo "Upload IPA to Appstore Connect failed"
fi