using System;
using System.ComponentModel.DataAnnotations;

namespace A03.Models.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class AddCourseViewModel
    {
        /// <summary>
        /// TODO: FILL OUT
        /// </summary>
        [Required]
        [RegularExpression("^([A-Z]\\-\\d{3}\\-[A-Z]{4})$")]
        public string TemplateID { get; set; }

        /// <summary>
        /// TODO: FILL OUT
        /// </summary>
        [Required]
        [RegularExpression("^\\d{4}\\-\\d{2}\\-\\d{2}[ |T]\\d{2}\\:\\d{2}\\:\\d{2}$")]
        public string StartDate { get; set; }

        /// <summary>
        /// TODO: FILL OUT
        /// </summary>
        [Required]
        [RegularExpression("^\\d{4}\\-\\d{2}\\-\\d{2}[ |T]\\d{2}\\:\\d{2}\\:\\d{2}$")]
        public string EndDate { get; set; }

        /// <summary>
        /// TODO: FILL OUT
        /// </summary>
        [Required]
        [RegularExpression("^\\d{5}$")]
        public string Semester { get; set; }

        /// <summary>
        /// TODO: FILL OUT
        /// </summary>
        [Range(0, int.MaxValue)]
        public int MaxStudents { get; set; }
    }
}
