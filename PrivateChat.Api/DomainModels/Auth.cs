using System.ComponentModel.DataAnnotations;
using entityUser = PrivateChat.Api.Entity.Models;

namespace PrivateChat.Api.DomainModels
{
    public class UserForAuthenticationDto
    {
        [Required(ErrorMessage = "Email is required.")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }
    }

    public class AuthResponseDto
    {
        public bool IsAuthSuccessful { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Token { get; set; }

        public IEnumerable<Object> claims { get; set; }
    }
}
