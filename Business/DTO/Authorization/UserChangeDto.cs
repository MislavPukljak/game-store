using Newtonsoft.Json;

namespace Business.DTO.Authorization;

public class UserChangeDto
{
    [JsonProperty(PropertyName = "email")]
    public string Email { get; set; }

    [JsonProperty(PropertyName = "firstName")]
    public string FirstName { get; set; }

    [JsonProperty(PropertyName = "lastName")]
    public string LastName { get; set; }

    [JsonProperty(PropertyName = "password")]
    public string Password { get; set; }

    [JsonProperty(PropertyName = "confirmPassword")]
    public string ConfirmPassword { get; set; }
}
