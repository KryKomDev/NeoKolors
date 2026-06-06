#!/usr/bin/env bash

# Aggregates build and test status artifacts and updates the GitHub Gist badge schema files.

set -euo pipefail

# Required environment variables
GIST_ID="${GIST_ID:-}"
GIST_TOKEN="${GIST_TOKEN:-}"
STATUS_DIR="${STATUS_DIR:-status-artifacts}"

if [ -z "$GIST_ID" ]; then
  echo "Error: GIST_ID environment variable is not set." >&2
  exit 1
fi

if [ -z "$GIST_TOKEN" ]; then
  echo "Error: GIST_TOKEN environment variable is not set." >&2
  exit 1
fi

if [ ! -d "$STATUS_DIR" ]; then
  echo "Warning: Status directory '$STATUS_DIR' does not exist. No badges to update."
  exit 0
fi

# Initialize an empty JSON object for files
files_json=$(jq -n '{}')

projects=("Common" "Console" "Extensions" "Settings" "Tui" "Tui.Core" "Tui.Fonts" "Tui.Fonts.Assets" "Tools.NKFont")

for proj in "${projects[@]}"; do
  # Check if build artifact exists
  build_file="$STATUS_DIR/status-build-${proj}/build_status.txt"
  if [ -f "$build_file" ]; then
    status_val=$(cat "$build_file")
    if [ "$status_val" = "passing" ]; then
      color="brightgreen"
    else
      color="red"
    fi
    content="{\"schemaVersion\": 1, \"label\": \"build\", \"message\": \"$status_val\", \"color\": \"$color\"}"
    files_json=$(echo "$files_json" | jq --arg proj "$proj" --arg content "$content" '. + {("build-" + $proj + ".json"): {content: $content}}')
  fi

  # Check if test artifact exists
  test_file="$STATUS_DIR/status-test-${proj}/test_status.txt"
  if [ -f "$test_file" ]; then
    status_val=$(cat "$test_file")
    if [ "$status_val" = "passing" ]; then
      color="brightgreen"
    else
      color="red"
    fi
    content="{\"schemaVersion\": 1, \"label\": \"tests\", \"message\": \"$status_val\", \"color\": \"$color\"}"
    files_json=$(echo "$files_json" | jq --arg proj "$proj" --arg content "$content" '. + {("test-" + $proj + ".json"): {content: $content}}')
  fi
done

# If files_json is not empty, construct full payload and send PATCH request
if [ "$files_json" != "{}" ]; then
  echo "Updating Gist badges on GitHub Gist $GIST_ID..."
  payload=$(jq -n --argjson files "$files_json" '{files: $files}')
  
  curl -L \
    -s \
    -X PATCH \
    -H "Accept: application/vnd.github+json" \
    -H "Authorization: Bearer $GIST_TOKEN" \
    -H "X-GitHub-Api-Version: 2022-11-28" \
    "https://api.github.com/gists/$GIST_ID" \
    -d "$payload" > /dev/null
    
  echo "Gist badges updated successfully."
else
  echo "No active project runs to update."
fi
