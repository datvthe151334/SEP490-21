using System.ComponentModel.DataAnnotations;

namespace RecruitXpress_BE.DTO
{
    public class SignUpModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
