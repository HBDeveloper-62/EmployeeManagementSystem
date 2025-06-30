using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace EmployeeManagementSystem.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; }

   
        [Required(ErrorMessage = "Position is required")]
        [StringLength(50, ErrorMessage = "Position cannot exceed 50 characters")]
        public string Position { get; set; }

        [Required(ErrorMessage = "Joining date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Joining Date")]
        public DateTime JoiningDate { get; set; }

        [Required(ErrorMessage = "CNIC is required")]
        [StringLength(15, ErrorMessage = "CNIC must be 15 characters")]
        [RegularExpression(@"^\d{5}-\d{7}-\d{1}$", ErrorMessage = "CNIC must be in format 12345-1234567-1")]
        [Display(Name = "CNIC")]
        public string CNIC { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(12, ErrorMessage = "Phone number must be 11 digits")]
        [RegularExpression(@"^03[0-9]{2}-?[0-9]{7}$", ErrorMessage = "Phone must be valid Pakistani format (0300-1234567)")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; }

        [Display(Name = "Profile Image")]
        public string ImagePath { get; set; }

        [Display(Name = "Resume")]
        public string ResumePath { get; set; }

        [NotMapped]
        [Display(Name = "Profile Image")]
        public IFormFile ImageFile { get; set; }

        [NotMapped]
        [Display(Name = "Resume File")]
        public IFormFile ResumeFile { get; set; }
    }
}