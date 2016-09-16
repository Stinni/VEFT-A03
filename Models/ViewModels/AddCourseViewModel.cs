using System.ComponentModel.DataAnnotations;

namespace A03.Models.ViewModels
{
    /// <summary>
    /// A ViewModel class for adding a new course to the database
    /// </summary>
    public class AddCourseViewModel
    {
        /// <summary>
        /// The course's template Id that links the course to the CourseTemplates table
        /// </summary>
        [Required]
        [RegularExpression("^([A-Z]\\-\\d{3}\\-[A-Z]{4})$")]
        // ReSharper disable once InconsistentNaming
        public string TemplateID { get; set; }

        /// <summary>
        /// The course's start date
        /// </summary>
        [Required]
        [RegularExpression("^\\d{4}\\-\\d{2}\\-\\d{2}[ |T]\\d{2}\\:\\d{2}\\:\\d{2}$")]
        public string StartDate { get; set; }

        /// <summary>
        /// The course's end date
        /// </summary>
        [Required]
        [RegularExpression("^\\d{4}\\-\\d{2}\\-\\d{2}[ |T]\\d{2}\\:\\d{2}\\:\\d{2}$")]
        public string EndDate { get; set; }

        /// <summary>
        /// String for which semester the course is being taught
        /// <example>"20163" - the year and then 3 for the autumn semester</example>
        /// </summary>
        [Required]
        [RegularExpression("^\\d{5}$")]
        public string Semester { get; set; }

        /// <summary>
        /// The maximum number of students that can be enrolled in this course
        /// </summary>
        [Range(0, int.MaxValue)]
        public int MaxStudents { get; set; }
    }
}
