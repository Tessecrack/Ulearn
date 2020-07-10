using System;

namespace Inheritance.DataStructure
{
    public class Category : IComparable
    {
        public string Letter { get; }
        public MessageType TypeMessage { get; }
        public MessageTopic TopicMessage { get; }
        public Category(string name, MessageType messageType, MessageTopic messageTopic)
        {
            Letter = name;
            TypeMessage = messageType;
            TopicMessage = messageTopic;
        }

        public override bool Equals(object obj)
            => this is Category
            && this != null
            && obj != null 
            && obj is Category 
            && CompareTo(obj as Category) == 0;

        public override string ToString() 
            => Letter + "." + TypeMessage.ToString() + "." + TopicMessage.ToString();

        public override int GetHashCode()
            => Letter.GetHashCode() + (int)TypeMessage + (int)TopicMessage + TopicMessage.ToString().Length;
        

        public int CompareTo(object obj)
        {
            if (!(obj is Category externalObject) 
                || obj == null && this == null ||
                externalObject.Letter == null || this.Letter == null) return 0;

            if (Letter.CompareTo(externalObject.Letter) == -1) return -1;
            else if (Letter.CompareTo(externalObject.Letter) == 1) return 1;

            if ((int)TypeMessage < (int)externalObject.TypeMessage) return -1;
            else if ((int)TypeMessage > (int)externalObject.TypeMessage) return 1;

            if ((int)TopicMessage < (int)externalObject.TopicMessage) return -1;
            else if ((int)TopicMessage > (int)externalObject.TopicMessage) return 1;

            return 0;
        }

        public static bool operator <=(Category catFirst, Category catSecond)
            => catFirst == null && catSecond == null ||
            catFirst.CompareTo(catSecond) == -1 || catFirst.CompareTo(catSecond) == 0;
        public static bool operator >=(Category catFirst, Category catSecond)
            => catFirst == null && catSecond == null ||
            catFirst.CompareTo(catSecond) == 1 || catFirst.CompareTo(catSecond) == 0;
        public static bool operator <(Category catFirst, Category catSecond)
            => catFirst != null && catSecond != null 
            && catFirst.CompareTo(catSecond) == -1;
        public static bool operator >(Category catFirst, Category catSecond)
            => catFirst != null && catSecond != null
            && catFirst.CompareTo(catSecond) == 1;
    }
}
