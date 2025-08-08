
# Lawyer Experience Bot — v1 (CA, NY, TX, IL)

A FastAPI-based MVP that looks up **year admitted** and **practice focus** for attorneys in **CA, NY, TX, IL**.

> This is a scaffold with working API endpoints and pluggable state adapters. The scraping/admission parsing functions contain
> clear TODOs and realistic selectors/regex to implement against each bar's public directory.

## Features
- `/lookup`: Resolve a lawyer by name + optional state; pulls earliest active admission date across CA/NY/TX/IL and computes years post-bar.
- `/batch`: CSV upload (name, firm, state); returns CSV with results.
- Evidence-first practice inference (maps firm bio/recent matters to normalized labels).

## Quick start

### 1) Setup (local)
```bash
python -m venv .venv && source .venv/bin/activate
pip install -r requirements.txt
uvicorn app.main:app --reload
```

### 2) Example
```bash
curl 'http://127.0.0.1:8000/lookup?name=Jane%20Doe&state=CA'
```

### 3) Docker
```bash
docker build -t lawyer-experience-bot -f docker/Dockerfile .
docker run -p 8000:8000 lawyer-experience-bot
```

## Adapters
State-specific modules live in `app/adapters/`. Each implements:
```python
async def lookup(name: str, firm: str | None = None) -> list[AdmissionRecord]
```
Return zero or more `AdmissionRecord` items (state, bar_number, admit_date, status, url).

### Implementing real lookups
- Use `httpx.AsyncClient` with a browser-like header.
- Respect robots.txt and rate limits. Backoff on 429.
- Parse using `selectolax` or `lxml` with explicit CSS/XPath selectors.
- Normalize dates with `utils.parse_date_fuzzy`.

Each adapter includes commented selectors/regex you can enable when ready.

## Practice inference
- `app/inference/practice.py` maps raw text (firm bio/press/cases) to normalized practice areas using keyword sets.
- Swap in a vector model later; keep the same interface.

## Batch processing
- POST `/batch` with a CSV (columns: name, firm, state). The API streams back a CSV with results.

## Notes
- CourtListener integration stubbed behind `COURTLISTENER_API_KEY` env var. Add later for matter-based signals.
- This is template code. You must plug in per-state scraping details and test with live pages.

