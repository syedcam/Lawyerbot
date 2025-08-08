
from collections import Counter
from typing import List, Dict
from ..models import PracticeLabel

# Minimal keyword sets. Expand freely.
PRACTICE_KEYWORDS: Dict[str, List[str]] = {
    "Labor & Employment": ["FLSA", "wage and hour", "harassment", "retaliation", "wrongful termination", "PAGA"],
    "White Collar": ["FCPA", "False Claims Act", "grand jury", "DOJ", "indictment", "internal investigation"],
    "Intellectual Property": ["patent prosecution", "PTAB", "trademark", "copyright", "Lanham Act"],
    "Securities Litigation": ["10b-5", "securities fraud", "SEC enforcement", "class action", "PSLRA"],
    "Commercial Litigation": ["breach of contract", "business tort", "fiduciary duty", "trade secret"],
    "M&A/Corporate": ["merger", "acquisition", "stock purchase", "asset purchase", "private equity"],
    "Bankruptcy/Restructuring": ["chapter 11", "chapter 7", "DIP financing", "reorganization"],
}

def infer_practices(text_blobs: List[str]) -> List[PracticeLabel]:
    text = (" \n ".join(t.lower() for t in text_blobs if t)).lower()
    scores = {}
    evidences = {k: [] for k in PRACTICE_KEYWORDS}
    for label, kws in PRACTICE_KEYWORDS.items():
        score = 0
        for kw in kws:
            if kw.lower() in text:
                score += 1
                evidences[label].append(kw)
        if score:
            scores[label] = score
    if not scores:
        return []
    total = sum(scores.values())
    results = []
    for label, sc in Counter(scores).most_common():
        conf = sc / total if total else 0.0
        results.append(PracticeLabel(label=label, confidence=round(conf, 2), evidence=evidences[label]))
    # Top 3
    return results[:3]
