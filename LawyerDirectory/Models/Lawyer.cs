using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LawyerDirectory.Models
{
    public class Lawyer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ProfilePictureUrl { get; set; }

        [Required]
        [StringLength(500)]
        public string AreaOfPractice { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, 100)]
        public int YearsOfExperience { get; set; }

        [Required]
        [StringLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Website { get; set; }

        [StringLength(10)]
        public string AmlawRanking { get; set; } = "NR";
    }
}
