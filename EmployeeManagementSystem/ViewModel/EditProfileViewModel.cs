using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.ViewModel
{
    public class EditProfileViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Contact { get; set; }

        public string Email { get; set; }

        public IFormFile? NewImage { get; set; }
    }
}
