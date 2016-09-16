using System;

namespace A03.Models.DTOs
{
    /// <summary>
    /// A DTO class for sending info about a course to the api
    /// </summary>
    public class CourseLiteDTO
    {
        /// <summary>
        /// The course's Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name for this course
        /// <example>"Web services"</example>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// String for which semester the course is being taught
        /// <example>"20163" - the year and then 3 for the autumn semester</example>
        /// </summary>
        public string Semester { get; set; }

        /// <summary>
        /// The starting date of this course
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The last day that this course will be taught
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// The number of credits for this course
        /// </summary>
        public int Credits {get; set;}

        /// <summary>
        /// The maximum number of students that can be enrolled in this course
        /// </summary>
        public int MaxStudents { get; set; }
    }
}
