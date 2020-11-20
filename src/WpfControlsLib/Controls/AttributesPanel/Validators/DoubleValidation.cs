using System;
using System.Globalization;
using System.Windows.Controls;

namespace WpfControlsLib.Controls.AttributesPanel.Validators
{
    public class DoubleValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (Double.TryParse(value.ToString(), out double _))
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
