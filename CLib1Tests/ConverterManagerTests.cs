using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLib1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;

[assembly: CLib1.ConverterProvider]
namespace CLib1.Tests
{
    namespace N1
    {
        class Converter1 : IParameterConverter
        {

        }
    }

    namespace N2
    {
        class Converter1 : IParameterConverter
        {

        }
        class Converter2 : IParameterConverter
        {

        }
        class Converter3 : IParameterConverter
        {

        }
    }


    [TestClass()]
    public class ConverterManagerTests
    {
        [TestMethod()]
        public void GetConverterTest()
        {
            CLib1.IConverterManager mgr = new ConverterManager();
            mgr.Should().NotBeNull();
            var converter = mgr.GetConverter("Converter3");
            converter.Should().BeOfType<N2.Converter3>().Which.GetType().Should().Implement<IParameterConverter>();
        }

        [TestMethod()]
        public void GetConverterTestDuplicateShortName()
        {
            CLib1.IConverterManager mgr = new ConverterManager();
            mgr.Should().NotBeNull();
            mgr.GetConverter("Hello").Should().BeNull();
            Action a1 = () => mgr.GetConverter("Converter1");
            a1.Should().Throw<Exception>().Which.Message.Contains("Converter cannot be uniquely specified by");
        }

        [TestMethod()]
        public void GetConverterTestDuplicateType()
        {
            // if there is a (namespace, typename) duplicate, it needs to be in another assembly otherwise it
            // would not compile, so we will create an assembly dynamically from code:
            var dll = CreateAssembly(this.GetType().Namespace);
            var load = Assembly.LoadFrom(dll);
            Action a = ()=>new ConverterManager();
            a.Should().Throw<Exception>().Which.Message.Contains("The following types are duplicates");
        }


        private string CreateAssembly(string nspace)
        {
            var name = "generated1.dll";
            var parameters = new CompilerParameters
            {
                GenerateExecutable = false,
                OutputAssembly = name
            };
            parameters.ReferencedAssemblies.Add("CLib1.dll");

            var code =
            $@"using CLib1;
              [assembly: CLib1.ConverterProvider]
              namespace {nspace}.N2 {{
                class Converter2 : IParameterConverter
                {{

                }}
            }}";

            var results = CodeDomProvider.CreateProvider("CSharp").CompileAssemblyFromSource(parameters, code);
            results.Errors.Count.Should().Be(0);
            return name;
        }
    }
}