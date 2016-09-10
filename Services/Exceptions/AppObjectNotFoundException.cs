using System;

namespace A03.Services.Exceptions
{
    /// <summary>
    /// A custom made exception to throw when an object doesn't exists in the database
    /// </summary>
    public class AppObjectNotFoundException : Exception
    {
        private readonly string _msg;

        /// <summary>
        /// A constructor that sets the message string
        /// </summary>
        public AppObjectNotFoundException()
        {
            _msg = "App Object Could Not Be Found";
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
