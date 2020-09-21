using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Foundry.Domain.Enums
{
    public class ImageFormat
    {
        public enum ImageFormatEnum
        {
            bmp,
            jpeg,
            gif,
            tiff,
            png,
            unknown
        }

        public static ImageFormatEnum GetImageFormat(byte[] bytes)
        {
            var bmp = Encoding.ASCII.GetBytes("BM");
            var gif = Encoding.ASCII.GetBytes("GIF");
            var png = new byte[] { 137, 80, 78, 71 };
            var tiff = new byte[] { 73, 73, 42 };
            var tiff2 = new byte[] { 77, 77, 42 };
            var jpeg = new byte[] { 255, 216, 255, 224 };
            var jpeg2 = new byte[] { 255, 216, 255, 225 };

            if (bmp.SequenceEqual(bytes.Take(bmp.Length)))
            {
                return ImageFormatEnum.bmp;
            }
            if (gif.SequenceEqual(bytes.Take(gif.Length)))
            {
                return ImageFormatEnum.gif;
            }
            if (png.SequenceEqual(bytes.Take(png.Length)))
            {
                return ImageFormatEnum.png;
            }
            if (tiff.SequenceEqual(bytes.Take(tiff.Length)))
            {
                return ImageFormatEnum.tiff;
            }
            if (tiff2.SequenceEqual(bytes.Take(tiff2.Length)))
            {
                return ImageFormatEnum.tiff;
            }
            if (jpeg.SequenceEqual(bytes.Take(jpeg.Length)))
            {
                return ImageFormatEnum.jpeg;
            }
            if (jpeg2.SequenceEqual(bytes.Take(jpeg2.Length)))
            {
                return ImageFormatEnum.jpeg;
            }
            return ImageFormatEnum.unknown;
        }


    }

    public class GeneralSettingData
    {
        public static Dictionary<string, string> GetTimeZonesInfo()
        {
            //  var result = TimeZoneInfo.GetSystemTimeZones();

            Dictionary<string, string> timeZone = new Dictionary<string, string>();

            timeZone.Add("EDT Eastern Daylight Time UTC – 4 hours", "Eastern Standard Time");
            timeZone.Add("CST Central Standard Time UTC – 6 hours", "Central Standard Time");
            timeZone.Add("CDT Central Daylight Time UTC – 5 hours", "Central Standard Time");
            timeZone.Add("MST Mountain Standard Time UTC – 7 hours", "Mountain Standard Time");

            return timeZone;

        }
        public static Dictionary<string, string> GetDataTypesOfDB()
        {
            //  var result = TimeZoneInfo.GetSystemTimeZones();

            Dictionary<string, string> dataTypes = new Dictionary<string, string>();

            dataTypes.Add("Int", "Int");
            dataTypes.Add("DateTime", "DateTime");
            dataTypes.Add("Time", "Time");
            dataTypes.Add("Money", "Money");
           // dataTypes.Add("Boolean", "Boolean");
            dataTypes.Add("String", "String");
            return dataTypes;

        }


        public static Dictionary<string, string> GetDataForIsRequired()
        {
            //  var result = TimeZoneInfo.GetSystemTimeZones();

            Dictionary<string, string> isRequiredContent = new Dictionary<string, string>();

            isRequiredContent.Add("Mandatory", "true");
            isRequiredContent.Add("Optional", "false");
         
            return isRequiredContent;

        }

        public static Dictionary<string, int> GetGenderDrpDwn()
        {
            //  var result = TimeZoneInfo.GetSystemTimeZones();

            Dictionary<string, int> isDrpContent = new Dictionary<string, int>();

            isDrpContent.Add("Male", 1);
            isDrpContent.Add("Female", 2);

            return isDrpContent;

        }

    }
}
