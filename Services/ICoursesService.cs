using System.Collections.Generic;
using A03.Models.DTOs;
using A03.Models.ViewModels;

namespace A03.Services
{
    /// <summary>
    /// Comments/Documentation in the CoursesService class
    /// </summary>
    public interface ICoursesService
    {
        /// <exclude />
        List<CourseLiteDTO> GetCoursesBySemester(string semester);

        /// <exclude />
        CourseLiteDTO GetCourseById(int cId);
        
        /// <exclude />
        CourseLiteDTO AddNewCourse(AddCourseViewModel model);

        /// <exclude />
        void UpdateCourseInfo(int cId, UpdateCourseViewModel model);

        /// <exclude />
        void DeleteCourse(int cId);

        /// <exclude />
        List<StudentLiteDTO> GetAllStudentsInCourse(int cId);

        /// <exclude />
        StudentLiteDTO AddStudentToCourse(int cId, AddStudentToCourseViewModel model);

        /// <exclude />
        void RemoveStudentFromCourse(int cId, string ssn);

        /// <exclude />
        List<StudentLiteDTO> GetWaitinglistForCourse(int cId);

        /// <exclude />
        StudentLiteDTO AddStudentToWaitinglist(int cId, AddStudentToCourseViewModel model);
    }
}
