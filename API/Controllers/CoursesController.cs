using Microsoft.AspNetCore.Mvc;

using A03.Models.ViewModels;
using A03.Services;
using A03.Services.Exceptions;

namespace A03.API.Controllers
{
    /// <summary>
    /// The controller for "api/courses" route
    /// Used for anything related to the Courses.db database
    /// Getting data about courses, students in courses, adding students to courses
    /// and even deleting courses
    /// </summary>
    [Route("api/courses")]
    public class CoursesController : Controller
    {
        private readonly ICoursesService _service;

        /// <summary>
        /// The CoursesController Constructor
        /// </summary>
        /// <param name="service">The service being used for database access</param>
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
        /// PUT method for the "api/courses/{id}" route
        /// Updates the StartDate and EndDate for the course with 'id' as it's Id
        /// </summary>
        /// <param name="id">The Id of the course being updated</param>
        /// <param name="model">Model with two attributes, StartDate and EndDate as strings</param>
        [HttpPut("{id}")]
        public IActionResult UpdateCourseDates(int id, [FromBody]UpdateCourseViewModel model)
        {
            if (!ModelState.IsValid) return new BadRequestResult();

            try
            {
                _service.UpdateCourseDates(id, model.StartDate, model.EndDate);
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
        /// <param name="model">Model with one attribute, the student's SSN named StudentSSN</param>
        [HttpPost]
        [Route("{id}/students", Name = "AddStudentToACourse")]
        public IActionResult AddStudentToACourse(int id, [FromBody]AddStudentToCourseViewModel model)
        {
            if (!ModelState.IsValid) return new BadRequestResult();

            try
            {
                _service.AddStudentToCourse(id, model.StudentSSN);
                return new NoContentResult();
            }
            catch (AppObjectNotFoundException) { return new NotFoundResult(); }
            catch (AppObjectExistsException) { return new BadRequestResult(); }
        }
    }
}
