using System.ComponentModel.DataAnnotations;

namespace FilmLibrary.Models.Models
{
    public class CredentialsModel
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}