using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;

namespace Aisger.Utils
{
    public class DbValidationMessageParser
    {
        public static string GetErrorMessage(DbEntityValidationException validationException)
        {
            StringBuilder messageBuilder = new StringBuilder("Validation Errors").AppendLine();

            var messages = from validationResult in validationException.EntityValidationErrors
                           select GetErrorMessage(validationResult);

            messages.ToList().ForEach(m => messageBuilder.AppendLine(m));

            return messageBuilder.ToString();
        }

        public static string GetErrorMessage(IEnumerable<DbEntityValidationResult> validationResults)
        {
            var errorMessagBuilder = new StringBuilder();

            validationResults.ToList().ForEach(result => errorMessagBuilder.AppendLine(GetErrorMessage(result)));

            return errorMessagBuilder.ToString();
        }

        public static string GetErrorMessage(DbEntityValidationResult validationResult)
        {
            return GetErrorMessage(validationResult.ValidationErrors);
        }

        public static string GetErrorMessage2(IEnumerable<DbValidationError> validationErrors)
        {
            var errorMessageBuilder = new StringBuilder();

            List<string> errorMessages = (from validationError in validationErrors
                                          select string.Format(
                                              "Property: {0}, Error Message: {1}",
                                              validationError.PropertyName,
                                              validationError.ErrorMessage)).ToList<string>();

            errorMessages.ToList().ForEach(m => errorMessageBuilder.AppendLine(m));

            return errorMessageBuilder.ToString();
        }

        public static string GetErrorMessage(IEnumerable<DbValidationError> validationErrors)
        {
            var errorMessageBuilder = new StringBuilder();

            List<string> errorMessages = (
                                            from validationError in validationErrors
                                            select string.Format("{0}!", validationError.ErrorMessage)
                                          ).ToList<string>();

            errorMessages.ToList().ForEach(m => errorMessageBuilder.AppendLine(m));

            return errorMessageBuilder.ToString();
        } 
    }
}