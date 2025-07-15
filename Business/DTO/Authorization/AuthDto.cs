using Newtonsoft.Json;

namespace Business.DTO.Authorization;

public class AuthDto
{
    [JsonProperty(PropertyName = "email")]
    public string Email { get; set; }

    [JsonProperty(PropertyName = "password")]
    public string Password { get; set; }
}
