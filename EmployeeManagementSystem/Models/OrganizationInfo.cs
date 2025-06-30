using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
namespace EmployeeManagementSystem.Models
{
    public class OrganizationInfo
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Organization name is required")]
        public string OrganizationName { get; set; }

        public string Address { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid Phone")]
        public string Phone { get; set; }

        public string? LogoPath { get; set; }  // e.g. /logos/mylogo.png
    }
}
