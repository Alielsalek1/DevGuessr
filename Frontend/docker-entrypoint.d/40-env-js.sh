#!/bin/sh
set -eu

escape_js_string() {
  printf '%s' "$1" | sed -e 's/\\/\\\\/g' -e 's/"/\\"/g' -e 's/&/\\&/g' -e 's/|/\\|/g' -e 's/\r//g' -e ':a;N;$!ba;s/\n/\\n/g'
}

if [ -f /usr/share/nginx/html/env.template.js ]; then
  project_name=$(escape_js_string "${PROJECT_NAME:-DevGuessr}")
  project_description=$(escape_js_string "${PROJECT_DESCRIPTION:-The Architect's Forge - A high-performance, resilient backend template with Clean Architecture}")
  api_base_url=$(escape_js_string "${FRONTEND_API_BASE_URL:-/api/v1}")

  sed \
    -e "s|\${PROJECT_NAME}|${project_name}|g" \
    -e "s|\${PROJECT_DESCRIPTION}|${project_description}|g" \
    -e "s|\${FRONTEND_API_BASE_URL}|${api_base_url}|g" \
    < /usr/share/nginx/html/env.template.js \
    > /usr/share/nginx/html/env.js
fi
