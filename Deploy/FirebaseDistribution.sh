#!/bin/bash

set -x

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

function parse_git_message() {
  git log -1 HEAD --pretty=format:%s
}

function parse_git_commit_id() {
  git rev-parse --verify HEAD
}

function build_target() {
  jq -r 'keys[0]' < build.json
}

echo "Uploading build.json..."

BUILD_TARGET=$(build_target)

echo "Uploading Git Data..."

GIT_BRANCH=$(parse_git_branch)$(parse_git_hash)

MESSAGE=$(parse_git_message) 

GIT_MESSAGE=$($MESSAGE | tr " " "_")

COMMIT_ID=$(git rev-parse --verify HEAD)

echo "Creating Release Notes..."

jq -n --arg config "$BUILD_TARGET" --arg version "$BEEM_VERSION" --arg type "$BEEM_BUILD_TYPE" --arg branch "$GIT_BRANCH" --arg message $GIT_MESSAGE --arg commitID "$COMMIT_ID" '$ARGS.named' > release_notes.json

echo "Uploading Build Result to Firebase Distribution..."

export FIREBASE_BUILD="$(find -E . -regex '.*\.(ipa|apk|aab)' -print -quit)"

if [ -z "$FIREBASE_BUILD" ]; then
    echo "Could not find .ipa/.apk/.aab file"
    exit 1
else
    echo "Install Firebase Tools.."
    npm install -g firebase-tools
    echo "Firebase Distribution..."
    firebase appdistribution:distribute $FIREBASE_BUILD --app $FIREBASE_APP --release-notes-file "release_notes.json" --groups $FIREBASE_GROUPS --token $FIREBASE_TOKEN;
fi