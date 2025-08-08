
import httpx, re
from typing import List, Optional
from ..models import AdmissionRecord
from ..utils import parse_date_fuzzy

# NY unified court system attorney search is form-based with POST; implement when ready.
SEARCH_URL = "https://iapps.courts.state.ny.us/attorney/AttorneySearch"

HEADERS = {"User-Agent": "Mozilla/5.0 (MVP Bot; lawful, rate-limited)"}

async def lookup(name: str, firm: Optional[str] = None) -> List[AdmissionRecord]:
    # TODO: Implement POST search and parse result table.
    # Parsing hints:
    # - Admission date often labeled 'Registration Number' (bar number) and 'Year Admitted' or 'Appellate Division Admission Date'
    # - Use regex: r"(Admitted|Admission).*?(\w+ \d{1,2}, \d{4}|\d{4})"
    return []
