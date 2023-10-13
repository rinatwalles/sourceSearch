using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace sourceSearch
{
    internal class FileHandler : IFileHandler
    {
        public async void Handle(string path)
        {
            FileInfo fi = new FileInfo(path);
            IFileReader reader;
            if (fi.Extension == "txt")
                reader = new TextFileReader();
            else
                reader = new WordFileReader();

            var txt = reader.Read(path);

            //var psukim = FindAllThePsukim(txt);


        }
    }
}
