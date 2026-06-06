#!/usr/bin/env bash

# Dynamically generates a GitHub Actions build matrix based on changed files, a JSON project map, and dependencies.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECTS_JSON="$SCRIPT_DIR/projects.json"

export EVENT_NAME="${EVENT_NAME:-}"
export FILTER_OUTPUTS="${FILTER_OUTPUTS:-}"
export GITHUB_OUTPUT="${GITHUB_OUTPUT:-}"

if [ -z "$FILTER_OUTPUTS" ]; then
  export FILTER_OUTPUTS="{}"
fi

if [ ! -f "$PROJECTS_JSON" ]; then
  echo "Error: Projects configuration file not found at $PROJECTS_JSON" >&2
  exit 1
fi

if ! command -v jq >/dev/null 2>&1; then
  echo "Error: jq is required but was not found." >&2
  exit 1
fi

# Clean/fallback for FILTER_OUTPUTS in jq
if ! echo "$FILTER_OUTPUTS" | jq -e . >/dev/null 2>&1; then
  export FILTER_OUTPUTS="{}"
fi

# Build matrix JSON using jq and recursive dependency resolution
MATRIX_JSON=$(jq -c \
  --arg event_name "$EVENT_NAME" \
  --argjson filter_outputs "$FILTER_OUTPUTS" \
  '
  # 1. Convert projects map to an array of project objects with name and direct_active status
  to_entries | map(
    .key as $name |
    .value + {
      name: $name,
      direct_active: (
        $event_name == "workflow_dispatch" or
        ($filter_outputs[.value.filter_key // $name] | (. == true or . == "true"))
      )
    }
  ) as $projects |

  # 2. Get list of directly active project names
  [ $projects[] | select(.direct_active) | .name ] as $start |

  # 3. Recursive function to find transitive closure of names
  def get_active($active):
    [
      $projects[] |
      select(
        .name as $n |
        # Not already in the active set
        ($active | index($n) == null) and
        # Has at least one dependency in the active set
        any(.dependencies[]; . as $dep | $active | index($dep) != null)
      ) |
      .name
    ] as $new_active |
    if ($new_active | length) > 0 then
      get_active($active + $new_active)
    else
      $active
    end;

  # Compute final active list
  get_active($start) as $final_active |

  # 4. Map back to the required list of matrix objects
  [
    $projects[] |
    select(.name as $n | $final_active | index($n) != null) |
    {
      name: .name,
      path: .path,
      test_path: .test_path
    }
  ]
  ' "$PROJECTS_JSON")

echo "Generated matrix: $MATRIX_JSON"

# Output results to GitHub Actions or stdout
if [ -n "${GITHUB_OUTPUT}" ]; then
  if [ "$MATRIX_JSON" = "[]" ] || [ "$MATRIX_JSON" = "" ]; then
    echo "has_projects=false" >> "$GITHUB_OUTPUT"
    echo "matrix={\"include\":[]}" >> "$GITHUB_OUTPUT"
  else
    echo "has_projects=true" >> "$GITHUB_OUTPUT"
    echo "matrix<<EOF" >> "$GITHUB_OUTPUT"
    echo "{\"include\":$MATRIX_JSON}" >> "$GITHUB_OUTPUT"
    echo "EOF" >> "$GITHUB_OUTPUT"
  fi
else
  if [ "$MATRIX_JSON" = "[]" ] || [ "$MATRIX_JSON" = "" ]; then
    echo "has_projects=false"
    echo "matrix={\"include\":[]}"
  else
    echo "has_projects=true"
    echo "matrix={\"include\":$MATRIX_JSON}"
  fi
fi
