#!/usr/bin/env bash

# Downloads and updates the Dokfx template to the latest released version from GitHub.

set -euo pipefail

# ANSI color codes
CYAN='\033[0;36m'
GREEN='\033[0;32m'
YELLOW='\033[0;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Repository details
REPO="KryKomDev/Dokfx"
API_URL="https://api.github.com/repos/$REPO/releases/latest"

# Get script folder and resolve target template directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TARGET_DIR="$(cd "$SCRIPT_DIR/../Docs" && pwd)/templates/dokfx"

echo -e "${CYAN}Fetching latest release information from GitHub API...${NC}"

# Check for curl or wget
if command -v curl >/dev/null 2>&1; then
    RELEASE_JSON=$(curl -s -L -H "User-Agent: bash" "$API_URL")
elif command -v wget >/dev/null 2>&1; then
    RELEASE_JSON=$(wget -qO- --header="User-Agent: bash" "$API_URL")
else
    echo -e "${RED}Error: Neither curl nor wget is installed.${NC}" >&2
    exit 1
fi

# Parse tag_name and asset download URL with multiple fallbacks (jq -> python3/python -> grep/sed)
TAG_NAME=""
DOWNLOAD_URL=""

if command -v jq >/dev/null 2>&1; then
    TAG_NAME=$(echo "$RELEASE_JSON" | jq -r '.tag_name // empty')
    DOWNLOAD_URL=$(echo "$RELEASE_JSON" | jq -r '.assets[]? | select(.name == "dokfx-template.zip") | .browser_download_url // empty')
    if [ -z "$DOWNLOAD_URL" ]; then
        DOWNLOAD_URL=$(echo "$RELEASE_JSON" | jq -r '.zipball_url // empty')
    fi
elif command -v python3 >/dev/null 2>&1 || command -v python >/dev/null 2>&1; then
    PYTHON_CMD="python3"
    if ! command -v python3 >/dev/null 2>&1; then
        PYTHON_CMD="python"
    fi
    TAG_NAME=$(echo "$RELEASE_JSON" | $PYTHON_CMD -c "import sys, json; print(json.load(sys.stdin).get('tag_name', ''))" 2>/dev/null)
    DOWNLOAD_URL=$(echo "$RELEASE_JSON" | $PYTHON_CMD -c "
import sys, json
data = json.load(sys.stdin)
assets = data.get('assets', [])
url = next((a['browser_download_url'] for a in assets if a['name'] == 'dokfx-template.zip'), '')
print(url if url else data.get('zipball_url', ''))
" 2>/dev/null)
else
    # Grep and sed fallbacks for minimal POSIX systems
    TAG_NAME=$(echo "$RELEASE_JSON" | grep -o '"tag_name": *"[^"]*"' | head -n 1 | cut -d'"' -f4 || true)
    DOWNLOAD_URL=$(echo "$RELEASE_JSON" | grep -o '"browser_download_url": *"[^"]*dokfx-template.zip"' | head -n 1 | cut -d'"' -f4 || true)
    if [ -z "$DOWNLOAD_URL" ]; then
        DOWNLOAD_URL=$(echo "$RELEASE_JSON" | grep -o '"zipball_url": *"[^"]*"' | head -n 1 | cut -d'"' -f4 || true)
    fi
fi

if [ -z "$TAG_NAME" ]; then
    echo -e "${RED}Error: Failed to parse tag name from GitHub API response.${NC}" >&2
    exit 1
fi

echo -e "${GREEN}Latest release tag: $TAG_NAME${NC}"

if [ -z "$DOWNLOAD_URL" ]; then
    echo -e "${RED}Error: Failed to find download URL for Dokfx template.${NC}" >&2
    exit 1
fi

# Download to a temporary location
TEMP_ZIP=$(mktemp)
# Append .zip extension for zip utilities on some platforms
mv "$TEMP_ZIP" "$TEMP_ZIP.zip"
TEMP_ZIP="$TEMP_ZIP.zip"

echo -e "${CYAN}Downloading template from: $DOWNLOAD_URL...${NC}"
if command -v curl >/dev/null 2>&1; then
    curl -L -o "$TEMP_ZIP" "$DOWNLOAD_URL"
elif command -v wget >/dev/null 2>&1; then
    wget -O "$TEMP_ZIP" "$DOWNLOAD_URL"
fi

# Clean destination folder
if [ -d "$TARGET_DIR" ]; then
    echo -e "${CYAN}Clearing existing template files in: $TARGET_DIR${NC}"
    rm -rf "${TARGET_DIR:?}"/*
else
    echo -e "${CYAN}Creating target directory: $TARGET_DIR${NC}"
    mkdir -p "$TARGET_DIR"
fi

# Extract zip
TEMP_EXTRACT_DIR=$(mktemp -d)
echo -e "${CYAN}Extracting template archive...${NC}"

if command -v unzip >/dev/null 2>&1; then
    unzip -q "$TEMP_ZIP" -d "$TEMP_EXTRACT_DIR"
else
    # Fallback to python extraction if unzip is missing
    if command -v python3 >/dev/null 2>&1 || command -v python >/dev/null 2>&1; then
        PYTHON_CMD="python3"
        if ! command -v python3 >/dev/null 2>&1; then
            PYTHON_CMD="python"
        fi
        $PYTHON_CMD -c "import zipfile; zipfile.ZipFile('$TEMP_ZIP').extractall('$TEMP_EXTRACT_DIR')"
    else
        echo -e "${RED}Error: Neither 'unzip' nor 'python' is available for zip extraction.${NC}" >&2
        # Clean up
        rm -f "$TEMP_ZIP"
        rm -rf "$TEMP_EXTRACT_DIR"
        exit 1
    fi
fi

# Handle wrapper folders (e.g. if we downloaded the source code zipball)
SOURCE_DIR="$TEMP_EXTRACT_DIR"
# Expand root items array
# shellcheck disable=SC2012
ROOT_ITEMS_COUNT=$(ls -1 "$TEMP_EXTRACT_DIR" | wc -l | tr -d ' ')
if [ "$ROOT_ITEMS_COUNT" -eq 1 ]; then
    SINGLE_ITEM=$(find "$TEMP_EXTRACT_DIR" -mindepth 1 -maxdepth 1)
    if [ -d "$SINGLE_ITEM" ]; then
        BASE_NAME=$(basename "$SINGLE_ITEM")
        if [ "$BASE_NAME" != "partials" ] && [ "$BASE_NAME" != "public" ]; then
            SOURCE_DIR="$SINGLE_ITEM"
            echo -e "${NC}Found nested directory structure inside zip: $BASE_NAME${NC}"
        fi
    fi
fi

# Copy template files to target
echo -e "${CYAN}Copying template files to target location...${NC}"
cp -R "$SOURCE_DIR"/* "$TARGET_DIR"/

# Clean up
rm -f "$TEMP_ZIP"
rm -rf "$TEMP_EXTRACT_DIR"

echo -e "${GREEN}Template updated successfully to version $TAG_NAME!${NC}"
