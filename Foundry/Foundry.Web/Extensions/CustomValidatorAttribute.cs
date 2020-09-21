using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace Foundry.Web.Extensions
{
    public class RequiredReloadAmountIfAttribute : ValidationAttribute, IClientModelValidator
    {
        private String PropertyName { get; set; }
        private new String ErrorMessage { get; set; }
        private Object DesiredValue { get; set; }

        public RequiredReloadAmountIfAttribute(String propertyName, Object desiredvalue, String errormessage)
        {
            this.PropertyName = propertyName;
            this.DesiredValue = desiredvalue;
            this.ErrorMessage = errormessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Object instance = validationContext.ObjectInstance;
            Type type = instance.GetType();
            Object proprtyvalue = type.GetProperty(PropertyName).GetValue(instance, null);
            if (proprtyvalue.ToString() == DesiredValue.ToString() && value == null)
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-reloadamount",
    "Amount for auto reload is required.");
        }

    }

    public class RequiredDroppedAmountIfAttribute : ValidationAttribute, IClientModelValidator
    {
        private String PropertyName { get; set; }
        private new String ErrorMessage { get; set; }
        private Object DesiredValue { get; set; }

        public RequiredDroppedAmountIfAttribute(String propertyName, Object desiredvalue, String errormessage)
        {
            this.PropertyName = propertyName;
            this.DesiredValue = desiredvalue;
            this.ErrorMessage = errormessage;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-droppedamount",
    "Please select for the amount drop.");
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Object instance = validationContext.ObjectInstance;
            Type type = instance.GetType();
            Object proprtyvalue = type.GetProperty(PropertyName).GetValue(instance, null);
            if (proprtyvalue.ToString() == DesiredValue.ToString() && value == null)
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }


    [AttributeUsage(AttributeTargets.Class)]
    public class AtLeastOneCheckboxAttribute : ValidationAttribute
    {
        private readonly string[] _checkboxNames;

        public AtLeastOneCheckboxAttribute(params string[] checkboxNames)
        {
            _checkboxNames = checkboxNames;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyInfos = value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => _checkboxNames.Contains(x.Name));

            var values = propertyInfos.Select(x => x.GetGetMethod().Invoke(value, null));
            if (values.Any(x => Convert.ToBoolean(x)))
            {
                return ValidationResult.Success;
            }
            else
            {
                ErrorMessage = "At least one checkbox must be selected";
                return new ValidationResult(ErrorMessage);
            }
        }
    }

    public class DateGreaterThanAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly string otherPropertyName;
        public DateGreaterThanAttribute() { }
        public DateGreaterThanAttribute(string otherPropertyName, string errorMessage)
            : base(errorMessage)
        {
            this.otherPropertyName = otherPropertyName;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-isgreater",
    "Start date is greater than end date.");
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = ValidationResult.Success;
            // Using reflection we can get a reference to the other date property, in this example the project start date
            var containerType = validationContext.ObjectInstance.GetType();
            var field = containerType.GetProperty(this.otherPropertyName);
            var extensionValue = field?.GetValue(validationContext.ObjectInstance, null);
            if (extensionValue == null)
            {
                return validationResult;
            }
            // Let's check that otherProperty is of type DateTime as we expect it to be
            if ((field.PropertyType == typeof(DateTime) || (field.PropertyType.IsGenericType && field.PropertyType == typeof(Nullable<DateTime>))))
            {
                DateTime toValidate = (DateTime)value;
                DateTime referenceProperty = (DateTime)field.GetValue(validationContext.ObjectInstance, null);
                // if the end date is lower than the start date, than the validationResult will be set to false and return
                // a properly formatted error message
                if (toValidate.CompareTo(referenceProperty) < 1)
                {
                    validationResult = new ValidationResult(ErrorMessageString);
                }
            }
            else
            {
                validationResult = new ValidationResult("An error occurred while validating the property. OtherProperty is not of type DateTime");
            }

            return validationResult;
        }

    }

}
