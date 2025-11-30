#!/usr/bin/env python3
"""
Lawyer Directory Backend - Reads data from CSV file
"""

from fastapi import FastAPI, Query
from fastapi.staticfiles import StaticFiles
from fastapi.responses import FileResponse
from typing import Optional, List
import csv
import os
import re

app = FastAPI(title="Lawyer Directory Backend")

# Global variable to store lawyers
LAWYERS = []

def clean_city(city):
    """Convert city abbreviations to full names"""
    if not city:
        return ""
    city = city.strip()
    if city in ["L.A", "L.A."]:
        return "Los Angeles"
    if city in ["SFO", "SF", "S.F", "S.F."]:
        return "San Francisco"
    return city

def clean_state(state):
    """Convert state abbreviations to full names"""
    if not state:
        return ""
    state = state.strip()
    if state == "CA":
        return "California"
    if state == "NY":
        return "New York"
    if state == "IL":
        return "Illinois"
    # Add more state mappings as needed
    return state

def extract_years(years_str):
    """Extract numeric years from the years of experience field"""
    if not years_str:
        return 0

    years_str = str(years_str).strip()

    # Extract just the first number found
    match = re.search(r'(\d+)', years_str)
    if match:
        return int(match.group(1))
    return 0

def parse_amlaw_rank(rank_str):
    """Parse AmLaw ranking"""
    if not rank_str or rank_str == "-" or rank_str == "" or rank_str == "0":
        return "Not Rated"

    rank_str = str(rank_str).strip()
    # Return just the number, not "Top {number}"
    if rank_str.isdigit():
        return rank_str
    return rank_str

def load_lawyers_from_csv_file(csv_filename, starting_id=1):
    """Load lawyer data from a single CSV file"""
    lawyers = []
    csv_path = os.path.join(os.path.dirname(__file__), csv_filename)

    if not os.path.exists(csv_path):
        print(f"Warning: CSV file not found at {csv_path}")
        return lawyers

    try:
        with open(csv_path, 'r', encoding='utf-8') as f:
            # CSV is comma-delimited
            reader = csv.DictReader(f, delimiter=',')

            for idx, row in enumerate(reader, starting_id):
                first_name = row.get('First Name', '').strip()
                last_name = row.get('Last Name', '').strip()

                if not first_name or not last_name:
                    continue

                # Get profile picture URL from CSV, or leave empty if not available
                profile_pic = row.get('Profile Picture', '').strip()
                # Don't use placeholder - just leave empty if no picture
                if not profile_pic:
                    profile_pic = ""

                # Handle both "Phone Number" and "Phone Number_old" column names
                phone = row.get('Phone Number', '').strip()
                if not phone:
                    phone = row.get('Phone Number_old', '').strip()

                lawyer = {
                    "id": idx,
                    "name": f"{first_name} {last_name}",
                    "firstName": first_name,
                    "lastName": last_name,
                    "title": row.get('Title', '').strip(),
                    "company": row.get('Company', '').strip(),
                    "email": row.get('Official Email', '').strip(),
                    "phone": phone,
                    "linkedinUrl": row.get('Person Linkedin Url', '').strip(),
                    "companyProfileUrl": row.get('Link to Company Profile', '').strip(),
                    "practiceArea": row.get('Practice Area', '').strip() or "Labor and Employment",
                    "durationInCurrentRole": row.get('Duration in Current Role', '').strip(),
                    "areaOfPractice": row.get('Practice Area', '').strip() or "Labor and Employment",
                    "description": "",  # Can be populated from company profile later
                    "profilePictureUrl": profile_pic,
                    "yearsOfExperience": extract_years(row.get('Years of Experience', '0')),
                    "city": clean_city(row.get('City', '')),
                    "state": clean_state(row.get('State', '')),
                    "amlawRanking": parse_amlaw_rank(row.get('Amlaw Rank', 'NR')),
                    "website": ""  # Not in CSV
                }

                lawyers.append(lawyer)

        print(f"✅ Loaded {len(lawyers)} lawyers from {csv_filename}")

    except Exception as e:
        print(f"Error loading {csv_filename}: {e}")

    return lawyers

