using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace SAJEM.Shared.Extensions
{
    public static class PersianDigitExtension
    {
        public static long ToPersianNumber(this string numberString)
        {
            if (string.IsNullOrEmpty(numberString)) return 0;
            

            long result;
            if (long.TryParse(numberString, out result))
                return result;
            else
                return 0;
        }
        public static string ToLatinNumber(string persianNumber)
        {
            string englishNumber = "";
            foreach (char ch in persianNumber)
            {
                englishNumber += char.GetNumericValue(ch);
            }
            return englishNumber;
        }
        public static string ToPersian(this string numberString,bool digitGrouping=false)
        {
            if (string.IsNullOrEmpty(numberString)) return "";
            if(digitGrouping) numberString = String.Format("{0:n0}", numberString);
            string persianNumber = "";
            foreach (char ch in numberString)
            {
                if (Char.IsDigit(ch))
                    persianNumber += (char)(1776 + char.GetNumericValue(ch));
                else
                {
                    persianNumber += ch;
                }
            }
            return persianNumber;
        }
        public static string ToPersianString(this long? number, bool digitGrouping = false)
        {
            if (number == null) return "";
            string numberString = string.Empty;
            if (digitGrouping) numberString = string.Format("{0:n0}", number);
            string persianNumber = string.Empty;
            foreach (char ch in numberString)
            {
                if (Char.IsDigit(ch))
                    persianNumber += (char)(1776 + char.GetNumericValue(ch));
                else
                {
                    persianNumber += ch;
                }
            }
            return persianNumber;
        }

        public static string ToPersianString(this long number, bool digitGrouping = false)
        {
            string numberString = string.Empty;
            if (digitGrouping) numberString = string.Format("{0:n0}", number);
            string persianNumber = string.Empty;
            foreach (char ch in numberString)
            {
                if (Char.IsDigit(ch))
                    persianNumber += (char)(1776 + char.GetNumericValue(ch));
                else
                {
                    persianNumber += ch;
                }
            }
            return persianNumber;
        }

    }
}
