﻿using Logo.TurtleInterfaces;
using System;
using System.Globalization;
using System.Windows.Data;
using PerformersScene.Models;
using PerformersScene.Models.Constants;
using PerformersScene.Operations;
using DoublePoint = PerformersScene.Models.DoublePoint;

namespace PerformersScene.Controls.Converters
{
    public class XConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            DoublePoint start = (DoublePoint)values[0];
            DoublePoint end = (DoublePoint)values[1];
            double speed = (double)values[2];
            double tag = (double)values[3];
            DoublePoint shift = (DoublePoint) values[4];
            var lambda = tag;
            // position = (1-lambda)*start+lambda*end
            var position = PointOperations.Plus(PointOperations.ScalarMultiply(1 - lambda, start),
                PointOperations.ScalarMultiply(lambda, end));
            var transformedPosition = PointOperations.Minus(position, shift);
            return transformedPosition.X;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}