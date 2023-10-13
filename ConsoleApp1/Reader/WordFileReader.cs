using Syncfusion.DocIO.DLS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace sourceSearch
{
    internal class WordFileReader : IFileReader
    {
        public string Read(string path)
        {
            using WordDocument document = new WordDocument(@"C:\Users\eliwa\Downloads\english.docx");
            return document.GetText();
        }
    }
}
