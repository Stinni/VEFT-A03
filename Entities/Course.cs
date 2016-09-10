namespace A03.Entities
{
    /// <summary>
    /// An entity class for a Course record that gets stored in a database
    /// </summary>
    public class Course
    {
        /// <summary>
        /// Primary key - The course's Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The template Id, links the course to the CourseTemplates table
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// String for which semester the course is being taught
        /// Example: "20163" - the year and then 3 for the autumn semester
        /// </summary>
        public string Semester { get; set; }

        /// <summary>
        /// The starting date of this course
        /// Example: "01.01.2015" - String on the form dd.MM.yyyy
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// The last day that this course will be taught
        /// Example: "01.01.2015" - String on the form dd.MM.yyyy
        /// </summary>
        public string EndDate { get; set; }
    }
}
