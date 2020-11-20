﻿using System;
using System.Globalization;
using System.Windows.Controls;

namespace WpfEditor.View.Validators
{
    public class IntValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (Int32.TryParse(value.ToString(), out int result))
            {
                return new ValidationResult(true, null);
            }
            else
            {
                return new ValidationResult(false, "it's not int value");
            }
        }
    }
}
