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
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The course's end date
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// TODO: FILL OUT
        /// </summary>
        public int MaxStudents { get; set; }
    }
}
