﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace GigHub.BusinessRules
{
    public class ValidTime : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var isValid = DateTime.TryParseExact(Convert.ToString(value), 
                "HH:mm", CultureInfo.CurrentCulture,
                DateTimeStyles.None, out var dateTime);

            return isValid;
        }
    }
}