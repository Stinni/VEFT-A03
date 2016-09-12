using System;

namespace A03.Services.Exceptions
{
    /// <summary>
    /// A custom made exception to throw when an object has reached it's capacity
    /// </summary>
    public class MaxNrOfStudentsReachedException : Exception
    {
        private readonly string _msg;

        /// <summary>
        /// A constructor that sets the message string
        /// </summary>
        public MaxNrOfStudentsReachedException()
        {
            _msg = "Max number of students for this course has been reached!";
        }

        /// <summary>
        /// Returns the message string
        /// </summary>
        public string GetMessage()
        {
            return _msg;
        }
    }
}
