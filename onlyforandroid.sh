#!/bin/bash    

#git checkout vzw
#git merge --strategy-option theirs
#git add .
#git commit -m " * merged "
git pull
git add .
git commit -m " * pulled and merged"

export VERSION_MAJOR=6
export VERSION_MINOR=0
export VERSION_REVISION=4000
export VERSION_TAG=

file="bundle"

export BUNDLE_ID=$(cat "$file")
BUNDLE_ID=$(($BUNDLE_ID + 1))

echo $BUNDLE_ID | tee $file

export VERSION_BUILD=$BUNDLE_ID

export KEYSTORE_NAME=Assets/keystore/RWKey.keystore
export KEYSTORE_PASS=testtest
export KEYALIAS_NAME=rwkey
export KEYALIAS_PASS=testtest

export NAME=Kid_Mode_$(date +%m_%d_%Y)_prod_v$VERSION_MAJOR.$VERSION_MINOR.$BUNDLE_ID
#echo $NAME
/Applications/Unity/Unity.app/Contents/MacOS/Unity -executeMethod GCSBuild.BuildAndroid -quit


git add .
git commit -m " * uploaded "$NAME.apk
git push

