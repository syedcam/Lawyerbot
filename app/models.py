
from pydantic import BaseModel, Field, HttpUrl
from typing import Optional, List
from datetime import date

class AdmissionRecord(BaseModel):
    state: str = Field(..., description="Two-letter state code")
    bar_number: Optional[str] = None
    admit_date: Optional[date] = None
    status: Optional[str] = None
    source_url: Optional[HttpUrl] = None

class Jurisdiction(BaseModel):
    state: str
    bar_number: Optional[str] = None
    admit_date: Optional[date] = None
    status: Optional[str] = None
    source_url: Optional[str] = None

class PracticeLabel(BaseModel):
    label: str
    confidence: float = 0.0
    evidence: List[str] = []

class RecentMatter(BaseModel):
    case: str
    court: Optional[str] = None
    date: Optional[date] = None
    tags: List[str] = []
    source: Optional[str] = None

class LookupResult(BaseModel):
    name: str
    jurisdictions: List[Jurisdiction]
    years_post_bar: Optional[float] = None
    top_practices: List[PracticeLabel] = []
    recent_matters: List[RecentMatter] = []
    notes: Optional[str] = None
