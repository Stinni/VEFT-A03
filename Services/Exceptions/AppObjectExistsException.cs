using System;

namespace A03.Services.Exceptions
{
    /// <summary>
    /// A custom made exception to throw when an object already exists in the database
    /// </summary>
    public class AppObjectExistsException : Exception
    {
        private readonly string _msg;

        /// <summary>
        /// A constructor that sets the message string
        /// </summary>
        public AppObjectExistsException()
        {
            _msg = "App Object Already Exists In The Database";
        }

        /// <summary>
        /// Returns the exception's message string
        /// </summary>
        public string GetMessage()
        {
            return _msg;
        }
    }
}
