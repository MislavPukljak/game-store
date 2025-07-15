using System.ComponentModel.DataAnnotations;

namespace Business.DTO.Authorization;

public class UpdatePermissionDto
{
    [Required(ErrorMessage = " Email is required")]
    public string Email { get; set; }
}
