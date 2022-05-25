using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;

namespace Aisger.Validation
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        // Note: we don't inherit from RequiredAttribute as some elements of the MVC
        // framework specifically look for it and choose not to add a RequiredValidator
        // for non-nullable fields if one is found. This would be invalid if we inherited
        // from it as obviously our RequiredIf only applies if a condition is satisfied.
        // Therefore we're using a private instance of one just so we can reuse the IsValid
        // logic, and don't need to rewrite it.
        private RequiredAttribute innerAttribute = new RequiredAttribute();
        public string DependentProperty { get; set; }
        public object TargetValue { get; set; }

        public RequiredIfAttribute(string dependentProperty, object targetValue)
        {
            this.DependentProperty = dependentProperty;
            this.TargetValue = targetValue;
        }

        public override bool IsValid(object value)
        {
            return innerAttribute.IsValid(value);
        }
    }


    public class MyCustomValidation : ValidationAttribute, IClientValidatable
    {
        public string DependentProperty { get; set; }

        public MyCustomValidation(string dependentProperty)
        {
            this.DependentProperty = dependentProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(DependentProperty);
            if (property == null)
            {
                return new ValidationResult(string.Format(
                    CultureInfo.CurrentCulture,
                    "Unknown property {0}",
                    new[] { DependentProperty }
                ));
            }
            
            System.Int64? otherPropertyValue = (System.Int64?)property.GetValue(validationContext.ObjectInstance, null);
            if (otherPropertyValue==null)
            {
                return new ValidationResult(ResourceSetting.ChooseValue);
            }
            if (value.ToString() == "0")
            {
                return new ValidationResult(ResourceSetting.ChooseValue);
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "requiredif",
            };
            rule.ValidationParameters.Add("other", DependentProperty);
            yield return rule;
        }
    }

    public class MyCustomTextValidation : ValidationAttribute, IClientValidatable
    {
        public string DependentProperty { get; set; }

        public MyCustomTextValidation(string dependentProperty)
        {
            this.DependentProperty = dependentProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(DependentProperty);
            if (property == null)
            {
                return new ValidationResult(string.Format(
                    CultureInfo.CurrentCulture,
                    "Unknown property {0}",
                    new[] { DependentProperty }
                ));
            }

            System.Int64? otherPropertyValue = (System.Int64?)property.GetValue(validationContext.ObjectInstance, null);
            if (otherPropertyValue == null)
            {
                return ValidationResult.Success;
            }
            if ( (value == null || string.IsNullOrEmpty(value.ToString())))
            {
                return new ValidationResult(ResourceSetting.NotEmpty);
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "requiredif",
            };
            rule.ValidationParameters.Add("other", DependentProperty);
            yield return rule;
        }
    }
}