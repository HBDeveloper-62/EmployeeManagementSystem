using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementSystem.Models
{
    public class Salary
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }

        [Required]
        public decimal BasicSalary { get; set; }

        public decimal Allowance { get; set; }

        public decimal Deductions { get; set; }

        [Required]
        public DateTime SalaryMonth { get; set; }

        public decimal NetSalary => BasicSalary + Allowance - Deductions;
    }
}
