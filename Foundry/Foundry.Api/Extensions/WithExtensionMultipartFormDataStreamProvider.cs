using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Foundry.Api.Extensions
{
    public class WithExtensionMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public WithExtensionMultipartFormDataStreamProvider(string rootPath)
          : base(rootPath)
        {
        }

        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            string extension = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ? Path.GetExtension(GetValidFileName(headers.ContentDisposition.FileName)) : "";
            return Guid.NewGuid().ToString() + extension;
        }
        public static string GetValidFileName(string filePath)
        {
            char[] invalids = System.IO.Path.GetInvalidFileNameChars();
            return String.Join("_", filePath.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
        }
    }
}
