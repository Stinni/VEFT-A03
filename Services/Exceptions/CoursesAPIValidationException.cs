using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace A03.Services.Exceptions
{
    /// <exclude />
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
