using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace sourceSearch
{
    internal class FileHandler : IHandler
    {
        public async void Handle(string path)
        {
            int index;
            string newStr;
            string postResult, hebrowResult = "", oldRes;
            string sameRes;
            FileInfo fi = new FileInfo(path);
            IFileReader reader;
            if (fi.Extension == ".txt")
                reader = new TextFileReader();
            else
                reader = new WordFileReader();

            var txt = reader.Read(path);

            var verses = FindByRegex.FindVerse(txt);

            foreach (string verse in verses)
            {
                oldRes = hebrowResult;
                Console.WriteLine(verse);
                postResult = await Post.VerseSource(verse);
                hebrowResult = ResultFunc.HebrowSource(postResult);
                if (hebrowResult != "0")
                {
                    sameRes = ResultFunc.SameVerse(hebrowResult, oldRes);
                    index = txt.LastIndexOf(verse);
                    newStr = txt.Insert(index, " (" + sameRes + ")");
                    Console.WriteLine("(" + sameRes + ")");
                    txt = newStr;
                }
                else
                {
                    hebrowResult = ResultFunc.PartVerse(verse);
                    if (hebrowResult != "0")
                    {
                        sameRes = ResultFunc.SameVerse(hebrowResult, oldRes);
                        index = txt.LastIndexOf(verse);
                        newStr = txt.Insert(index, " (" + sameRes + ")");
                        Console.WriteLine("(" + sameRes + ")");
                        txt = newStr;
                    }


                }
            }
            var pathRes = @"C:\Users\eliwa\OneDrive\רינת\אבוש\sourceSearch\ConsoleApp1\res.txt";


            File.WriteAllText(pathRes, txt);

        }
    }
}
