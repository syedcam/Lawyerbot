
import httpx, re
from typing import List, Optional
from ..models import AdmissionRecord
from ..utils import parse_date_fuzzy

SEARCH_URL = "https://www.iardc.org/lawyer-search"
HEADERS = {"User-Agent": "Mozilla/5.0 (MVP Bot; lawful, rate-limited)"}

async def lookup(name: str, firm: Optional[str] = None) -> List[AdmissionRecord]:
    # TODO: Implement search; ARDC shows 'Admission Date' and 'Registration Number' on profile.
    # Regex hint: r"Admission Date\s*:?\s*(\w+ \d{1,2}, \d{4})"
    return []
