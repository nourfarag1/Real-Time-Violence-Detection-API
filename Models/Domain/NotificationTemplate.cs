using System.ComponentModel.DataAnnotations;

namespace Vedect.Models.Domain;

public class NotificationTemplate
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string EventType { get; set; } // "violence_detected" or "warning_detected"

    [Required]
    [MaxLength(100)]
    public string Title { get; set; }

    [Required]
    [MaxLength(500)]
    public string Body { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;
} 