#!/usr/bin/env python3
"""
Import lawyer data from CSV and generate Python code for server.py
"""

# For now, I'll create a smaller representative sample from the data you provided
# In production, you would parse the full CSV

LAWYERS_DATA = """
First Name,Last Name,Company,Email,Phone,LinkedIn,City,State,Years,AmLaw,Profile URL
Maria,G. Arroyo,Morgan Lewis & Bockius LLP,maria.arroyo@morganlewis.com,310-907-1069,https://www.linkedin.com/in/maria-g-arroyo-54685685/,Los Angeles,California,5,10,www.morganlewis.com/bios/mariaarroyo
Mayra,Negrete,Morgan Lewis & Bockius LLP,mayra.negrete@morganlewis.com,714-830-0543,https://www.linkedin.com/in/mayra-negrete-a3b853152/,Los Angeles,California,4,10,www.morganlewis.com/bios/mayranegrete
Oscar,E. Peralta,Greenberg Traurig LLP,oscar.peralta@gtlaw.com,310-586-7700,https://www.linkedin.com/in/oscar-eduardo-peralta-74b6a0140/,Los Angeles,California,4,14,www.gtlaw.com/en/professionals/p/peralta-oscar-e
"""

import csv
from io import StringIO
import json

lawyers = []
reader = csv.DictReader(StringIO(LAWYERS_DATA.strip()))

for idx, row in enumerate(reader, 1):
    lawyer = {
        "id": idx,
        "name": f"{row['First Name'].strip()} {row['Last Name'].strip()}",
        "firstName": row['First Name'].strip(),
        "lastName": row['Last Name'].strip(),
        "company": row['Company'].strip(),
        "email": row['Email'].strip(),
        "phone": row['Phone'].strip(),
        "linkedinUrl": row['LinkedIn'].strip(),
        "companyProfileUrl": row['Profile URL'].strip() if row['Profile URL'].strip() else "",
        "areaOfPractice": "Labor and Employment",
        "description": "",
        "profilePictureUrl": "https://i.pravatar.cc/150?img=" + str(idx + 4),  # Placeholder avatars
        "yearsOfExperience": int(row['Years']) if row['Years'].isdigit() else 0,
        "city": row['City'].strip(),
        "state": row['State'].strip(),
        "amlawRanking": f"Top {row['AmLaw']}" if row['AmLaw'] and row['AmLaw'].strip() and row['AmLaw'] != "-" else "NR",
        "website": ""
    }
    lawyers.append(lawyer)

print(json.dumps(lawyers, indent=2))
