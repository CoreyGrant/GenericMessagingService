using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GenericMessagingService.IntegrationTests.Assertions
{
    public static partial class CustomAssert
    {
        public static void FileExists(string path)
        {
            if (!File.Exists(path))
            {
                throw new CustomAssertionException("File does not exist");
            }
        }

        public static void FileExists(string folder, string filename)
        {
            FileExists(Path.Join(folder, filename));
        }
    }
}
