using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using WeSafe.Bus.Contracts;

namespace WeSafe.Web.Common.Services
{
    public static class BlobExtensions
    {
        public static IEnumerable<Blob> EncodeFile(IFormFileCollection fileCollection)
        {
            var encodedFilers = new List<Blob>();

            foreach (var file in fileCollection)
            {
                encodedFilers.Add(EncodeFile(file));
            }

            return encodedFilers;
        }

        public static Blob EncodeFile(IFormFile file)
        {
            if (file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    var fileBytes = ms.ToArray();

                    return new Blob()
                    {
                        EncodedFile = Convert.ToBase64String(fileBytes),
                        ContentType = string.Empty
                    };
                }
            }

            return null;
        }
    }
}
