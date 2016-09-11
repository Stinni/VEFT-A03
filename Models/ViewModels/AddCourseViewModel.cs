using System;
using System.ComponentModel.DataAnnotations;

namespace A03.Models.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class AddCourseViewModel
    {
        [RegularExpression("^([A-Z]\\-\\d{3}\\-[A-Z]{4})$")]
        [Required]
        public string TemplateID { get; set; }

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime StartDate { get; set; }

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime EndDate { get; set; }

        [RegularExpression("^\\d{5}$")]
        [Required]
        public string Semester { get; set; }

        [Required]
        public int MaxStudents { get; set; }
    }
}
