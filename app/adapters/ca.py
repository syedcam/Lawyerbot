
import httpx, re
from typing import List, Optional
from ..models import AdmissionRecord
from ..utils import parse_date_fuzzy

SEARCH_URL = "https://apps.calbar.ca.gov/attorney/Licensee/Detail/{}"  # requires bar number when known
NAME_SEARCH = "https://apps.calbar.ca.gov/attorney/LicenseeSearch/QuickSearch?FreeText={}"

HEADERS = {"User-Agent": "Mozilla/5.0 (MVP Bot; lawful, rate-limited)"}

async def lookup(name: str, firm: Optional[str] = None) -> List[AdmissionRecord]:
    # TODO: Implement real search by name via NAME_SEARCH, follow detail links.
    # Below is a stub that returns an empty list unless patterns are implemented.
    # Parsing hint (from the detail page):
    # - Admission: look for label 'Date Admitted' or 'Date Admitted to The State Bar of California'
    #   Example regex: r"Date Admitted[^\d]*(\w+ \d{1,2}, \d{4})"
    # - Bar Number: in header 'Bar Number: 123456'
    # - Status: 'License Status'
    return []
