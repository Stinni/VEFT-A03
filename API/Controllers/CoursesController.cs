using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

using A03.Models.ViewModels;
using A03.Services;
using A03.Services.Exceptions;

namespace A03.API.Controllers
{
    /// <summary>
    /// The controller for "api/courses" route
    /// Used for anything related to the Courses.db database, f.ex. getting data about
    /// courses, students in courses, adding students to courses and even deleting courses
    /// </summary>
    [Route("api/courses")]
    public class CoursesController : Controller
    {
        private readonly ICoursesService _service;

        /// <exclude />
        public CoursesController(ICoursesService service)
        {
            _service = service;
        }

        /// <summary>
        /// GET method for the "api/courses/" and "api/courses?semester={semester}" routes
        /// Sends a list of all courses being taught at a certain semester
        /// </summary>
        /// <param name="semester">Optional search paramater, if empty, 20163 is used</param>
        [HttpGet]
        public IActionResult GetCoursesOnSemester(string semester)
        {
            try
            {
                var list = _service.GetCoursesBySemester(semester);
                return new ObjectResult(list);
            }
            catch(AppObjectNotFoundException) { return new NotFoundResult(); }
        }

        /// <summary>
        /// GET method for the "api/courses/{id}" route
        /// Sends detailed info about a course with 'id' as it's Id
        /// </summary>
        /// <param name="id">The Id of the course being enquired about</param>
        [HttpGet]
        [Route("{id}", Name = "GetCourseById")]
        public IActionResult GetCourseById(int id)
        {
            try
            {
                var course = _service.GetCourseById(id);
                return new OkObjectResult(course);
            }
            catch (AppObjectNotFoundException) { return new NotFoundResult(); }
        }

        /// <summary>
        /// Post method for the "api/courses/" route
        /// Adds a new course sent as object in the message body to the database 
        /// </summary>
        /// <param name="model">AddCourseViewModel entity with all required attributes</param>
        [HttpPost]
        public IActionResult AddNewCourse([FromBody]AddCourseViewModel model)
        {
            if (model == null || !ModelState.IsValid) return BadRequest(ModelState);

            var course = _service.AddNewCourse(model);
            var location = Url.Link("GetCourseById", new {id = course.Id});
            return new CreatedResult(location, course);
        }

        /// <summary>
        /// PUT method for the "api/courses/{id}" route
        /// Updates the StartDate, EndDate and MaxStudents for the course with 'id' as it's Id
        /// </summary>
        /// <param name="id">The Id of the course being updated</param>
        /// <param name="model">Model with three attributes, StartDate, EndDate and MaxStudents</param>
        [HttpPut("{id}")]
        public IActionResult UpdateCourseInfo(int id, [FromBody]UpdateCourseViewModel model)
        {
            if (model == null || !ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                _service.UpdateCourseInfo(id, model);
                return new NoContentResult();
            }
            catch(AppObjectNotFoundException) { return new NotFoundResult(); }
        }

        /// <summary>
        /// DELETE method for the "api/courses/{id}" route
        /// Deletes all connections/enrollments of students to a course with
        /// 'id' as it's Id and then deletes the course itself
        /// </summary>
        /// <param name="id">The Id of the course being deleted</param>
        [HttpDelete("{id}")]
        public IActionResult DeleteCourse(int id)
        {
            try
            {
                _service.DeleteCourse(id);
                return new NoContentResult();
            }
            catch (AppObjectNotFoundException) { return new NotFoundResult(); }
        }

        /// <summary>
        /// GET method for the "/api/courses/{id}/students" route
        /// Sends a list of all students in a course with 'id' as it's Id
        /// </summary>
        /// <param name="id">The Id of the course being enquired about</param>
        [HttpGet]
        [Route("{id}/students", Name = "GetAllStudentsInCourse")]
        public IActionResult GetAllStudentsInCourse(int id)
        {
            try
            {
                var students = _service.GetAllStudentsInCourse(id);
                return new OkObjectResult(students);
            }
            catch (AppObjectNotFoundException) { return new NotFoundResult(); }
        }

        /// <summary>
        /// POST method for the "/api/courses/{id}/students" route
        /// Checks if the Student's SSN is valid and adds that student to
        /// a course with 'id' as it's Id
        /// </summary>
        /// <param name="id">The Id of the course that the student's enrolled in</param>
        /// <param name="model">Model with one attribute, the student's SSN</param>
        [HttpPost]
        [Route("{id}/students", Name = "AddStudentToCourse")]
        public IActionResult AddStudentToCourse(int id, [FromBody]AddStudentToCourseViewModel model)
        {
            if (model == null || !ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var student = _service.AddStudentToCourse(id, model);
                var location = Url.Link("GetAllStudentsInCourse", new { id });
                return new CreatedResult(location, student);
            }
            catch (AppObjectNotFoundException) { return new NotFoundResult(); }
            catch (AppObjectExistsException) { return new StatusCodeResult(412); }
            catch (MaxNrOfStudentsReachedException) { return new StatusCodeResult(412); }
        }

        /// <summary>
        /// DELETE method for the "/api/courses/{id}/students/{ssn}" route
        /// If a student with 'ssn' as his/her SSN is enrolled in a course with
        /// 'id' as it's Id, that student's removed from the course by labeling
        /// that record as deleted. Removes a student from a course
        /// The first student, if any, on a waitinglist for the same course is
        /// enrolled to the course.
        /// </summary>
        /// <param name="id">The course's Id</param>
        /// <param name="ssn">The student's SSN</param>
        [HttpDelete]
        [Route("{id}/students/{ssn}", Name = "RemoveStudentFromCourse")]
        public IActionResult RemoveStudentFromCourse(int id, string ssn)
        {
            var regex = new Regex("^\\d{10}$");
            if (!regex.IsMatch(ssn)) return new BadRequestResult();

            try
            {
                _service.RemoveStudentFromCourse(id, ssn);
                return new NoContentResult();
            }
            catch (AppObjectNotFoundException) { return new NotFoundResult(); }
            catch (AppObjectExistsException) { return new StatusCodeResult(412); }
        }

        /// <summary>
        /// GET method for the "/api/courses/{id}/waitinglist" route
        /// Sends a list of all students on a waitinglist for a course with
        /// 'id' as it's Id
        /// </summary>
        /// <param name="id">The Id of the course being enquired about</param>
        [HttpGet]
        [Route("{id}/waitinglist", Name = "GetWaitinglistForCourse")]
        public IActionResult GetWaitinglistForCourse(int id)
        {
            try
            {
                var students = _service.GetWaitinglistForCourse(id);
                return new OkObjectResult(students);
            }
            catch (AppObjectNotFoundException) { return new NotFoundResult(); }
        }

        /// <summary>
        /// POST method for the "/api/courses/{id}/waitinglist" route
        /// Checks if the Student's SSN is valid and adds that student to
        /// a waitinglist for a course with 'id' as it's Id
        /// </summary>
        /// <param name="id">The Id of the course</param>
        /// <param name="model">Model with one attribute, the student's SSN named StudentSSN</param>
        [HttpPost]
        [Route("{id}/waitinglist", Name = "AddStudentToWaitinglist")]
        public IActionResult AddStudentToWaitinglist(int id, [FromBody]AddStudentToCourseViewModel model)
        {
            if (model == null || !ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var student = _service.AddStudentToWaitinglist(id, model);
                return new OkObjectResult(student);
            }
            catch (AppObjectNotFoundException) { return new NotFoundResult(); }
            catch (AppObjectExistsException) { return new StatusCodeResult(412); }
        }
    }
}
