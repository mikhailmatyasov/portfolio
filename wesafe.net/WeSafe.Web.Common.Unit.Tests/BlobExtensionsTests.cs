using Microsoft.AspNetCore.Http.Internal;
using System.IO;
using System.Linq;
using WeSafe.Web.Common.Services;
using Xunit;

namespace WeSafe.Web.Common.Unit.Tests
{
    public class BlobExtensionsTests
    {
        [Fact]
        public void EncodeFile_EncodeFileFormCollection()
        {
            var result = BlobExtensions.EncodeFile(GetFileCollection());

            Assert.NotNull(result);
            Assert.True(result.Count() == 1);

            var blob = result.First();
            Assert.NotEmpty(blob.EncodedFile);
        }

        protected FormFileCollection GetFileCollection()
        {
            var fileStream = File.Open("FormFile.png", FileMode.Open, FileAccess.Read, FileShare.Read);
            var file = new FormFile(fileStream, 0, fileStream.Length, "Data", "dummy.txt");

            return new FormFileCollection()
            {
                file
            };
        }
    }
}
