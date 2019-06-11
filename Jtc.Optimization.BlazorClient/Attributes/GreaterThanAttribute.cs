using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Jtc.Optimization.BlazorClient.Attributes
{
    public class GreaterThanAttribute : ValidationAttribute
    {

        private readonly string _relatedField;

        public GreaterThanAttribute(string relatedField)
        {
            _relatedField = relatedField;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {          
            var type = validationContext.ObjectInstance.GetType();
            var raw = type.GetProperty(_relatedField).GetValue(validationContext.ObjectInstance);
            if (raw == null)
            {
                return ValidationResult.Success;
            }

            if (value.GetType().IsAssignableFrom(typeof(int)) && (int)value > (int)raw)
            {
                return ValidationResult.Success;
            }
            if (value.GetType().IsAssignableFrom(typeof(double)) && (double)value > (double)raw)
            {
                return ValidationResult.Success;
            }
            if (value.GetType().IsAssignableFrom(typeof(decimal)) && (decimal)value > (decimal)raw)
            {
                return ValidationResult.Success;
            }
            if (value.GetType().IsAssignableFrom(typeof(DateTime)) && (DateTime)value > (DateTime)raw)
            {
                return ValidationResult.Success;
            }

            Console.WriteLine($"{validationContext.MemberName} must be greater than {_relatedField}");

            //other overload does not work
            return new ValidationResult($"{validationContext.MemberName} must be greater than {_relatedField}", new[] {"StartDate" });
     
        }

    }
}
