using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace A03.Services.Exceptions
{
    /// <summary>
    /// A custom made exception to throw when a validation fails in the
    /// CoursesAPIValidation class. It's not being used in this implementation.
    /// </summary>
    public class CoursesAPIValidationException : Exception
    {
        private readonly List<ValidationResult> _listOfValidationResults;

        /// <exclude />
        public CoursesAPIValidationException(List<ValidationResult> list)
        {
            _listOfValidationResults = list;
        }

        /// <exclude />
        public List<ValidationResult> GetValidationResults()
        {
            return _listOfValidationResults;
        }
    }
}
