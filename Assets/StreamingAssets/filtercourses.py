import json
import re
import html

INPUT_FILE = "performanceCourses.json"
OUTPUT_FILE = "PFA_courses.json"

FIELDS_TO_KEEP = [
    "courseCode",
    "longTitle",
    "courseDescription",
    "prerequisites",
    "antirequisites",
    "customrequisites"
]

# regex for course codes like "RTA 201", "FCD 920"
COURSE_CODE_REGEX = re.compile(r"\b[A-Z]{3}\s?\d{3}\b")

def extract_course_codes(text):
    if not text:
        return None

    # unescape HTML entities
    text = html.unescape(text)

    # find all course codes
    codes = COURSE_CODE_REGEX.findall(text)

    return list(dict.fromkeys(codes)) if codes else None  # dedupe, preserve order


with open(INPUT_FILE, "r", encoding="utf-8") as f:
    courses = json.load(f)

filtered_courses = []

for course in courses:
    filtered = {}

    for field in FIELDS_TO_KEEP:
        if field in ["prerequisites", "antirequisites", "customrequisites"]:
            filtered[field] = extract_course_codes(course.get(field))
        else:
            filtered[field] = course.get(field)

    filtered_courses.append(filtered)

with open(OUTPUT_FILE, "w", encoding="utf-8") as f:
    json.dump(filtered_courses, f, indent=2, ensure_ascii=False)

print(f"Saved {len(filtered_courses)} cleaned courses to {OUTPUT_FILE}")
