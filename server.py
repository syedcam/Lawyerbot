#!/usr/bin/env python3
"""
Temporary Python backend to serve the Lawyer Directory application.
This serves both the static files and the API endpoints.
"""

from fastapi import FastAPI, Query
from fastapi.staticfiles import StaticFiles
from fastapi.responses import FileResponse
from typing import Optional, List
import json
import os

app = FastAPI(title="Lawyer Directory Backend")

# Sample data (in production this would come from a database)
LAWYERS = [
    {
        "id": 1,
        "name": "Sarah Johnson",
        "profilePictureUrl": "https://i.pravatar.cc/150?img=5",
        "areaOfPractice": "Corporate Law",
        "description": "Specializing in mergers and acquisitions with Fortune 500 companies",
        "yearsOfExperience": 15,
        "state": "California",
        "city": "San Francisco",
        "website": "https://sarahjohnsonlaw.com",
        "amlawRanking": "Top 100"
    },
    {
        "id": 2,
        "name": "Michael Chen",
        "profilePictureUrl": "https://i.pravatar.cc/150?img=12",
        "areaOfPractice": "Criminal Defense",
        "description": "Defending clients in state and federal courts for over 20 years",
        "yearsOfExperience": 20,
        "state": "New York",
        "city": "New York City",
        "website": "https://michaelchenlaw.com",
        "amlawRanking": "NR"
    },
    {
        "id": 3,
        "name": "Emily Rodriguez",
        "profilePictureUrl": "https://i.pravatar.cc/150?img=32",
        "areaOfPractice": "Family Law",
        "description": "Compassionate representation in divorce and custody cases",
        "yearsOfExperience": 10,
        "state": "Texas",
        "city": "Austin",
        "website": "https://emilyrodriguezlaw.com",
        "amlawRanking": "NR"
    },
    {
        "id": 4,
        "name": "David Park",
        "profilePictureUrl": "https://i.pravatar.cc/150?img=15",
        "areaOfPractice": "Intellectual Property",
        "description": "Patent and trademark expertise for tech startups",
        "yearsOfExperience": 12,
        "state": "California",
        "city": "Palo Alto",
        "website": "https://davidparkip.com",
        "amlawRanking": "Top 200"
    },
    {
        "id": 5,
        "name": "Jennifer Martinez",
        "profilePictureUrl": "https://i.pravatar.cc/150?img=24",
        "areaOfPractice": "Real Estate",
        "description": "Commercial and residential property transactions",
        "yearsOfExperience": 8,
        "state": "Florida",
        "city": "Miami",
        "website": "https://jennifermartinezlaw.com",
        "amlawRanking": "NR"
    },
    {
        "id": 6,
        "name": "Robert Williams",
        "profilePictureUrl": "https://i.pravatar.cc/150?img=11",
        "areaOfPractice": "Employment Law",
        "description": "Representing employees in discrimination and harassment cases",
        "yearsOfExperience": 18,
        "state": "Illinois",
        "city": "Chicago",
        "website": "https://robertwilliamslaw.com",
        "amlawRanking": "NR"
    },
    {
        "id": 7,
        "name": "Lisa Thompson",
        "profilePictureUrl": "https://i.pravatar.cc/150?img=47",
        "areaOfPractice": "Tax Law",
        "description": "Corporate tax planning and IRS dispute resolution",
        "yearsOfExperience": 25,
        "state": "New York",
        "city": "New York City",
        "website": "https://lisathompsontax.com",
        "amlawRanking": "Top 50"
    },
    {
        "id": 8,
        "name": "James Lee",
        "profilePictureUrl": "https://i.pravatar.cc/150?img=33",
        "areaOfPractice": "Personal Injury",
        "description": "Fighting for accident victims' rights and fair compensation",
        "yearsOfExperience": 14,
        "state": "California",
        "city": "Los Angeles",
        "website": "https://jamesleelaw.com",
        "amlawRanking": "NR"
    },
    {
        "id": 9,
        "name": "Amanda Davis",
        "profilePictureUrl": "https://i.pravatar.cc/150?img=20",
        "areaOfPractice": "Immigration Law",
        "description": "Helping families navigate complex immigration processes",
        "yearsOfExperience": 9,
        "state": "Texas",
        "city": "Houston",
        "website": "https://amandadavislaw.com",
        "amlawRanking": "NR"
    },
    {
        "id": 10,
        "name": "Christopher Brown",
        "profilePictureUrl": "https://i.pravatar.cc/150?img=52",
        "areaOfPractice": "Environmental Law",
        "description": "Protecting natural resources and holding polluters accountable",
        "yearsOfExperience": 16,
        "state": "Washington",
        "city": "Seattle",
        "website": "https://christopherbrownlaw.com",
        "amlawRanking": "Top 150"
    }
]

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
    states = sorted(list(set(l["state"] for l in LAWYERS)))
    return states

@app.get("/api/lawyers/cities")
async def get_cities(state: str = Query(...)):
    """Get list of cities for a specific state"""
    cities = sorted(list(set(l["city"] for l in LAWYERS if l["state"].lower() == state.lower())))
    return cities

@app.get("/api/lawyers/practices")
async def get_practice_areas():
    """Get list of all practice areas"""
    practices = sorted(list(set(l["areaOfPractice"] for l in LAWYERS)))
    return practices

@app.get("/api/lawyers")
async def get_all_lawyers():
    """Get all lawyers"""
    return sorted(LAWYERS, key=lambda x: x["name"])

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

if __name__ == "__main__":
    import uvicorn
    print("🚀 Starting Lawyer Directory Server...")
    print("📍 Open your browser to: http://localhost:5000")
    print("🎨 The modern black/white theme should now be visible!")
    uvicorn.run(app, host="0.0.0.0", port=5000)
