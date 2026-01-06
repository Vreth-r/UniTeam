import json
import math

# -----------------------------
# CONFIG
# -----------------------------

INPUT_FILE = "PFA_courses_filtered.json"
OUTPUT_FILE = "PFA.json"

HORIZONTAL_SPACING = 350
VERTICAL_SPACING = 250
MAX_COLUMNS = 6   # wrap to next row after this many nodes

# -----------------------------
# LOAD JSON
# -----------------------------

with open(INPUT_FILE, "r", encoding="utf-8") as f:
    data = json.load(f)

courses = data["courses"]

# -----------------------------
# POSITIONING
# -----------------------------

for index, course in enumerate(courses):
    col = index % MAX_COLUMNS
    row = index // MAX_COLUMNS

    x = col * HORIZONTAL_SPACING
    y = -row * VERTICAL_SPACING  # negative Y works better for Unity UI

    # -----------------------------
    # ADD REQUIRED FIELDS
    # -----------------------------

    course["position"] = {
        "x": x,
        "y": y
    }

    # Starting node = no prerequisites
    course["isStartingNode"] = not course.get("prerequisites")

    # Default expansion flags (safe defaults)
    course["isExpansionNode"] = False
    course["startsExpanded"] = False

# -----------------------------
# SAVE OUTPUT
# -----------------------------

with open(OUTPUT_FILE, "w", encoding="utf-8") as f:
    json.dump(data, f, indent=2, ensure_ascii=False)

print(f"✅ Skill tree fields added. Output written to '{OUTPUT_FILE}'")
