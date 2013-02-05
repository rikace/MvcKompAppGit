using Hik.JTable.Models;
using Hik.JTable.Repositories;
using Hik.JTable.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace MvcKomp3.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    public class JTableController : RepositoryBasedController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PagingAndSorting()
        {
            return View();
        }

        public ActionResult SelectingRows()
        {
            return View();
        }

        public ActionResult MasterChild()
        {
            return View();
        }

        public ActionResult UsingWithValidationEngine1()
        {
            return View();
        }

        public ActionResult UsingWithValidationEngine2()
        {
            return View();
        }

        public ActionResult Filtering()
        {
            var cities = _repository.CityRepository.GetAllCities().Select(city => new SelectListItem { Text = city.CityName, Value = city.CityId.ToString() }).ToList();
            cities.Insert(0, new SelectListItem { Selected = true, Text = "All cities", Value = "0" });

            ViewBag.Cities = cities;
            return View();
        }

        public ActionResult ColumnResizing()
        {
            return View();
        }

        public ActionResult ColumnHideShow()
        {
            return View();
        }
    }

    public abstract class RepositoryBasedController : Controller
    {
        #region Private/protected fields

        protected readonly IRepositoryContainer _repository;

        #endregion

        #region Constructor

        protected RepositoryBasedController(RepositorySize size = RepositorySize.Medium, string repositoryKey = "common")
        {
            _repository = RepositorySesssion.GetRepository(size, repositoryKey);
        }

        #endregion

        #region Student actions

        [HttpPost]
        public JsonResult StudentList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                Thread.Sleep(200);
                var studentCount = _repository.StudentRepository.GetStudentCount();
                var students = _repository.StudentRepository.GetStudents(jtStartIndex, jtPageSize, jtSorting);
                return Json(new { Result = "OK", Records = students, TotalRecordCount = studentCount });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult StudentListByFiter(string name = "", int cityId = 0, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var studentCount = _repository.StudentRepository.GetStudentCountByFilter(name, cityId);
                var students = _repository.StudentRepository.GetStudentsByFilter(name, cityId, jtStartIndex, jtPageSize, jtSorting);
                return Json(new { Result = "OK", Records = students, TotalRecordCount = studentCount });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CreateStudent(Student student)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                var addedStudent = _repository.StudentRepository.AddStudent(student);
                return Json(new { Result = "OK", Record = addedStudent });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateStudent(Student student)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                _repository.StudentRepository.UpdateStudent(student);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteStudent(int studentId)
        {
            try
            {
                Thread.Sleep(50);
                _repository.StudentRepository.DeleteStudent(studentId);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        #endregion

        #region City actions

        [HttpPost]
        public JsonResult GetCityOptions()
        {
            try
            {
                var cities = _repository.CityRepository.GetAllCities().Select(c => new { DisplayText = c.CityName, Value = c.CityId });
                return Json(new { Result = "OK", Options = cities });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        #endregion

        #region Phone actions

        [HttpPost]
        public JsonResult PhoneList(int studentId)
        {
            try
            {
                Thread.Sleep(200);
                var phones = _repository.PhoneRepository.GetPhonesOfStudent(studentId);
                return Json(new { Result = "OK", Records = phones });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeletePhone(int phoneId)
        {
            try
            {
                Thread.Sleep(50);
                _repository.PhoneRepository.DeletePhone(phoneId);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdatePhone(Phone phone)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                _repository.PhoneRepository.UpdatePhone(phone);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CreatePhone(Phone phone)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                var addedPhone = _repository.PhoneRepository.AddPhone(phone);
                return Json(new { Result = "OK", Record = addedPhone });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        #endregion

        #region Exam actions

        [HttpPost]
        public JsonResult ExamList(int studentId)
        {
            try
            {
                Thread.Sleep(200);
                var exams = _repository.ExamRepository.GetExamsOfStudent(studentId).OrderBy(e => e.ExamDate).ToList();
                return Json(new { Result = "OK", Records = exams });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteExam(int studentExamId)
        {
            try
            {
                Thread.Sleep(50);
                _repository.ExamRepository.DeleteExam(studentExamId);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateExam(StudentExam exam)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                _repository.ExamRepository.UpdateExam(exam);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CreateExam(StudentExam exam)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }

                var addedExam = _repository.ExamRepository.AddExam(exam);
                return Json(new { Result = "OK", Record = addedExam });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        #endregion

        #region Person actions

        [HttpPost]
        public JsonResult PersonList()
        {
            try
            {
                var persons = _repository.PersonRepository.GetAllPeople();
                return Json(new { Result = "OK", Records = persons });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CreatePerson(Person person)
        {
            try
            {
                var addedPerson = _repository.PersonRepository.AddPerson(person);
                return Json(new { Result = "OK", Record = addedPerson });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdatePerson(Person person)
        {
            try
            {
                _repository.PersonRepository.UpdatePerson(person);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeletePerson(int personId)
        {
            try
            {
                _repository.PersonRepository.DeletePerson(personId);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        #endregion
    }
}
