namespace EditorPrototype
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    public sealed class BoolToColorConverter : IValueConverter
    {
        public Brush TrueColor { get; set; } = Brushes.LightBlue;

        public Brush FalseColor { get; set; } = Brushes.Yellow;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                return this.TrueColor;
            }

            return (bool)value ? this.TrueColor : this.FalseColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Brush))
            {
                return false;
            }

            return ReferenceEquals((Brush)value, this.TrueColor);
        }
    }
}
