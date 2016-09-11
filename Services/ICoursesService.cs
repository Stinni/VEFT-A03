using System;
using System.Collections.Generic;
using A03.Models;
using A03.Models.ViewModels;

namespace A03.Services
{
    /// <summary>
    /// Comments/Documentation in the CoursesService class
    /// </summary>
    public interface ICoursesService
    {
        List<CourseLiteDTO> GetCoursesBySemester(string semester);

        CourseLiteDTO GetCourseById(int id);

        CourseLiteDTO AddCourse(AddCourseViewModel model);

        void UpdateCourseDates(int id, DateTime sDate, DateTime eDate);

        void DeleteCourse(int id);

        List<StudentLiteDTO> GetAllStudentsInCourse(int id);

        void AddStudentToCourse(int cId, string sId);

        void RemoveStudentFromCourse(int cId, string sId);
    }
}
