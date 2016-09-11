using System;
using System.ComponentModel.DataAnnotations;

namespace A03.Models.ViewModels
{
    /// <summary>
    /// Used to store info for updating a course's start and end dates
    /// 
    /// Note on validation:
    /// Required and other types of ModelBinding didn't work, some problems with dependencies
    /// and/or frameworks so I ended up scrapping that and validating input myself :)
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
        /// TODO: FILL OUT
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int MaxStudents { get; set; }
    }
}
