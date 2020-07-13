using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace Reflection.Randomness
{
    public interface ISet<T>
    {
        Generator<T> Set(IContinousDistribution distribution);
    }

    public class FromDistribution : Attribute
    {
        private Type distrType;
        private object[] args;

        public FromDistribution(Type typeOfDistribution, params object[] args)
        {
            distrType = typeOfDistribution;
            this.args = args;
        }

        public IContinousDistribution GetDistribution()
        {
            try
            {
                return (IContinousDistribution)Activator.CreateInstance(distrType, args);
            }
            catch
            {
                throw new ArgumentException(distrType.Name);
            }
        }
    }

    public class Generator<T> : ISet<T>
    {
        private string propertyHandler = "";
        public Dictionary<PropertyInfo, IContinousDistribution> DictOfProperties; 
        public Generator()
        {
            DictOfProperties = new Dictionary<PropertyInfo, IContinousDistribution>();

            var properties = typeof(T).GetProperties();

            foreach(var property in properties)
            {
                if (property.GetCustomAttribute(typeof(FromDistribution), false) is FromDistribution attribute)
                {
                    DictOfProperties.Add(property, attribute.GetDistribution());
                }
            }
        }

        public T Generate(Random rnd)
        {
            var binds = new List<MemberBinding>();
            foreach (var element in DictOfProperties)
            {
                var bind = Expression.Bind(element.Key, Expression.Constant(element.Value.Generate(rnd)));
                binds.Add(bind);
            }
            return Expression.Lambda<Func<T>>
                (Expression.MemberInit(Expression.New(typeof(T).GetConstructor(new Type[0])), binds))
                .Compile()();
        }

        public ISet<T> For(Expression<Func<T, double>> expr)
        {
            try
            {
                propertyHandler = (expr.Body as MemberExpression).Member.Name;
                if (typeof(T).GetProperty(propertyHandler) == null)
                    throw new ArgumentException();
                return new GeneratorWithoutFor<T>(this, propertyHandler);
            }
            catch
            { 
                throw new ArgumentException(); 
            }
        }

        public Generator<T> Set(IContinousDistribution distribution)
        {
            var properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Name == propertyHandler)
                {
                    DictOfProperties[properties[i]] = distribution;
                    break;
                }
            }
            return this;
        }
    }

    public class GeneratorWithoutFor<T> : ISet<T>
    {
        private readonly string propertyHandlerTemp;
        private Generator<T> genTemp;
        public GeneratorWithoutFor(Generator<T> gen, 
            string propertyName)
        {
            propertyHandlerTemp = propertyName;
            genTemp = gen;
        }

        public Generator<T> Set(IContinousDistribution distribution)
        {
            var properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Name == propertyHandlerTemp)
                {
                    genTemp.DictOfProperties[properties[i]] = distribution;
                    break;
                }
            }
            return genTemp;
        }
    }
}
