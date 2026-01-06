import json
from copy import deepcopy

# -------- CONFIG --------
INPUT_FILE = "PFA.json"
OUTPUT_FILE = "PFAContext.json"

PROGRAM_CONTEXT = {
    "Acting": "none",
    "Production": "none",
    "Dance": "none"
}
# ------------------------

def add_context_to_nodes(data):
    if "nodes" not in data or not isinstance(data["nodes"], list):
        raise ValueError("JSON does not contain a valid 'nodes' array")

    for node in data["nodes"]:
        # Only add if missing (non-destructive)
        if "context" not in node:
            node["context"] = deepcopy(PROGRAM_CONTEXT)

    return data


def main():
    with open(INPUT_FILE, "r", encoding="utf-8") as f:
        data = json.load(f)

    updated_data = add_context_to_nodes(data)

    with open(OUTPUT_FILE, "w", encoding="utf-8") as f:
        json.dump(updated_data, f, indent=4, ensure_ascii=False)

    print(f"Context added to nodes. Output written to '{OUTPUT_FILE}'")


if __name__ == "__main__":
    main()
