using System.ComponentModel.DataAnnotations;

namespace Vedect.Models.DTOs;
 
public class RegisterDeviceRequest
{
    [Required]
    public string FcmToken { get; set; }
} 