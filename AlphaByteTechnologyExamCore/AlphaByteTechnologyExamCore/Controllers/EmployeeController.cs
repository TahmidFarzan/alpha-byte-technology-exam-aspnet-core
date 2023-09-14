using AlphaByteTechnologyExamCore.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace AlphaByteTechnologyExam.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly DBContext _dbContext = new DBContext();

        [HttpGet]
        public IActionResult Index()
        {
            List<Employee> employeeList = _dbContext.tEmployee.OrderBy(e => e.Id).ToList();
            return View(employeeList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.divisionList = _dbContext.tDivision.ToList();
            ViewBag.departmentList = _dbContext.tDepartment.ToList();
            return View();
        }

        [HttpGet]
        public IActionResult Details(long id)
        {

            Employee employee = _dbContext.tEmployee.FirstOrDefault(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpGet]
        public IActionResult Edit(long id)
        {
           
            Employee employee = _dbContext.tEmployee.FirstOrDefault(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }
            ViewBag.divisionList = _dbContext.tDivision.ToList();
            ViewBag.departmentList = _dbContext.tDepartment.Where(d => d.DivId == employee.DivId).ToList();

            return View(employee);
        }

        [HttpGet]
        public JsonResult GetDepartmentByDivision(long divID)
        {
            List<Department> departmentList = _dbContext.tDepartment.Where(e => e.DivId == divID).ToList();

            return Json(departmentList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(Employee formRecord) {
            List<string> extraFormValidation = ExtraFormValidation(null, formRecord);
            if (ModelState.IsValid && extraFormValidation.Count() == 0)
            {
                try
                {
                    int sameEmployeeIdCount = _dbContext.tEmployee.Count(e => e.Id == formRecord.Id);
                    if (!(sameEmployeeIdCount == 0))
                    {
                        ModelState.AddModelError("Id", "Same id exit.Id Must be unique.");
                    }
                    Employee newRecord = new Employee();
                    newRecord.IdNumber = formRecord.IdNumber;
                    newRecord.Name = formRecord.Name;
                    newRecord.Dob = formRecord.Dob;
                    newRecord.DeptId = formRecord.DeptId;
                    newRecord.DivId = formRecord.DivId;

                    if (formRecord.ResumeFile != null && formRecord.ResumeFile.Length > 0)
                    {
                        string fileUrl = null;
                       
                        IWebHostEnvironment _webHostEnvironment = HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment)) as IWebHostEnvironment;
                        string appDataFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "storage");
                        if (!Directory.Exists(appDataFolderPath))
                        {
                            Directory.CreateDirectory(appDataFolderPath);
                        }
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(formRecord.ResumeFile.FileName);
                        string filePath = Path.Combine(appDataFolderPath, uniqueFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            formRecord.ResumeFile.CopyTo(stream);
                        }
                        fileUrl = Url.Content("~/storage/" + uniqueFileName); ;
                        newRecord.FileParh = fileUrl;
                    }
                    _dbContext.tEmployee.Add(newRecord);
                    _dbContext.SaveChanges();
                    TempData["successMessage"] = "Record saved successfully";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the employee data."+ ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError("", "Extra validation fail.");
                foreach (var row in extraFormValidation)
                {
                    ModelState.AddModelError("", row);
                }
            }

            ViewBag.DivisionList = _dbContext.tDivision.ToList();
            ViewBag.DepartmentList = _dbContext.tDepartment.ToList();
            return View("Create", formRecord);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Update(long id, Employee formRecord)
        {
            List<string> extraFormValidation = ExtraFormValidation(id, formRecord);
            if (ModelState.IsValid && extraFormValidation.Count() == 0)
            {
                try
                {
                    Employee employee = _dbContext.tEmployee.FirstOrDefault(e => e.Id == id);

                    if (employee == null)
                    {
                        return NotFound();
                    }
                    employee.IdNumber = formRecord.IdNumber;
                    employee.Name = formRecord.Name;
                    employee.Dob = formRecord.Dob;
                    employee.DeptId = formRecord.DeptId;
                    employee.DivId = formRecord.DivId;

                    if ( (formRecord.ResumeFile != null) && (formRecord.ResumeFile.Length > 0) )
                    {
                        IWebHostEnvironment _webHostEnvironment = HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment)) as IWebHostEnvironment;

                        string filePhysicalPath = Path.Combine(_webHostEnvironment.WebRootPath, "storage", Path.GetFileName(employee.FileParh));;
                        if (System.IO.File.Exists(filePhysicalPath))
                        {
                            System.IO.File.Delete(filePhysicalPath);

                        }

                        string fileUrl = null;
                        string appDataFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "storage");
                        if (!Directory.Exists(appDataFolderPath))
                        {
                            Directory.CreateDirectory(appDataFolderPath);
                        }
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(formRecord.ResumeFile.FileName);
                        string filePath = Path.Combine(appDataFolderPath, uniqueFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            formRecord.ResumeFile.CopyTo(stream);
                        }
                        fileUrl = Url.Content("~/storage/" + uniqueFileName); ;
                        employee.FileParh = fileUrl;
                    }

                    _dbContext.Entry(employee).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    TempData["successMessage"] = "Record saved successfully";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the employee data." + ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError("", "Extra validation fail.");
                foreach (var row in extraFormValidation)
                {
                    ModelState.AddModelError("", row);
                }
            }

            ViewBag.DivisionList = _dbContext.tDivision.ToList();
            ViewBag.DepartmentList = _dbContext.tDepartment.ToList();
            return View("Edit", formRecord);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(long id)
        {
            Employee employee = _dbContext.tEmployee.FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                return NotFound();
            }
            IWebHostEnvironment _webHostEnvironment = HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment)) as IWebHostEnvironment;
            string filePhysicalPath = Path.Combine(_webHostEnvironment.WebRootPath, "storage", Path.GetFileName(employee.FileParh)); ;
            if (System.IO.File.Exists(filePhysicalPath))
            {
                System.IO.File.Delete(filePhysicalPath);

            }

            _dbContext.tEmployee.Remove(employee);
            _dbContext.SaveChanges();
            TempData["successMessage"] = "Record saved deleted.";
            return RedirectToAction("Index");
        }


        private List<string> ExtraFormValidation(long? id,Employee employee)
        {
            List<string> errors = new List<string>();

            List<Employee> filterEmployees = _dbContext.tEmployee.Where(e => e.IdNumber == employee.IdNumber).ToList();

            if (!(id == null))
            {
                filterEmployees = filterEmployees.Where(e => e.Id != id).ToList();
            }

            filterEmployees = filterEmployees.ToList();

            if (!(filterEmployees.Count() == 0))
            {
                errors.Add("Id Number must be unique.");
            }

            return errors;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}