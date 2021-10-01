#!/bin/bash

echo "Uploading Build Result to Firebase Distribution..."

set -x

build_target=$(jq -r 'keys[0]' < build.json)

build_projectid=$(jq -r ".[\"${build_target}\"].projectid" < build.json)

IFS='/' read -ra ADDR <<< "$build_projectid"

export YOUR_API_KEY="6c9ddff22a920c8e97a8b2449a9b366b"
export ORG_ID=${ADDR[0]}
export PROJECT_ID=${ADDR[1]}
export BUILD_TARGET_ID="${build_target}"

echo "Start Response"

http_response=$(curl -sL -H "Content-Type: application/json" -H "Authorization: Basic $YOUR_API_KEY" https://build-api.cloud.unity3d.com/api/v1/orgs/$ORG_ID/projects/$PROJECT_ID/buildtargets/$BUILD_TARGET_ID/builds)

echo "${http_response}"

echo "End Response"

COMMIT_ID=$(git rev-parse --verify HEAD)

echo $COMMIT_ID

#!/usr/bin/env bash

# checks if branch has something pending
function parse_git_dirty() {
  git diff --quiet --ignore-submodules HEAD 2>/dev/null; [ $? -eq 1 ] && echo "*"
}

# gets the current git branch
function parse_git_branch() {
  git branch --no-color 2> /dev/null | sed -e '/^[^*]/d' -e "s/* \(.*\)/\1$(parse_git_dirty)/"
}

# get last commit hash prepended with @ (i.e. @8a323d0)
function parse_git_hash() {
  git rev-parse --short HEAD 2> /dev/null | sed "s/\(.*\)/@\1/"
}

# DEMO
GIT_BRANCH=$(parse_git_branch)$(parse_git_hash)
echo ${GIT_BRANCH}

COMMENT=$(git log -1 $(parse_git_branch))

echo $COMMENT;

export RELEASE_NOTES="Version: $BEEM_VERSION, \n Build Type: $BEEM_BUILD_TYPE, \n Branch: $GIT_BRANCH, \n Commit ID: $COMMIT_ID"

export FIREBASE_BUILD="$(find -E . -regex '.*\.(ipa|apk|aab)' -print -quit)"

if [ -z "$FIREBASE_BUILD" ]; then
    echo "Could not find .ipa/.apk/.aab file"
    exit 1
else
    echo "Install Firebase Tools.."
    npm install -g firebase-tools
    echo "Firebase Distribution..."
    firebase appdistribution:distribute $FIREBASE_BUILD --app $FIREBASE_APP --release-notes $RELEASE_NOTES --groups $FIREBASE_GROUPS --token $FIREBASE_TOKEN;
fi