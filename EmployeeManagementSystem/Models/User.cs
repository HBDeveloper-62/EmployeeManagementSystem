using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z.\s]+$", ErrorMessage = "Name must contain only letters and dot.")]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Invalid Email format.")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^03[0-9]{9}$", ErrorMessage = "Enter a valid Pakistani phone number (e.g., 03XXXXXXXXX).")]
        public string Contact { get; set; }


        [Required]
        [StringLength(50)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
            ErrorMessage = "Password must have uppercase, lowercase, digit, and special character.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmedPassword { get; set; }

        public string Role { get; set; } = "Admin";  // Default Admin

        public string? ImagePath { get; set; }       // Profile Image

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
