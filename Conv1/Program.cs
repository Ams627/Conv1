using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Threading.Tasks;
using CLib1;
using System.Reflection;

[assembly: CLib1.ConverterProvider]

namespace Conv1
{
    public class XX1 : IParameterConverter
    {

    }
    class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var c = new ConverterManager();
            }
            catch (Exception ex)
            {
                var fullname = System.Reflection.Assembly.GetEntryAssembly().Location;
                var progname = Path.GetFileNameWithoutExtension(fullname);
                Console.Error.WriteLine($"{progname} Error: {ex.Message}");
            }

        }
    }
}
