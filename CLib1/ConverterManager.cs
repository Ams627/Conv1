using System;
using System.Linq;

[assembly: CLib1.ConverterProvider]
namespace CLib1
{
    public class ConverterManager : IConverterManager
    {
        private readonly ILookup<string, Type> _shortNameLookup;
        private readonly ILookup<string, Type> _longNameLookup;
        public ConverterManager()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(ConverterProviderAttribute)));

            var types = assemblies.SelectMany(x => x.GetTypes());
            var converters = types.Where(x => typeof(IParameterConverter).IsAssignableFrom(x) && !x.IsInterface).ToList();

            _longNameLookup = converters.ToLookup(x => x.FullName);
            if (_longNameLookup.Any(x => x.Count() > 1))
            {
                var dups = string.Join(", ", _longNameLookup.Where(x => x.Count() > 1).Select(y => y.Key));
                throw new Exception($"The following types are duplicates: {dups}");
            }
            _shortNameLookup = converters.ToLookup(x => x.Name);
        }

        public IParameterConverter GetConverter(string name)
        {
            Type type;
            if (name.Contains('.'))
            {
                type = _longNameLookup[name].FirstOrDefault();
            }
            else
            {
                var res = _shortNameLookup[name];
                if (res.Count() > 1)
                {
                    throw new Exception($"Converter cannot be uniquely specified by {name}.");
                }
                type = res?.FirstOrDefault();
            }
            if (type == null) return null;

            var instance = (IParameterConverter)Activator.CreateInstance(type);
            return instance;
        }
    }
}
