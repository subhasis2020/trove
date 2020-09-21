using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundry.Services
{
    public static class ParameterValidator
    {
        public static void ValidateObject(object obj, string nameParameter, string customMessage = null)
        {
            if (obj == null) { throw new ArgumentNullException(nameParameter, customMessage ?? $"The parameter {nameParameter} it is not null"); }
        }

        public static void ValidateString(string str, string nameParameter, string customMessage = null)
        {
            if (string.IsNullOrWhiteSpace(str)) { throw new ArgumentNullException(nameParameter, customMessage ?? $"The parameter {nameParameter} it is not null/empty/white"); }
        }

        public static void ValidateEnumerable(System.Collections.IEnumerable enumerable, string nameParameter, string customMessage = null)
        {
            ValidateObject(enumerable, nameParameter);

            var nullItems = ToEnumerableObj(enumerable).Where(a => a == null);

            if (nullItems.Any()) { throw new ArgumentNullException(nameParameter, customMessage ?? $"The colec-parameter {nameParameter} has not any null item"); }
        }

        public static void ValidateEnumerableString(IEnumerable<string> enumerable, string nameParameter, string customMessage = null)
        {
            ValidateObject(enumerable, nameParameter);

            var nullItems = enumerable.Where(a => string.IsNullOrWhiteSpace(a));

            if (nullItems.Any()) { throw new ArgumentNullException(nameParameter, customMessage ?? $"The colec-parameter {nameParameter} has not any null/empty/white item"); }
        }
                          
        private static IEnumerable<object> ToEnumerableObj(System.Collections.IEnumerable enumerable)
        {
            var result = new List<object>();

            foreach (var e in enumerable)
            {
                result.Add(e);
            }

            return result;
        }
    }
}
