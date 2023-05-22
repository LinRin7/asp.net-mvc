using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class Member
    {
        public int Id { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 1)]
        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }


        public string Role { get; set; }

        public Member()
        {

        }
        public Member(int id, string name, string email, string password)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
        }
    }
}
