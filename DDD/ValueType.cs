using Ddd.Taxi.Domain;
using System.Linq;
using System.Reflection;

namespace Ddd.Infrastructure
{
    /// <summary>
    /// Базовый класс для всех Value типов.
    /// </summary>
    public class ValueType<T> where T : class
    {
        private readonly PropertyInfo[] properties;

        public ValueType() => properties = typeof(T)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public);

        public override bool Equals(object obj)
        {
            if (!(obj is T) 
                || !(obj is ValueType<T> downCast) 
                || downCast.properties.Length != properties.Length) return false;

            for (var i = 0; i < properties.Length; i++)
            {
                var valueThisProp = properties[i].GetValue(this) ;
                var valueObjProp = downCast.properties[i].GetValue(downCast);
                if (valueThisProp == null && valueObjProp == null) continue;
                if (!valueThisProp.Equals(valueObjProp)) return false;
            }
            return true;
        }

        public bool Equals(PersonName personName) => Equals((object)personName);
        public override int GetHashCode()
        {
            int code = 0;
            for (int i = 0; i < properties.Length; i++)
            {
                unchecked
                {
                    if (!(properties[i].GetValue(this) == null))
                    {
                        var coef = i > 0 ? i + 1 : 16777;
                        code += properties[i].GetValue(this).GetHashCode() * coef;
                    }
                }
            }
            return code;
        }

        public override string ToString()
        {
            var typeName = typeof(T).FullName.Split('.', '+').Last();
            var listOfProperties = properties
                .Select(element => element.Name + ": " + element.GetValue(this))
                .OrderBy(element => element[0]).Aggregate((s1, s2) => s1 + "; " + s2);
            return typeName + "(" + listOfProperties + ")";
        }
    }
}
