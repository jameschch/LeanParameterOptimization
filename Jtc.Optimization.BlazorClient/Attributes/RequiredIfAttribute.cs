using System;
using System.ComponentModel.DataAnnotations;

namespace Jtc.Optimization.BlazorClient.Attributes
{

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class RequiredIfAttribute : RequiredAttribute
    {
        private readonly string _relatedField;
        private readonly bool _related;
        private readonly Type _relatedType;

        protected RequiredIfAttribute(string relatedField, bool related, Type relatedType)
        {
            _relatedField = relatedField;
            _related = related;
            _relatedType = relatedType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var type = validationContext.ObjectInstance.GetType();
            var raw = type.GetProperty(_relatedField).GetValue(validationContext.ObjectInstance);
            if (raw == null)
            {
                return ValidationResult.Success;
            }

            var validating = Convert.ChangeType(raw, _relatedType);

            if (validating.Equals(_related))
            {
                return base.IsValid(value, validationContext);
            }

            return ValidationResult.Success;
        }
    }

    public class RequiredIfTrueAttribute : RequiredIfAttribute
    {
        public RequiredIfTrueAttribute(string relatedField) : base(relatedField, true, typeof(bool))
        {

        }
    }

    public class RequiredIfFalseAttribute : RequiredIfAttribute
    {
        public RequiredIfFalseAttribute(string relatedField) : base(relatedField, false, typeof(bool))
        {

        }
    }
}
