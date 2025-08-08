
from typing import List, Optional
from ..models import AdmissionRecord

# Common interface for all state adapters.

async def lookup(name: str, firm: Optional[str] = None) -> List[AdmissionRecord]:
    raise NotImplementedError
