# API Import Guide - Lawyer Directory

This guide explains how to import lawyer data into the database using the three supported formats: JSON, XML, and CSV.

## 📋 Table of Contents

1. [API Endpoints Overview](#api-endpoints-overview)
2. [JSON Import](#json-import)
3. [XML Import](#xml-import)
4. [CSV Import](#csv-import)
5. [Testing with cURL](#testing-with-curl)
6. [Testing with Postman](#testing-with-postman)
7. [Bot Integration](#bot-integration)

---

## API Endpoints Overview

All import endpoints are under `/api/lawyers/import/`:

| Endpoint | Method | Content Type | Description |
|----------|--------|--------------|-------------|
| `/api/lawyers/import/json` | POST | application/json | Import from JSON array |
| `/api/lawyers/import/xml` | POST | application/xml or text/xml | Import from XML |
| `/api/lawyers/import/csv` | POST | multipart/form-data | Upload CSV file |

### Response Format

All endpoints return an `ImportResult` object:

```json
{
  "successCount": 5,
  "failureCount": 1,
  "errors": [
    "Line 3: Duplicate - John Doe already exists in Boston, Massachusetts"
  ],
  "importedLawyers": [
    {
      "id": 21,
      "name": "Jane Smith",
      "profilePictureUrl": "https://example.com/photo.jpg",
      "areaOfPractice": "Corporate Law",
      "description": "Corporate attorney with 10 years experience",
      "yearsOfExperience": 10,
      "state": "New York",
      "city": "New York",
      "website": "https://example.com",
      "amlawRanking": "NR"
    }
  ]
}
```

---

## JSON Import

### Endpoint
```
POST /api/lawyers/import/json
Content-Type: application/json
```

### Request Format

Send an array of lawyer objects:

```json
[
  {
    "name": "Jane Smith",
    "profilePictureUrl": "https://i.pravatar.cc/150?img=25",
    "areaOfPractice": "Corporate Law, M&A",
    "description": "Experienced corporate attorney specializing in mergers and acquisitions.",
    "yearsOfExperience": 12,
    "state": "New York",
    "city": "New York",
    "website": "https://www.janesmith.law",
    "amlawRanking": "150"
  },
  {
    "name": "Robert Johnson",
    "profilePictureUrl": "https://i.pravatar.cc/150?img=26",
    "areaOfPractice": "Criminal Defense",
    "description": "Former prosecutor now defending criminal cases.",
    "yearsOfExperience": 8,
    "state": "California",
    "city": "Los Angeles",
    "website": "https://www.rjdefense.com",
    "amlawRanking": "NR"
  }
]
```

### Required Fields
- `name` (string, max 200 chars)
- `areaOfPractice` (string, max 500 chars)
- `description` (string, max 2000 chars)
- `yearsOfExperience` (integer, 0-100)
- `state` (string, max 100 chars)
- `city` (string, max 100 chars)

### Optional Fields
- `profilePictureUrl` (valid URL, max 500 chars)
- `website` (valid URL, max 500 chars)
- `amlawRanking` (string, max 10 chars, defaults to "NR")

---

## XML Import

### Endpoint
```
POST /api/lawyers/import/xml
Content-Type: application/xml
```

### Request Format

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Lawyers>
  <LawyerImportDto>
    <Name>Jane Smith</Name>
    <ProfilePictureUrl>https://i.pravatar.cc/150?img=25</ProfilePictureUrl>
    <AreaOfPractice>Corporate Law, M&amp;A</AreaOfPractice>
    <Description>Experienced corporate attorney specializing in mergers and acquisitions.</Description>
    <YearsOfExperience>12</YearsOfExperience>
    <State>New York</State>
    <City>New York</City>
    <Website>https://www.janesmith.law</Website>
    <AmlawRanking>150</AmlawRanking>
  </LawyerImportDto>
  <LawyerImportDto>
    <Name>Robert Johnson</Name>
    <ProfilePictureUrl>https://i.pravatar.cc/150?img=26</ProfilePictureUrl>
    <AreaOfPractice>Criminal Defense</AreaOfPractice>
    <Description>Former prosecutor now defending criminal cases.</Description>
    <YearsOfExperience>8</YearsOfExperience>
    <State>California</State>
    <City>Los Angeles</City>
    <Website>https://www.rjdefense.com</Website>
    <AmlawRanking>NR</AmlawRanking>
  </LawyerImportDto>
</Lawyers>
```

**Important**: The root element must be `<Lawyers>` and each lawyer must be wrapped in `<LawyerImportDto>`.

---

## CSV Import

### Endpoint
```
POST /api/lawyers/import/csv
Content-Type: multipart/form-data
```

### CSV Format

The CSV file must have a header row with the following columns (order doesn't matter):

```csv
Name,ProfilePictureUrl,AreaOfPractice,Description,YearsOfExperience,State,City,Website,AmlawRanking
Jane Smith,https://i.pravatar.cc/150?img=25,"Corporate Law, M&A",Experienced corporate attorney specializing in mergers and acquisitions.,12,New York,New York,https://www.janesmith.law,150
Robert Johnson,https://i.pravatar.cc/150?img=26,Criminal Defense,Former prosecutor now defending criminal cases.,8,California,Los Angeles,https://www.rjdefense.com,NR
```

### Required Columns
- Name
- AreaOfPractice
- Description
- YearsOfExperience
- State
- City

### Optional Columns
- ProfilePictureUrl
- Website
- AmlawRanking

**Notes**:
- Column names are case-insensitive
- Values with commas should be wrapped in quotes
- Empty optional fields will default to `null` or "NR" for AmlawRanking

---

## Testing with cURL

### Test JSON Import

**On Mac/Linux:**
```bash
curl -X POST http://localhost:5000/api/lawyers/import/json \
  -H "Content-Type: application/json" \
  -d '[
    {
      "name": "Test Lawyer",
      "areaOfPractice": "Test Practice",
      "description": "Test description",
      "yearsOfExperience": 5,
      "state": "California",
      "city": "San Francisco",
      "amlawRanking": "NR"
    }
  ]'
```

**On Windows (PowerShell):**
```powershell
$json = @'
[
  {
    "name": "Test Lawyer",
    "areaOfPractice": "Test Practice",
    "description": "Test description",
    "yearsOfExperience": 5,
    "state": "California",
    "city": "San Francisco",
    "amlawRanking": "NR"
  }
]
'@

Invoke-RestMethod -Uri "http://localhost:5000/api/lawyers/import/json" `
  -Method POST `
  -ContentType "application/json" `
  -Body $json
```

### Test XML Import

```bash
curl -X POST http://localhost:5000/api/lawyers/import/xml \
  -H "Content-Type: application/xml" \
  -d '<?xml version="1.0" encoding="UTF-8"?>
<Lawyers>
  <LawyerImportDto>
    <Name>Test Lawyer XML</Name>
    <AreaOfPractice>Test Practice</AreaOfPractice>
    <Description>Test description for XML import</Description>
    <YearsOfExperience>7</YearsOfExperience>
    <State>Texas</State>
    <City>Austin</City>
    <AmlawRanking>NR</AmlawRanking>
  </LawyerImportDto>
</Lawyers>'
```

### Test CSV Import

```bash
curl -X POST http://localhost:5000/api/lawyers/import/csv \
  -F "file=@sample_lawyers.csv"
```

---

## Testing with Postman

### Setup

1. Open Postman
2. Create a new request
3. Set method to **POST**
4. Enter URL: `http://localhost:5000/api/lawyers/import/json` (or xml/csv)

### For JSON:
1. Select **Body** tab
2. Choose **raw**
3. Set type to **JSON**
4. Paste JSON array
5. Click **Send**

### For XML:
1. Select **Body** tab
2. Choose **raw**
3. Set type to **XML**
4. Paste XML content
5. Click **Send**

### For CSV:
1. Select **Body** tab
2. Choose **form-data**
3. Change key type to **File**
4. Enter key name: `file`
5. Click **Select Files** and choose your CSV
6. Click **Send**

---

## Bot Integration

### Example: Python Bot

```python
import requests
import json

# API endpoint
url = "http://localhost:5000/api/lawyers/import/json"

# Lawyer data (could come from web scraping, database, etc.)
lawyers_data = [
    {
        "name": "Bot Imported Lawyer",
        "profilePictureUrl": "https://example.com/photo.jpg",
        "areaOfPractice": "Technology Law, IP",
        "description": "Specializes in technology and intellectual property law.",
        "yearsOfExperience": 10,
        "state": "Washington",
        "city": "Seattle",
        "website": "https://example.com",
        "amlawRanking": "NR"
    }
]

# Send POST request
response = requests.post(
    url,
    headers={"Content-Type": "application/json"},
    data=json.dumps(lawyers_data)
)

# Check response
if response.status_code == 200:
    result = response.json()
    print(f"✓ Successfully imported: {result['successCount']}")
    print(f"✗ Failed: {result['failureCount']}")
    if result['errors']:
        print("Errors:")
        for error in result['errors']:
            print(f"  - {error}")
else:
    print(f"Error: {response.status_code}")
    print(response.text)
```

### Example: Node.js Bot

```javascript
const axios = require('axios');

const url = 'http://localhost:5000/api/lawyers/import/json';

const lawyersData = [
  {
    name: 'Bot Imported Lawyer',
    profilePictureUrl: 'https://example.com/photo.jpg',
    areaOfPractice: 'Technology Law, IP',
    description: 'Specializes in technology and intellectual property law.',
    yearsOfExperience: 10,
    state: 'Washington',
    city: 'Seattle',
    website: 'https://example.com',
    amlawRanking: 'NR'
  }
];

axios.post(url, lawyersData, {
  headers: { 'Content-Type': 'application/json' }
})
.then(response => {
  const result = response.data;
  console.log(`✓ Successfully imported: ${result.successCount}`);
  console.log(`✗ Failed: ${result.failureCount}`);
  if (result.errors.length > 0) {
    console.log('Errors:');
    result.errors.forEach(error => console.log(`  - ${error}`));
  }
})
.catch(error => {
  console.error('Error:', error.message);
});
```

---

## Validation Rules

All imports perform the following validations:

1. **Required Fields**: Must be present and non-empty
2. **Field Lengths**: Must not exceed maximum character limits
3. **Years of Experience**: Must be 0-100
4. **URLs**: Must be valid URLs (if provided)
5. **Duplicates**: Same Name + State + City combination not allowed
6. **Data Types**: Fields must match expected types (numbers, strings, etc.)

---

## Error Handling

### Common Errors

| Error | Cause | Solution |
|-------|-------|----------|
| "No lawyer data provided" | Empty request body | Include lawyer data in request |
| "Validation failed for..." | Invalid field values | Check field types and lengths |
| "Duplicate: X already exists" | Lawyer already in database | Update existing or use different name/location |
| "Missing required columns" (CSV) | CSV missing required columns | Ensure CSV has all required columns |
| "Failed to parse XML" | Invalid XML format | Validate XML syntax |
| "Line X: Invalid YearsOfExperience" | Non-numeric or out of range | Use integer 0-100 |

---

## Best Practices

1. **Start Small**: Test with 1-2 records first
2. **Validate Data**: Check data quality before importing
3. **Handle Duplicates**: Check for existing lawyers before import
4. **Monitor Responses**: Always check `successCount` and `errors`
5. **Batch Imports**: For large datasets, import in batches of 50-100
6. **Log Results**: Keep track of import results for auditing
7. **Error Recovery**: Retry failed imports after fixing errors

---

## Sample Files Location

Sample files for testing are included in the `SampleImportFiles/` directory:
- `sample_lawyers.json`
- `sample_lawyers.xml`
- `sample_lawyers.csv`

---

## Support

For questions or issues with the import API:
1. Check the application logs
2. Verify data format matches examples above
3. Test with sample files first
4. Review validation error messages

