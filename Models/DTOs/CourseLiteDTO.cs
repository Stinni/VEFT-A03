namespace A03.Models
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
        /// Example: "Web services"
        /// </summary>
        public string Name { get; set; }

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
