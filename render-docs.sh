#!/bin/zsh

DOCUMENT_PREVIEW="document-preview"

rm -rf "$DOCUMENT_PREVIEW"

mkdir "$DOCUMENT_PREVIEW"

pandoc -F mermaid-filter -o "./$DOCUMENT_PREVIEW/readme.html" "./readme.md"