using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace A03.Services.Exceptions
{
    public class CoursesAPIValidationException : Exception
    {
        private readonly List<ValidationResult> _listOfValidationResults;

        public CoursesAPIValidationException(List<ValidationResult> list)
        {
            _listOfValidationResults = list;
        }

        public List<ValidationResult> GetValidationResults()
        {
            return _listOfValidationResults;
        }
    }
}
