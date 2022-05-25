using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Aisger.Validation
{
    public class CustomDateAttribute : RangeAttribute
    {
        public CustomDateAttribute()
            : base(typeof(DateTime), DateTime.MinValue.ToShortDateString(), DateTime.Today.ToShortDateString())
        { }
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public class ValidDecimal : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || value.ToString().Length == 0)
            {
                return ValidationResult.Success;
            }
            decimal d;
            return !decimal.TryParse(value.ToString(), out d) ? new ValidationResult(ErrorMessage) : ValidationResult.Success;
        }
    }
public class ValidDecimalValidator : DataAnnotationsModelValidator<ValidDecimal>
{
    public ValidDecimalValidator(ModelMetadata metadata, ControllerContext context, ValidDecimal attribute)
        : base(metadata, context, attribute)
    {
        if (!attribute.IsValid(context.HttpContext.Request.Form[metadata.PropertyName]))
        {
            var propertyName = metadata.PropertyName;
            context.Controller.ViewData.ModelState[propertyName].Errors.Clear();
            context.Controller.ViewData.ModelState[propertyName].Errors.Add(attribute.ErrorMessage);
        }
    }

}
}