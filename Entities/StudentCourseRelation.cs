namespace A03.Entities
{
    /// <summary>
    /// An entity class to link a student to a course that he's enrolled in
    /// </summary>
    public class StudentCourseRelation
    {
        /// <summary>
        /// The primary key, only for the framework
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Id of the course being linked to a student
        /// Foreign key linked to the CourseId column in the Courses table
        /// Forms a unique duo with StudentId
        /// </summary>
        public int CourseId { get; set; }

        /// <summary>
        /// The Id of the student being linked to a course
        /// Foreign key linked to the Id column in the Students table
        /// Forms a unique duo with CourseId
        /// </summary>
        public string StudentId { get; set; }

        public bool Deleted { get; set; }
    }
}
