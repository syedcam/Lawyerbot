
from datetime import datetime, date
from dateutil import parser

def parse_date_fuzzy(s: str | None):
    if not s:
        return None
    try:
        return parser.parse(s, fuzzy=True).date()
    except Exception:
        return None

def years_since(d: date | None, today: date | None = None) -> float | None:
    if not d:
        return None
    today = today or date.today()
    delta = (today - d).days
    return round(delta / 365.25, 1)
