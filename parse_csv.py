#!/usr/bin/env python3
"""
Parse the CSV data and convert to lawyer records format
"""

import csv
import json
from io import StringIO

# Paste the CSV data here (I'll fill this in)
CSV_DATA = """First Name	Last Name	Title	Company	Official Email	Personal Email	Email Used	Phone Number	Person Linkedin Url	Subject Line 1	Personalization 1	City	State	Link to Company Profile	LinkedIn Inmail	Comments	Yrs of Exp.	Amlaw Rank	Initial Campaign For	Defense or Plaintiff
Maria	G. Arroyo	Associate	Morgan, Lewis & Bockius LLP	maria.arroyo@morganlewis.com			310-907-1069	https://www.linkedin.com/in/maria-g-arroyo-54685685/	Labor & Employment Attorney	"Not sure if the timing is right, however, I think you would be a great fit for a Labor & Employment Associate role (partnership track) at an AmLaw 100 firm since you handle actions brought under the California Private Attorneys General Act (PAGA) at Morgan Lewis.

This Amlaw firm prioritizes work-life balance over maximizing billable hours, and associates can take on complex, high-profile, cross-border projects.

Salary range is $260,000 - $355,000/yr.

They are looking for the following:  3 to 5 years of Labor & Employment experience (PAGA actions, wage and hour class actions, FLSA collective action) to join their Los Angeles office. The ideal candidate has experience defending single-plaintiff employment cases.

Since this is a confidential search I would be able to go over details offline.

Let me know if you would like more information?"	L.A	CA	www.morganlewis.com/bios/mariaarroyo	"Hi Maria,

 An Amlaw 100 firm with a renowned labor and employment practice group in Los Angeles is looking for someone with your experience (employment disputes brought under PAGA).

 Salary is 260K-355K/yr.

 Would you like to know more?"	"5yrs 3mos exp.

 Jen: LI invite sent 07/21/25

 Syed: Left voicemail and LinkedIn invite on 3/25/2025

 Leopard List: 03/24/25
 4yrs 8mos exp."	5	10	McGuireWoods	Defense"""

def clean_city(city):
    """Convert city abbreviations to full names"""
    if not city:
        return ""
    city = city.strip()
    if city == "L.A":
        return "Los Angeles"
    return city

def clean_state(state):
    """Convert state abbreviations to full names"""
    if not state:
        return ""
    state = state.strip()
    if state == "CA":
        return "California"
    return state

def extract_years(years_str):
    """Extract numeric years from the years of experience field"""
    if not years_str:
        return 0

    years_str = str(years_str).strip()

    # Try to extract just the number
    import re
    match = re.search(r'(\d+)', years_str)
    if match:
        return int(match.group(1))
    return 0

def parse_amlaw_rank(rank_str):
    """Parse AmLaw ranking"""
    if not rank_str or rank_str == "-" or rank_str == "":
        return "NR"

    rank_str = str(rank_str).strip()
    if rank_str.isdigit():
        return f"Top {rank_str}"
    return rank_str

# Parse TSV (tab-separated)
lawyers = []
reader = csv.DictReader(StringIO(CSV_DATA), delimiter='\t')

for idx, row in enumerate(reader, 1):
    first_name = row.get('First Name', '').strip()
    last_name = row.get('Last Name', '').strip()

    if not first_name or not last_name:
        continue

    lawyer = {
        "id": idx,
        "name": f"{first_name} {last_name}",
        "firstName": first_name,
        "lastName": last_name,
        "company": row.get('Company', '').strip(),
        "email": row.get('Official Email', '').strip(),
        "phone": row.get('Phone Number', '').strip(),
        "linkedinUrl": row.get('Person Linkedin Url', '').strip(),
        "companyProfileUrl": row.get('Link to Company Profile', '').strip(),
        "areaOfPractice": "Labor and Employment",
        "description": "",  # Leave blank for now
        "profilePictureUrl": "",  # Leave blank for now
        "yearsOfExperience": extract_years(row.get('Yrs of Exp.', 0)),
        "city": clean_city(row.get('City', '')),
        "state": clean_state(row.get('State', '')),
        "amlawRanking": parse_amlaw_rank(row.get('Amlaw Rank', 'NR')),
        "website": ""  # Not in CSV, leaving blank
    }

    lawyers.append(lawyer)

# Print as Python list for copying
print(f"# Found {len(lawyers)} lawyers")
print("\nLAWYERS = [")
for lawyer in lawyers:
    print(f"    {json.dumps(lawyer, indent=4)},")
print("]")
