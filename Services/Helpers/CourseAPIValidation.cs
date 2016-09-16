using A03.Services.Exceptions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace A03.Services.Helpers
{
    /// <summary>
    /// A custom Validation class to validate data sent from the API to the service.
    /// It doesn't work so it's not being used in this implemention.
    /// </summary>
    public class CourseAPIValidation
    {
        /// <exclude />
        public static void Validate<T>(T model)
        {
            var results = new List<ValidationResult>();
            if (model == null)
            {
                // Add custom error code which is used to retrieve the error message in the correct lang in the filter at the API level
                results.Add(new ValidationResult("Model Equals Null"));
                throw new CoursesAPIValidationException(results);
            }

            var context = new ValidationContext(model, null, null);

            if (!Validator.TryValidateObject(model, context, results))
            {
                // No need to inject results in exception. 
                throw new CoursesAPIValidationException(results);
            }
        }
    }
}