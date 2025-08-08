
import httpx, re
from typing import List, Optional
from ..models import AdmissionRecord
from ..utils import parse_date_fuzzy

SEARCH_URL = "https://www.texasbar.com/AM/Template.cfm?Section=Find_A_Lawyer"  # results are loaded via query
HEADERS = {"User-Agent": "Mozilla/5.0 (MVP Bot; lawful, rate-limited)"}

async def lookup(name: str, firm: Optional[str] = None) -> List[AdmissionRecord]:
    # TODO: Implement GET with query parameters for name; parse profile pages.
    # Hints:
    # - Detail page fields often include 'Bar Card Number' and 'Practice Areas' plus 'Admitted' with month/year.
    return []
