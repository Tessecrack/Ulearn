using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;
namespace Reflection.Randomness
{
    public class Generator<TypeObject> : IFUCK<TypeObject>
    {
        public PropertyInfo Property;
        public Dictionary<PropertyInfo, IContinousDistribution> Dictionary;
        public string HelperPropertyName;
        public Generator()
        {
            Dictionary = new Dictionary<PropertyInfo, IContinousDistribution>();
            var properties = typeof(TypeObject).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var attributes = properties[i].GetCustomAttributes(typeof(FromDistribution), false);
                if (attributes.Length != 0)
                {
                    try
                    {
                        var attribute = (FromDistribution)properties[i].GetCustomAttribute(typeof(FromDistribution), false);
                        Dictionary.Add(properties[i], attribute.Distribution);
                    }
                    catch
                    {
                        var attribute = (FromDistribution)properties[i].GetCustomAttribute(typeof(FromDistribution), false);
                        throw new ArgumentException(attribute.GetType().ToString());
                    }
                }
            }
        }
        public TypeObject Generate(Random rnd)
        {
            var listOfBinds = new List<MemberBinding>();
            foreach (var element in Dictionary)
            {
                var expresion = Expression.Constant(element.Value.Generate(rnd));
                var bind = Expression.Bind(element.Key, expresion);
                listOfBinds.Add(bind);
            }
            var body = Expression.MemberInit(Expression.New(typeof(TypeObject).GetConstructor(new Type[0])), listOfBinds);
            return Expression.Lambda<Func<TypeObject>>(body).Compile()();
        }
        public IFUCK<TypeObject> For(Expression<Func<TypeObject, double>> func)
        {
            try
            {
                HelperPropertyName = (func.Body as MemberExpression).Member.Name;
                bool checkNull = typeof(TypeObject).GetProperty(HelperPropertyName) == null;
                if (checkNull)
                    throw new ArgumentException();
                var prop = typeof(TypeObject).GetProperty(HelperPropertyName);
                return new Gen<TypeObject>(prop, this, HelperPropertyName);
            }
            catch
            { throw new ArgumentException(); }
        }

        public Generator<TypeObject> Set(IContinousDistribution distribution)
        {
            var properties = typeof(TypeObject).GetProperties();
            for (int i = 0; i < properties.Length; i++)
                if (properties[i].Name == HelperPropertyName)
                { Dictionary[properties[i]] = distribution; break; }
            return this;
        }
    }
    public interface IFUCK<TypeObject>
    {
        Generator<TypeObject> Set(IContinousDistribution distribution);
    }
    public class Gen<TypeObject> : IFUCK<TypeObject>
    {
        private PropertyInfo property;
        private Generator<TypeObject> generator;
        private Dictionary<PropertyInfo, IContinousDistribution> dict;
        private string name;
        public Gen(PropertyInfo prop, Generator<TypeObject> gen, string propertyName)
        {
            property = prop;
            generator = gen;
            dict = generator.Dictionary;
            name = propertyName;
        }
        public Generator<TypeObject> Set(IContinousDistribution distribution)
        {
            var properties = typeof(TypeObject).GetProperties();
            for (int i = 0; i < properties.Length; i++)
                if (properties[i].Name == name)
                { dict[properties[i]] = distribution; break; }
            return generator;
        }
    }

    public class FromDistribution : Attribute
    {
        public IContinousDistribution Distribution { get; set; }
        private Type[] Initialize(int length)
        {
            Type[] arr = new Type[length];
            for (int i = 0; i < length; i++)
                arr[i] = typeof(double);
            return arr;
        }

        public FromDistribution(Type type, params object[] parameters)
        {
            if (parameters.Length > 2) throw new ArgumentException(type.Name);
            ConstructorInfo constructor = type.GetConstructor(Initialize(parameters.Length));
            Distribution = (IContinousDistribution)constructor.Invoke(parameters);
        }
    }
}