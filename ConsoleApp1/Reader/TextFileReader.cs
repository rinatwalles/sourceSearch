using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sourceSearch
{
    internal class TextFileReader : IFileReader
    {
        public string Read(string path)
        {
            return File.ReadAllText(@path);
        }
    }
}
