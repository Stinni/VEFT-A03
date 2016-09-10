using System;
using System.ComponentModel.DataAnnotations;

namespace A03.Models.ViewModels
{
    public class AddCourseViewModel
    {
        public string TemplateID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Semester { get; set; }
        public int MaxStudents { get; set; }
    }
}
