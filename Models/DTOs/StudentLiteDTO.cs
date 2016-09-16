namespace A03.Models.DTOs
{
    /// <summary>
    /// A DTO class for sending info about a student to the api
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class StudentLiteDTO
    {
        /// <summary>
        /// The student's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The student's SSN
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string SSN { get; set; }
    }
}