def load_lawyers_from_csv():
    """Load lawyer data from all CSV files"""
    global LAWYERS
    LAWYERS = []

    # Load Los Angeles lawyers
    la_lawyers = load_lawyers_from_csv_file("LA_Lsearch_Masterlist_LaborEmployment_updated.csv", starting_id=1)
    LAWYERS.extend(la_lawyers)

    # Load San Francisco lawyers - continue ID sequence
    next_id = len(LAWYERS) + 1
    sf_lawyers = load_lawyers_from_csv_file("Final_SFO_Lsearch_Masterlist_LaborEmployment_updated.csv", starting_id=next_id)
    LAWYERS.extend(sf_lawyers)

    # Load New York lawyers - continue ID sequence
    next_id = len(LAWYERS) + 1
    ny_lawyers = load_lawyers_from_csv_file("NY_MandA_updated_FromCooley-Final.csv", starting_id=next_id)
    LAWYERS.extend(ny_lawyers)

    # Load Chicago lawyers - continue ID sequence
    next_id = len(LAWYERS) + 1
    chicago_lawyers = load_lawyers_from_csv_file("Chicago_MandA_Chicago_4-6y.csv", starting_id=next_id)
    LAWYERS.extend(chicago_lawyers)

    # If no data loaded, use sample data
    if len(LAWYERS) == 0:
        print("Warning: No CSV files found, loading sample data")
        LAWYERS = get_sample_data()
    else:
        print(f"✅ Total: {len(LAWYERS)} lawyers loaded from all CSV files")

def get_sample_data():
    """Return sample data if CSV is not available"""
    return [
        {
            "id": 1,
            "name": "Maria G. Arroyo",
            "firstName": "Maria",
            "lastName": "G. Arroyo",
            "company": "Morgan, Lewis & Bockius LLP",
            "email": "maria.arroyo@morganlewis.com",
            "phone": "310-907-1069",
            "linkedinUrl": "https://www.linkedin.com/in/maria-g-arroyo-54685685/",
            "companyProfileUrl": "www.morganlewis.com/bios/mariaarroyo",
            "areaOfPractice": "Labor and Employment",
            "description": "",
            "profilePictureUrl": "https://i.pravatar.cc/150?img=5",
            "yearsOfExperience": 5,
            "city": "Los Angeles",
            "state": "California",
            "amlawRanking": "Top 10",
            "website": ""
        }
    ]

# Load lawyers on startup
load_lawyers_from_csv()

@app.get("/api/lawyers/search")
async def search_lawyers(
    areaOfPractice: Optional[str] = Query(None),
    minYears: Optional[int] = Query(None),
    maxYears: Optional[int] = Query(None),
    state: Optional[str] = Query(None),
    city: Optional[str] = Query(None)
):
    """Search lawyers based on filter criteria"""
    results = LAWYERS.copy()

    # Filter by area of practice
    if areaOfPractice:
        results = [l for l in results if areaOfPractice.lower() in l["areaOfPractice"].lower()]

    # Filter by minimum years
    if minYears is not None and minYears >= 0:
        results = [l for l in results if l["yearsOfExperience"] >= minYears]

    # Filter by maximum years
    if maxYears is not None and maxYears >= 0:
        results = [l for l in results if l["yearsOfExperience"] <= maxYears]

    # Filter by state
    if state:
        results = [l for l in results if l["state"].lower() == state.lower()]

    # Filter by city
    if city:
        results = [l for l in results if l["city"].lower() == city.lower()]

    return results

@app.get("/api/lawyers/states")
async def get_states():
    """Get list of all states"""
    states = sorted(list(set(l["state"] for l in LAWYERS if l["state"])))
    return states

@app.get("/api/lawyers/cities")
async def get_cities(state: str = Query(...)):
    """Get list of cities for a specific state"""
    cities = sorted(list(set(l["city"] for l in LAWYERS if l["state"].lower() == state.lower() and l["city"])))
    return cities

@app.get("/api/lawyers/practices")
async def get_practice_areas():
    """Get list of all practice areas"""
    practices = sorted(list(set(l["areaOfPractice"] for l in LAWYERS if l["areaOfPractice"])))
    return practices

@app.get("/api/lawyers")
async def get_all_lawyers():
    """Get all lawyers"""
    return sorted(LAWYERS, key=lambda x: x["name"])

@app.get("/api/lawyers/{lawyer_id}")
async def get_lawyer_by_id(lawyer_id: int):
    """Get a single lawyer by ID"""
    for lawyer in LAWYERS:
        if lawyer["id"] == lawyer_id:
            return lawyer
    return {"error": "Lawyer not found"}

# Serve static files - use relative path from script location
import os
script_dir = os.path.dirname(os.path.abspath(__file__))
static_path = os.path.join(script_dir, "LawyerDirectory", "wwwroot")
app.mount("/css", StaticFiles(directory=os.path.join(static_path, "css")), name="css")
app.mount("/js", StaticFiles(directory=os.path.join(static_path, "js")), name="js")

@app.get("/")
async def serve_index():
    """Serve the main index.html file"""
    return FileResponse(os.path.join(static_path, "index.html"))

@app.get("/detail.html")
async def serve_detail():
    """Serve the detail.html file"""
    return FileResponse(os.path.join(static_path, "detail.html"))

if __name__ == "__main__":
    import uvicorn
    print("🚀 Starting Lawyer Directory Server...")
    print("📍 Open your browser to: http://localhost:8000")
    print("🎨 The modern black/white theme should now be visible!")
    uvicorn.run(app, host="0.0.0.0", port=8000)
