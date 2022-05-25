using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;

namespace Aisger.Validation
{
    public class RequiredIfValidator : DataAnnotationsModelValidator<RequiredIfAttribute>
    {
        public RequiredIfValidator(ModelMetadata metadata, ControllerContext context, RequiredIfAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            // no client validation - I might well blog about this soon!
            return base.GetClientValidationRules();
        }

        public override IEnumerable<ModelValidationResult> Validate(object container)
        {
            // get a reference to the property this validation depends upon
            var field = Metadata.ContainerType.GetProperty(Attribute.DependentProperty);

            if (field != null)
            {
                // get the value of the dependent property
                var value = field.GetValue(container, null);

                // compare the value against the target value
                if ((value == null && Attribute.TargetValue == null) ||
                    (value.Equals(Attribute.TargetValue)))
                {
                    // match => means we should try validating this field
                    if (!Attribute.IsValid(Metadata.Model))
                        // validation failed - return an error
                        yield return new ModelValidationResult { Message = ErrorMessage };
                }
            }
        }
    }

    public class RequiredIfOtherFieldIsNullAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string _otherProperty;
        public RequiredIfOtherFieldIsNullAttribute(string otherProperty)
        {
            _otherProperty = otherProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_otherProperty);
            if (property == null)
            {
                return new ValidationResult(string.Format(
                    CultureInfo.CurrentCulture,
                    "Unknown property {0}",
                    new[] { _otherProperty }
                ));
            }
            var otherPropertyValue = property.GetValue(validationContext.ObjectInstance, null);

            if (otherPropertyValue == null || otherPropertyValue as string == string.Empty)
            {
                if (value == null || value as string == string.Empty)
                {
                    return new ValidationResult(string.Format(
                        CultureInfo.CurrentCulture,
                        FormatErrorMessage(validationContext.DisplayName),
                        new[] { _otherProperty }
                    ));
                }
            }

            return null;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "requiredif",
            };
            rule.ValidationParameters.Add("other", _otherProperty);
            yield return rule;
        }
    }
}