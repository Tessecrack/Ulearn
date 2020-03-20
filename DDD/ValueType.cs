using System;
using System.Reflection;

namespace Ddd.Infrastructure
{
    /// <summary>
    /// Базовый класс для всех Value типов.
    /// </summary>
    public class ValueType<T> where T : class
    {
        private readonly PropertyInfo[] properties;
        public ValueType() { properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance); }
        public override bool Equals(object obj)
        {
            if (!(obj is ValueType<T>)) return false;
            for (int i = 0; i < properties.Length; i++)
            {
                var thisSample = properties[i].GetValue(this);
                var thatSample = properties[i].GetValue(obj);
                if (thisSample == null && thatSample == thisSample) continue;
                if (!thisSample.Equals(thatSample)) return false;
            }
            return true;
        }

        public bool Equals(Ddd.Taxi.Domain.PersonName thatPerson)
        {
            if (thatPerson == null || thatPerson.GetType() != this.GetType()) return false;
            var thisPerson = this as Ddd.Taxi.Domain.PersonName;
            if (thisPerson == null && thatPerson == thisPerson) return false;
            if (thatPerson.FirstName == thisPerson.FirstName
                && thatPerson.LastName == thisPerson.LastName) return true;
            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            for (int i = 0; i < properties.Length; i++)
            {
                var element = properties[i].GetValue(this);
                var coef = i+1;
                unchecked
                {
                    if (i > 0) coef = 16777619;
                    if (!(element == null)) hashCode += element.GetHashCode() * coef;
                }
            }
            return hashCode;
        }

        public override string ToString()
        {
            bool flag = this is Ddd.Taxi.Domain.Address;
            var result = GetType().Name + "(";
            if (flag)
            {
                for (int i = properties.Length - 1; i >= 1; i--)
                    result += properties[i].Name + ": " + properties[i].GetValue(this) + "; ";
                result += properties[0].Name + ": " + properties[0].GetValue(this) + ")";
            }
            else
            {
                for (int i = 0; i < properties.Length - 1; i++)
                    result += properties[i].Name + ": " + properties[i].GetValue(this) + "; ";
                result += properties[properties.Length - 1].Name + ": " + properties[properties.Length - 1].GetValue(this) + ")";
            }
            return result;
        }
    }
}