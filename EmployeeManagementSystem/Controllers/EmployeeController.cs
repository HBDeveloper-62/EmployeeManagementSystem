using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Filters;
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementSystem.Controllers
    
{
    [AuthorizeEmployee]
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;
        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        // List all employees
        public IActionResult EmployeeInfo()
        {
            var employees = _context.Employees.ToList();  // non-null list
            return View(employees);
        }
        public IActionResult Employees()
        {
            var employees = _context.Employees.ToList();  // non-null list
            return View(employees);
        }


        public IActionResult Details(int id)
        {
            var emp = _context.Employees.FirstOrDefault(e => e.Id == id);
            if (emp == null) return NotFound();
            return View(emp);
        }

        // Add Employee (GET)
        [HttpGet]
        public IActionResult Add()
        {
            return View(new Employee());
        }

        // Add Employee (POST)
        [HttpPost]
        public async Task<IActionResult> Add(Employee emp, IFormFile imageFile, IFormFile resumeFile)
        {
            ModelState.Remove("ImageFile");
            ModelState.Remove("ResumeFile");
            ModelState.Remove("ImagePath");
            ModelState.Remove("ResumePath");

            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        Console.WriteLine($"❌ Field: {modelState.Key}, Error: {error.ErrorMessage}");
                    }
                }
                return View(emp);
            }

            if (imageFile != null)
            {
                string imgName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                string imgPath = Path.Combine("wwwroot/images", imgName);
                using (var stream = new FileStream(imgPath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                emp.ImagePath = "/images/" + imgName;
            }

            if (resumeFile != null)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(resumeFile.FileName);
                string filePath = Path.Combine("wwwroot/resumes", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await resumeFile.CopyToAsync(stream);
                }
                emp.ResumePath = "/resumes/" + fileName;
            }

            _context.Employees.Add(emp);
            await _context.SaveChangesAsync();
            return RedirectToAction("EmployeeInfo");
        }

        // Edit Employee
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var emp = _context.Employees.FirstOrDefault(e => e.Id == id);
            if (emp == null) return NotFound();
            return View(emp);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Employee emp, IFormFile imageFile, IFormFile resumeFile)
        {
            ModelState.Remove("resumeFile");
            ModelState.Remove("imageFile");

            if (!ModelState.IsValid)
                return View(emp);

            if (imageFile != null)
            {
                string imgName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                string imgPath = Path.Combine("wwwroot/images", imgName);
                using (var stream = new FileStream(imgPath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                emp.ImagePath = "/images/" + imgName;
            }

            if (resumeFile != null)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(resumeFile.FileName);
                string filePath = Path.Combine("wwwroot/resumes", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await resumeFile.CopyToAsync(stream);
                }
                emp.ResumePath = "/resumes/" + fileName;
            }

            _context.Employees.Update(emp);
            _context.SaveChanges();
            return RedirectToAction("EditSuccess");
        }

        public IActionResult EditSuccess() => View();

        // Delete
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var emp = _context.Employees.FirstOrDefault(e => e.Id == id);
            if (emp == null) return NotFound();
            return View(emp);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(Employee emp)
        {
            if (!ModelState.IsValid) return View(emp);

            _context.Employees.Remove(emp);
            _context.SaveChanges();
            return RedirectToAction("EmployeeInfo");
        }

        // Attendance List
        public IActionResult Attendance()
        {
            var attendanceList = _context.Attendances.Include(a => a.Employee)
                                                     .OrderByDescending(a => a.AttendanceDate)
                                                     .ToList();
            return View(attendanceList);
        }

        // Mark Attendance (GET)
        [HttpGet]
        public IActionResult Mark()
        {
            ViewBag.EmployeeList = new SelectList(_context.Employees, "Id", "FullName");
            return View(new Attendance()); // ⬅️ Yeh model bhej raha hai
        }


        // Mark Attendance (POST)
        [HttpPost]
        public IActionResult Mark(Attendance attendance)
        {
            ModelState.Remove("Employee");
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        Console.WriteLine($"❌ Field: {modelState.Key}, Error: {error.ErrorMessage}");
                    }
                }

                ViewBag.EmployeeList = new SelectList(_context.Employees, "Id", "FullName");
                return View(attendance);
            }

            _context.Attendances.Add(attendance);
            _context.SaveChanges();

            return RedirectToAction("Attendance");
        }
        [HttpGet]
        public IActionResult EditMark(int id)
        {
            var attendance = _context.Attendances
                .Include(a => a.Employee) // Optional: if you want Employee details too
                .FirstOrDefault(a => a.Id == id);

            if (attendance == null)
            {
                return NotFound(); // 👈 This is the error you're seeing
            }

            ViewBag.Employees = new SelectList(_context.Employees, "Id", "FullName", attendance.EmployeeId);
            return View(attendance);
        }



        [HttpPost]
        public IActionResult EditMark(Attendance mark)
        {
            ModelState.Remove("Employee");

            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        Console.WriteLine($"❌ Field: {modelState.Key}, Error: {error.ErrorMessage}");
                    }
                }

                ViewBag.Employees = new SelectList(_context.Employees, "Id", "FullName", mark.EmployeeId);
                return View(mark);
            }

            var existingAttendance = _context.Attendances.FirstOrDefault(m => m.Id == mark.Id);
            if (existingAttendance == null)
            {
                return NotFound();
            }

            existingAttendance.EmployeeId = mark.EmployeeId;
            existingAttendance.Status = mark.Status;
            existingAttendance.AttendanceDate = mark.AttendanceDate;

            _context.SaveChanges();

            return RedirectToAction("Attendance");
        }


        // Leave List
        public IActionResult LeaveRequests()
        {
            var leaves = _context.LeaveRequests.Include(l => l.Employee).ToList();
            return View(leaves);
        }

        // GET: AddLeave
        [HttpGet]
        public IActionResult AddLeave()
        {
            ViewBag.Employees = _context.Employees.ToList();
            return View(new LeaveRequest()); // send empty model
        }

        // POST: AddLeave
        [HttpPost]
        public IActionResult AddLeave(LeaveRequest leave)
        {
            ModelState.Remove("Employee");
            if (!ModelState.IsValid)
            {
               
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        Console.WriteLine($"❌ Field: {modelState.Key}, Error: {error.ErrorMessage}");
                    }
                }

                ViewBag.Employees = _context.Employees.ToList();
                return View(leave); 
            }

            _context.LeaveRequests.Add(leave);
            _context.SaveChanges();

            return RedirectToAction("LeaveRequests");
        }
        [HttpGet]
        public IActionResult EditLeave(int id)
        {
            var leave=_context.LeaveRequests.FirstOrDefault(l=>l.Id == id);
            if(leave==null)
            {
                return NotFound();
            }

            ViewBag.Employees= _context.Employees.ToList();
            return View(leave);
        }
        [HttpPost]
        public IActionResult EditLeave(LeaveRequest updatedLeave)
        {
           
            ModelState.Remove("Employee");

            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        Console.WriteLine($"❌ Field: {modelState.Key}, Error: {error.ErrorMessage}");
                    }
                }

                ViewBag.Employees = _context.Employees.ToList();
                return View(updatedLeave);
            }

            
            var existingLeave = _context.LeaveRequests.FirstOrDefault(l => l.Id == updatedLeave.Id);
            if (existingLeave == null)
            {
                return NotFound(); 
            }

            
            existingLeave.EmployeeId = updatedLeave.EmployeeId;
            existingLeave.StartDate = updatedLeave.StartDate;
            existingLeave.EndDate = updatedLeave.EndDate;
            existingLeave.Reason = updatedLeave.Reason;
            existingLeave.Status = updatedLeave.Status;

           
            _context.SaveChanges();

            return RedirectToAction("LeaveRequests");
        }



        // Salary List
        public IActionResult Salaries()
        {
            var salaries = _context.Salaries
                .Include(s => s.Employee) // ✅ Yeh line lazmi hai
                .ToList();

            return View(salaries);
        }


        [HttpGet]
        public IActionResult AddSalary()
        {
            ViewBag.Employees = _context.Employees.ToList();
            return View(new Salary());
        }


        [HttpPost]
        public IActionResult AddSalary(Salary salary)
        {
            ModelState.Remove("Employee");
            if (!ModelState.IsValid)
            {
                // Console par error log karna helpful hai debug ke liye
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        Console.WriteLine($"❌ Field: {modelState.Key}, Error: {error.ErrorMessage}");
                    }
                }

                ViewBag.Employees = _context.Employees.ToList();
                return View(salary); // show form again with errors
            }

            _context.Salaries.Add(salary);
            _context.SaveChanges();

            return RedirectToAction("Salaries");
        }
        [HttpGet]
        public IActionResult EditSalaries(int id)
        {
            var salary = _context.Salaries.FirstOrDefault(s => s.Id == id);
            if (salary == null)
            {
                return NotFound();
            }

            ViewBag.Employees = _context.Employees.ToList(); 
            return View(salary);
        }

        [HttpPost]
        public IActionResult EditSalaries(Salary salaries)
        {
            ModelState.Remove("Employee");

            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        Console.WriteLine($"❌ Field: {modelState.Key}, Error: {error.ErrorMessage}");
                    }
                }

                ViewBag.Employees = _context.Employees.ToList();
                return View(salaries); 
            }

            var existingSalary = _context.Salaries.FirstOrDefault(s => s.Id == salaries.Id); 
            if (existingSalary == null)
            {
                return NotFound();
            }

            existingSalary.EmployeeId = salaries.EmployeeId;
            existingSalary.BasicSalary = salaries.BasicSalary;
            existingSalary.Allowance = salaries.Allowance;
            existingSalary.Deductions = salaries.Deductions;
            existingSalary.SalaryMonth = salaries.SalaryMonth;

            _context.SaveChanges();

            return RedirectToAction("Salaries");
        }

    }
}
