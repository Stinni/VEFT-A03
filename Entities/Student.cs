namespace A03.Entities
{
    /// <summary>
    /// An entity class for a Student record that gets stored in a database
    /// </summary>
    public class Student
    {
        /// <summary>
        /// Primary key - The student's Id
        /// The SSN is the actual student's Id. This Id is only for the framework to
        /// know what the primary key is. (Workaround)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// SSN stands for social security number but this is actually a kennitala
        /// 10 digits as a string
        /// Example: "121219801239"
        /// </summary>
        public string SSN { get; set; }

        /// <summary>
        /// The Student's name
        /// </summary>
        public string Name { get; set; }
    }
}
