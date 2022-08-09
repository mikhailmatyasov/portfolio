using System.IO;
using Microsoft.AspNetCore.Http;

namespace WeSafe.Event.Unit.Tests.AddEvent
{
    public abstract class AddEventBaseTest
    {
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
