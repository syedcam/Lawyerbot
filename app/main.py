
from fastapi import FastAPI, Query, UploadFile, File, HTTPException
from fastapi.responses import StreamingResponse, JSONResponse
from typing import Optional, List, Dict
from datetime import date
import io, csv, asyncio

from .models import LookupResult, Jurisdiction
from .utils import years_since
from .adapters import STATE_MODULES
from .inference.practice import infer_practices

app = FastAPI(title="Lawyer Experience Bot (MVP)", version="0.1.0")

async def gather_admissions(name: str, firm: Optional[str]) -> List[Jurisdiction]:
    tasks = [STATE_MODULES[s].lookup(name, firm) for s in STATE_MODULES]
    results = await asyncio.gather(*tasks)
    jurisdictions: List[Jurisdiction] = []
    for state, recs in zip(STATE_MODULES.keys(), results):
        for r in recs:
            jurisdictions.append(Jurisdiction(
                state=r.state,
                bar_number=r.bar_number,
                admit_date=r.admit_date,
                status=r.status,
                source_url=r.source_url,
            ))
    # Deduplicate by (state, bar_number) if present
    seen = set()
    uniq = []
    for j in jurisdictions:
        key = (j.state, j.bar_number or "", j.admit_date or date.min)
        if key in seen:
            continue
        seen.add(key)
        uniq.append(j)
    return uniq

@app.get("/lookup", response_model=LookupResult)
async def lookup(name: str = Query(...), firm: Optional[str] = Query(None)):
    jurisdictions = await gather_admissions(name, firm)
    # earliest active admission
    admit_dates = [j.admit_date for j in jurisdictions if j.admit_date]
    earliest = min(admit_dates) if admit_dates else None
    ypb = years_since(earliest) if earliest else None

    # Practice inference — currently uses only firm text blobs placeholder
    practice_labels = infer_practices([firm or ""])

    return LookupResult(
        name=name,
        jurisdictions=jurisdictions,
        years_post_bar=ypb,
        top_practices=practice_labels,
        recent_matters=[],
        notes="State adapters are stubs. Plug in live selectors to enable real results."
    )

@app.post("/batch")
async def batch(file: UploadFile = File(...)):
    if not file.filename.lower().endswith(".csv"):
        raise HTTPException(status_code=400, detail="Upload a CSV with columns: name, firm, state(optional)")
    content = await file.read()
    stream = io.StringIO(content.decode("utf-8"))
    reader = csv.DictReader(stream)
    output = io.StringIO()
    writer = csv.writer(output)
    writer.writerow(["name","years_post_bar","jurisdictions","top_practices"])
    for row in reader:
        name = row.get("name","").strip()
        firm = row.get("firm","").strip() or None
        if not name:
            continue
        res: LookupResult = await lookup(name=name, firm=firm)  # type: ignore
        jtxt = "; ".join(f"{j.state}:{j.admit_date or ''}" for j in res.jurisdictions)
        ptxt = "; ".join(f"{p.label}({p.confidence})" for p in res.top_practices)
        writer.writerow([res.name, res.years_post_bar or "", jtxt, ptxt])
    output.seek(0)
    return StreamingResponse(output, media_type="text/csv", headers={"Content-Disposition": "attachment; filename=results.csv"})
