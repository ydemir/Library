using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Helpers
{
    public static class DateTimeOffSetExtensions
    {
        public static int GetCurrentAge(this DateTimeOffset datetimeOffset)
        {
            var currentDate = DateTime.UtcNow;
            int age = currentDate.Year - datetimeOffset.Year;
            if (currentDate < datetimeOffset.AddYears(age))
            {
                age--;
            }
            return age;
        }
    }
}
