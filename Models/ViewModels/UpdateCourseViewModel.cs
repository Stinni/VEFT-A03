using System.ComponentModel.DataAnnotations;

namespace A03.Models.ViewModels
{
    /// <summary>
    /// Used to store info for updating a course's start date, end date and the maximum
    /// number of students that can be enrolled in said course.
    /// </summary>
    public class UpdateCourseViewModel
    {
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
        /// The maximum number of students that can be enrolled in this course
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int MaxStudents { get; set; }
    }
}
