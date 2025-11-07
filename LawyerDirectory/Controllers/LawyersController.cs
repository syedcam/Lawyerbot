using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LawyerDirectory.Data;
using LawyerDirectory.Models;
using System.Xml.Serialization;
using System.Text;
using System.Globalization;

namespace LawyerDirectory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LawyersController : ControllerBase
    {
        private readonly LawyerContext _context;

        public LawyersController(LawyerContext context)
        {
            _context = context;
        }

        // GET: api/Lawyers/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Lawyer>>> SearchLawyers(
            [FromQuery] string? areaOfPractice,
            [FromQuery] int? minYears,
            [FromQuery] int? maxYears,
            [FromQuery] string? state,
            [FromQuery] string? city)
        {
            var query = _context.Lawyers.AsQueryable();

            // Filter by area of practice (case-insensitive, partial match)
            if (!string.IsNullOrWhiteSpace(areaOfPractice))
            {
                query = query.Where(l => l.AreaOfPractice.ToLower().Contains(areaOfPractice.ToLower()));
            }

            // Filter by minimum years of experience
            if (minYears.HasValue && minYears.Value >= 0)
            {
                query = query.Where(l => l.YearsOfExperience >= minYears.Value);
            }

            // Filter by maximum years of experience
            if (maxYears.HasValue && maxYears.Value >= 0)
            {
                query = query.Where(l => l.YearsOfExperience <= maxYears.Value);
            }

            // Filter by state (exact match, case-insensitive)
            if (!string.IsNullOrWhiteSpace(state))
            {
                query = query.Where(l => l.State.ToLower() == state.ToLower());
            }

            // Filter by city (exact match, case-insensitive)
            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(l => l.City.ToLower() == city.ToLower());
            }

            var lawyers = await query.OrderBy(l => l.Name).ToListAsync();
            return Ok(lawyers);
        }

        // GET: api/Lawyers/states
        [HttpGet("states")]
        public async Task<ActionResult<IEnumerable<string>>> GetStates()
        {
            var states = await _context.Lawyers
                .Select(l => l.State)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            return Ok(states);
        }

        // GET: api/Lawyers/cities?state=California
        [HttpGet("cities")]
        public async Task<ActionResult<IEnumerable<string>>> GetCitiesByState([FromQuery] string state)
        {
            if (string.IsNullOrWhiteSpace(state))
            {
                return BadRequest("State parameter is required");
            }

            var cities = await _context.Lawyers
                .Where(l => l.State.ToLower() == state.ToLower())
                .Select(l => l.City)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return Ok(cities);
        }

        // GET: api/Lawyers/practices
        [HttpGet("practices")]
        public async Task<ActionResult<IEnumerable<string>>> GetPracticeAreas()
        {
            var practices = await _context.Lawyers
                .SelectMany(l => l.AreaOfPractice.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(p => p.Trim())
                .Distinct()
                .OrderBy(p => p)
                .ToListAsync();

            return Ok(practices);
        }

        // GET: api/Lawyers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lawyer>>> GetAllLawyers()
        {
            return await _context.Lawyers.OrderBy(l => l.Name).ToListAsync();
        }

        // ===================================
        // DATA IMPORT ENDPOINTS
        // ===================================

        /// <summary>
        /// POST: api/Lawyers/import/json
        /// Import lawyers from JSON array
        /// </summary>
        /// <param name="lawyersData">Array of lawyer data in JSON format</param>
        /// <returns>Import result with success/failure counts</returns>
        [HttpPost("import/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(ImportResult), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ImportResult>> ImportFromJson([FromBody] List<LawyerImportDto> lawyersData)
        {
            if (lawyersData == null || !lawyersData.Any())
            {
                return BadRequest("No lawyer data provided");
            }

            var result = new ImportResult();

            foreach (var dto in lawyersData)
            {
                try
                {
                    // Validate the DTO
                    if (!ModelState.IsValid)
                    {
                        result.FailureCount++;
                        result.Errors.Add($"Validation failed for {dto.Name}: {string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))}");
                        continue;
                    }

                    // Check for duplicates (same name, state, city)
                    var exists = await _context.Lawyers
                        .AnyAsync(l => l.Name.ToLower() == dto.Name.ToLower()
                                    && l.State.ToLower() == dto.State.ToLower()
                                    && l.City.ToLower() == dto.City.ToLower());

                    if (exists)
                    {
                        result.FailureCount++;
                        result.Errors.Add($"Duplicate: {dto.Name} already exists in {dto.City}, {dto.State}");
                        continue;
                    }

                    // Map DTO to entity
                    var lawyer = new Lawyer
                    {
                        Name = dto.Name,
                        ProfilePictureUrl = dto.ProfilePictureUrl,
                        AreaOfPractice = dto.AreaOfPractice,
                        Description = dto.Description,
                        YearsOfExperience = dto.YearsOfExperience,
                        State = dto.State,
                        City = dto.City,
                        Website = dto.Website,
                        AmlawRanking = dto.AmlawRanking ?? "NR"
                    };

                    _context.Lawyers.Add(lawyer);
                    result.ImportedLawyers.Add(lawyer);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.FailureCount++;
                    result.Errors.Add($"Error importing {dto.Name}: {ex.Message}");
                }
            }

            // Save all changes
            if (result.SuccessCount > 0)
            {
                await _context.SaveChangesAsync();
            }

            return Ok(result);
        }

        /// <summary>
        /// POST: api/Lawyers/import/xml
        /// Import lawyers from XML format
        /// </summary>
        /// <param name="xmlContent">XML content as string</param>
        /// <returns>Import result with success/failure counts</returns>
        [HttpPost("import/xml")]
        [Consumes("application/xml", "text/xml")]
        [ProducesResponseType(typeof(ImportResult), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ImportResult>> ImportFromXml()
        {
            try
            {
                using var reader = new StreamReader(Request.Body, Encoding.UTF8);
                var xmlContent = await reader.ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(xmlContent))
                {
                    return BadRequest("No XML data provided");
                }

                // Deserialize XML
                var serializer = new XmlSerializer(typeof(List<LawyerImportDto>), new XmlRootAttribute("Lawyers"));
                List<LawyerImportDto> lawyersData;

                using (var stringReader = new StringReader(xmlContent))
                {
                    lawyersData = (List<LawyerImportDto>)serializer.Deserialize(stringReader)!;
                }

                if (lawyersData == null || !lawyersData.Any())
                {
                    return BadRequest("No valid lawyer data found in XML");
                }

                // Use the same import logic as JSON
                var result = new ImportResult();

                foreach (var dto in lawyersData)
                {
                    try
                    {
                        // Check for duplicates
                        var exists = await _context.Lawyers
                            .AnyAsync(l => l.Name.ToLower() == dto.Name.ToLower()
                                        && l.State.ToLower() == dto.State.ToLower()
                                        && l.City.ToLower() == dto.City.ToLower());

                        if (exists)
                        {
                            result.FailureCount++;
                            result.Errors.Add($"Duplicate: {dto.Name} already exists in {dto.City}, {dto.State}");
                            continue;
                        }

                        var lawyer = new Lawyer
                        {
                            Name = dto.Name,
                            ProfilePictureUrl = dto.ProfilePictureUrl,
                            AreaOfPractice = dto.AreaOfPractice,
                            Description = dto.Description,
                            YearsOfExperience = dto.YearsOfExperience,
                            State = dto.State,
                            City = dto.City,
                            Website = dto.Website,
                            AmlawRanking = dto.AmlawRanking ?? "NR"
                        };

                        _context.Lawyers.Add(lawyer);
                        result.ImportedLawyers.Add(lawyer);
                        result.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        result.FailureCount++;
                        result.Errors.Add($"Error importing {dto.Name}: {ex.Message}");
                    }
                }

                if (result.SuccessCount > 0)
                {
                    await _context.SaveChangesAsync();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to parse XML: {ex.Message}");
            }
        }

        /// <summary>
        /// POST: api/Lawyers/import/csv
        /// Import lawyers from CSV file
        /// Expected columns: Name, ProfilePictureUrl, AreaOfPractice, Description, YearsOfExperience, State, City, Website, AmlawRanking
        /// </summary>
        /// <param name="file">CSV file</param>
        /// <returns>Import result with success/failure counts</returns>
        [HttpPost("import/csv")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ImportResult), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ImportResult>> ImportFromCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("File must be a CSV file");
            }

            var result = new ImportResult();

            try
            {
                using var reader = new StreamReader(file.OpenReadStream());

                // Read header line
                var headerLine = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(headerLine))
                {
                    return BadRequest("CSV file is empty");
                }

                var headers = headerLine.Split(',').Select(h => h.Trim().Trim('"')).ToList();

                // Validate required columns
                var requiredColumns = new[] { "Name", "AreaOfPractice", "Description", "YearsOfExperience", "State", "City" };
                var missingColumns = requiredColumns.Where(rc => !headers.Any(h => h.Equals(rc, StringComparison.OrdinalIgnoreCase))).ToList();

                if (missingColumns.Any())
                {
                    return BadRequest($"Missing required columns: {string.Join(", ", missingColumns)}");
                }

                // Create column index map (case-insensitive)
                var columnMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < headers.Count; i++)
                {
                    columnMap[headers[i]] = i;
                }

                int lineNumber = 1;

                // Read data lines
                while (!reader.EndOfStream)
                {
                    lineNumber++;
                    var line = await reader.ReadLineAsync();

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    try
                    {
                        var values = ParseCsvLine(line);

                        if (values.Count < headers.Count)
                        {
                            result.FailureCount++;
                            result.Errors.Add($"Line {lineNumber}: Invalid number of columns");
                            continue;
                        }

                        var dto = new LawyerImportDto
                        {
                            Name = GetCsvValue(values, columnMap, "Name"),
                            ProfilePictureUrl = GetCsvValue(values, columnMap, "ProfilePictureUrl"),
                            AreaOfPractice = GetCsvValue(values, columnMap, "AreaOfPractice"),
                            Description = GetCsvValue(values, columnMap, "Description"),
                            State = GetCsvValue(values, columnMap, "State"),
                            City = GetCsvValue(values, columnMap, "City"),
                            Website = GetCsvValue(values, columnMap, "Website"),
                            AmlawRanking = GetCsvValue(values, columnMap, "AmlawRanking") ?? "NR"
                        };

                        // Parse years of experience
                        var yearsStr = GetCsvValue(values, columnMap, "YearsOfExperience");
                        if (!int.TryParse(yearsStr, out int years) || years < 0 || years > 100)
                        {
                            result.FailureCount++;
                            result.Errors.Add($"Line {lineNumber}: Invalid YearsOfExperience value '{yearsStr}'");
                            continue;
                        }
                        dto.YearsOfExperience = years;

                        // Validate required fields
                        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.AreaOfPractice) ||
                            string.IsNullOrWhiteSpace(dto.Description) || string.IsNullOrWhiteSpace(dto.State) ||
                            string.IsNullOrWhiteSpace(dto.City))
                        {
                            result.FailureCount++;
                            result.Errors.Add($"Line {lineNumber}: Missing required fields");
                            continue;
                        }

                        // Check for duplicates
                        var exists = await _context.Lawyers
                            .AnyAsync(l => l.Name.ToLower() == dto.Name.ToLower()
                                        && l.State.ToLower() == dto.State.ToLower()
                                        && l.City.ToLower() == dto.City.ToLower());

                        if (exists)
                        {
                            result.FailureCount++;
                            result.Errors.Add($"Line {lineNumber}: Duplicate - {dto.Name} already exists in {dto.City}, {dto.State}");
                            continue;
                        }

                        var lawyer = new Lawyer
                        {
                            Name = dto.Name,
                            ProfilePictureUrl = dto.ProfilePictureUrl,
                            AreaOfPractice = dto.AreaOfPractice,
                            Description = dto.Description,
                            YearsOfExperience = dto.YearsOfExperience,
                            State = dto.State,
                            City = dto.City,
                            Website = dto.Website,
                            AmlawRanking = dto.AmlawRanking
                        };

                        _context.Lawyers.Add(lawyer);
                        result.ImportedLawyers.Add(lawyer);
                        result.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        result.FailureCount++;
                        result.Errors.Add($"Line {lineNumber}: {ex.Message}");
                    }
                }

                if (result.SuccessCount > 0)
                {
                    await _context.SaveChangesAsync();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to process CSV file: {ex.Message}");
            }
        }

        // Helper method to parse CSV line (handles quoted values with commas)
        private List<string> ParseCsvLine(string line)
        {
            var values = new List<string>();
            var currentValue = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    values.Add(currentValue.ToString().Trim());
                    currentValue.Clear();
                }
                else
                {
                    currentValue.Append(c);
                }
            }

            values.Add(currentValue.ToString().Trim());
            return values;
        }

        // Helper method to get CSV value by column name
        private string? GetCsvValue(List<string> values, Dictionary<string, int> columnMap, string columnName)
        {
            if (columnMap.TryGetValue(columnName, out int index) && index < values.Count)
            {
                var value = values[index].Trim().Trim('"');
                return string.IsNullOrWhiteSpace(value) ? null : value;
            }
            return null;
        }
    }
}
