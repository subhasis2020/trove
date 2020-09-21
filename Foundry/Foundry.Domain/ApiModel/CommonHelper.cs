using System;
using System.Drawing;
using System.IO;

namespace Foundry.Domain.ApiModel
{
    public static class CommonHelper
    {


        public static byte[] Base64ToImage(string imageData)
        {
            return Convert.FromBase64String(imageData);
        }
    }
}
