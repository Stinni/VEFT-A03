using System.ComponentModel.DataAnnotations;

namespace A03.Models.ViewModels
{
    /// <summary>
    /// A ViewModel class for adding a student to a course
    /// </summary>
    public class AddStudentToCourseViewModel
    {
        /// <summary>
        /// The student's SSN
        /// </summary>
        [Required]
        [RegularExpression("^\\d{10}$")]
        // ReSharper disable once InconsistentNaming
        public string SSN { get; set; }
    }
}
