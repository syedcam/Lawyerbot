using System.ComponentModel.DataAnnotations;

namespace LawyerDirectory.Models
{
    /// <summary>
    /// Data Transfer Object for importing lawyer data
    /// Used for JSON, XML, and CSV imports
    /// </summary>
    public class LawyerImportDto
    {
        [Required(ErrorMessage = "Lawyer name is required")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        [Url(ErrorMessage = "Profile picture must be a valid URL")]
        [StringLength(500, ErrorMessage = "Profile picture URL cannot exceed 500 characters")]
        public string? ProfilePictureUrl { get; set; }

        [Required(ErrorMessage = "Area of practice is required")]
        [StringLength(500, ErrorMessage = "Area of practice cannot exceed 500 characters")]
        public string AreaOfPractice { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Years of experience is required")]
        [Range(0, 100, ErrorMessage = "Years of experience must be between 0 and 100")]
        public int YearsOfExperience { get; set; }

        [Required(ErrorMessage = "State is required")]
        [StringLength(100, ErrorMessage = "State cannot exceed 100 characters")]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string City { get; set; } = string.Empty;

        [Url(ErrorMessage = "Website must be a valid URL")]
        [StringLength(500, ErrorMessage = "Website URL cannot exceed 500 characters")]
        public string? Website { get; set; }

        [StringLength(10, ErrorMessage = "AmLaw ranking cannot exceed 10 characters")]
        public string AmlawRanking { get; set; } = "NR";
    }

    /// <summary>
    /// Response object for bulk import operations
    /// </summary>
    public class ImportResult
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<Lawyer> ImportedLawyers { get; set; } = new List<Lawyer>();
    }
}
