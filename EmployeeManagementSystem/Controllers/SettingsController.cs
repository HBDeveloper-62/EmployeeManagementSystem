using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
    public class SettingsController : Controller
    {
        private readonly AppDbContext _context;

        public SettingsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OrganizationInfo()
        {
            var org = _context.organizationInfos.FirstOrDefault();

            if (org == null)
            {
                // 🧠 If info not added, go to Add page
                return RedirectToAction("AddOrganizationInfo");
            }

            return View(org);
        }

        [HttpGet]
        public IActionResult AddOrganizationInfo()
        {
            return View(new OrganizationInfo());
        }

        [HttpPost]
        public async Task<IActionResult> AddOrganizationInfo(OrganizationInfo orgInfo, IFormFile logoFile)
        {
            ModelState.Remove("LogoPath ");
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        Console.WriteLine($"❌ Field: {modelState.Key}, Error: {error.ErrorMessage}");
                    }
                }
                return View(orgInfo);
            }

            if (logoFile != null && logoFile.Length > 0)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(logoFile.FileName);
                string path = Path.Combine("wwwroot/logos", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await logoFile.CopyToAsync(stream);
                }

                orgInfo.LogoPath = "/logos/" + fileName;
            }

            _context.organizationInfos.Add(orgInfo);
            await _context.SaveChangesAsync();

            return RedirectToAction("OrganizationInfo");
        }


        [HttpGet]
        public IActionResult EditOrganizationInfo()
        {
            var org = _context.organizationInfos.FirstOrDefault();
            return View(org ?? new OrganizationInfo());
        }

        [HttpPost]
        public async Task<IActionResult> EditOrganizationInfo(OrganizationInfo orgInfo, IFormFile logoFile)
        {
            if (!ModelState.IsValid)
            {
                // log errors
                return View(orgInfo);
            }

            if (logoFile != null && logoFile.Length > 0)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(logoFile.FileName);
                string path = Path.Combine("wwwroot/logos", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await logoFile.CopyToAsync(stream);
                }

                orgInfo.LogoPath = "/logos/" + fileName;
            }

            var existing = _context.organizationInfos.FirstOrDefault();
            if (existing == null)
            {
                _context.organizationInfos.Add(orgInfo);
            }
            else
            {
                existing.OrganizationName = orgInfo.OrganizationName;
                existing.Email = orgInfo.Email;
                existing.Phone = orgInfo.Phone;
                existing.Address = orgInfo.Address;
                if (!string.IsNullOrEmpty(orgInfo.LogoPath))
                    existing.LogoPath = orgInfo.LogoPath;

                _context.organizationInfos.Update(existing);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("OrganizationInfo");
        }
    }
}
