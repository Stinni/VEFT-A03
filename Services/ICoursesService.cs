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
        /// <exclude />
        List<CourseLiteDTO> GetCoursesBySemester(string semester);

        /// <exclude />
        CourseLiteDTO GetCourseById(int id);
        
        /// <exclude />
        CourseLiteDTO AddNewCourse(AddCourseViewModel model);

        /// <exclude />
        void UpdateCourseInfo(int id, UpdateCourseViewModel model);

        /// <exclude />
        void DeleteCourse(int id);

        /// <exclude />
        List<StudentLiteDTO> GetAllStudentsInCourse(int id);

        /// <exclude />
        void AddStudentToCourse(int cId, string sId);

        /// <exclude />
        void RemoveStudentFromCourse(int cId, string sId);

        /// <exclude />
        List<StudentLiteDTO> GetWaitinglistForCourse(int id);

        /// <exclude />
        void AddStudentToWaitinglist(int cId, string sId);
    }
}
