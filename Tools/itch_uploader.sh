#!/usr/bin/env bash
set -e

echo "=== Starting itch.io Butler Deployment ==="

if [ -n "$BUTLER_API_KEY" ]; then
    echo "::mask-value::$BUTLER_API_KEY"
fi

if [ -z "$BUTLER_API_KEY" ] || [ -z "$ITCH_USER" ] || [ -z "$ITCH_GAME" ] || [ -z "$ITCH_CHANNEL" ]; then
    echo "Error: Missing required environment variables (BUTLER_API_KEY, ITCH_USER, ITCH_GAME, ITCH_CHANNEL)."
    exit 1
fi

UPLOAD_DIR="$OUTPUT_DIRECTORY"
echo "Initial upload path: $UPLOAD_DIR"

# WebGL: Find nested index.html
INDEX_PATH=$(find "$OUTPUT_DIRECTORY" -maxdepth 3 -name "index.html" -print -quit 2>/dev/null || true)
if [ -n "$INDEX_PATH" ]; then
    UPLOAD_DIR="$(dirname "$INDEX_PATH")"
    echo "Detected WebGL build with index.html at: $INDEX_PATH"
fi
echo "Final upload path: $OUTPUT_DIRECTORY"


mkdir -p ./butler-bin

if [[ "$BUILDER_OS" == "WINDOWS" ]]; then
    echo "Detected Windows Builder"
    curl -L -o butler.zip https://broth.itch.zone/butler/windows-amd64/LATEST/archive/default
    unzip -q -o butler.zip -d ./butler-bin
    BUTLER_EXE="./butler-bin/butler.exe"
else
    echo "Detected macOS/Linux Builder"
    curl -L -o butler.zip https://broth.itch.zone/butler/darwin-amd64/LATEST/archive/default
    unzip -q -o butler.zip -d ./butler-bin
    chmod +x ./butler-bin/butler
    BUTLER_EXE="./butler-bin/butler"
fi

echo "--- Butler Version ---"
$BUTLER_EXE -V

# 5. Push to itch.io
VERSION="${UCB_BUILD_NUMBER:-1}"
echo "Uploading build #${VERSION} to itch.io (${ITCH_USER}/${ITCH_GAME}:${ITCH_CHANNEL})..."

$BUTLER_EXE push "$UPLOAD_DIR" "${ITCH_USER}/${ITCH_GAME}:${ITCH_CHANNEL}" --userversion "$VERSION"

echo "=== itch.io deployment successfully completed! ==="
exit 0
