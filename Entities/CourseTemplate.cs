namespace A03.Entities
{
    /// <summary>
    /// An entity class to store info about a course template
    /// A course will link it's CourseId to a CourseId in this class
    /// and through that it's actual name
    /// </summary>
    public class CourseTemplate
    {
        /// <summary>
        /// Primary key - The CourseTemplate's Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The CourseTemplate's CourseId is a unique property
        /// Links to the CourseId in the Courses table
        /// <example>"T-514-VEFT"</example>
        /// </summary>
        public string CourseId { get; set; }

        /// <summary>
        /// The name for this CourseTemplate's Course
        /// <example>"Web services"</example>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The number of credits for this course
        /// </summary>
        public int Credits { get; set; }
    }
}
