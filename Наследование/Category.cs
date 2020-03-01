using System;

namespace Inheritance.DataStructure
{
    public class Category : IComparable
    {
        string nameProduct;
        MessageType messageType;
        MessageTopic messageTopic;
        public Category(string productName, MessageType typeMessage, MessageTopic topicMessage)
        {
            if (productName == "" || productName == null) nameProduct = "";
            else nameProduct = productName;
            messageType = typeMessage;
            messageTopic = topicMessage;
        }
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            Category category = obj as Category;

            if (this == null) return -1;
            if (category == null) return 1;

            int comp = this.nameProduct.CompareTo(category.nameProduct);
            if (comp == -1 || comp == 1) return comp;

            if (this.messageType > category.messageType) return 1;
            else if (this.messageType < category.messageType) return -1;

            if (this.messageTopic > category.messageTopic) return 1;
            else if (this.messageTopic < category.messageTopic) return -1;

            return 0;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Category outside = obj as Category;
            if (outside == null) return false;

            return
                this.nameProduct == outside.nameProduct &&
                this.messageType == outside.messageType &&
                this.messageTopic == outside.messageTopic;
        }
        public override string ToString() => nameProduct + "." + messageType + "." + messageTopic;
        public override int GetHashCode() =>
            nameProduct.GetHashCode() + (int)messageType + (int)messageTopic + messageTopic.ToString().Length;
        public static bool operator >(Category c1, Category c2)
        {
            if (c1 == null || c2 == null) return false;
            if (c1.CompareTo(c2) == 1) return true;
            else if (c1.CompareTo(c2) == -1) return false;
            return ((int)c1.messageTopic +
                (int)c1.messageType) >
                ((int)c2.messageTopic +
                (int)c2.messageType);
        }
        public static bool operator <(Category c1, Category c2)
        {
            if (c1 == null || c2 == null) return false;
            if (c1.CompareTo(c2) == -1) return true;
            else if (c1.CompareTo(c2) == 1) return false;
            return ((int)c1.messageTopic +
                (int)c1.messageType) <
                ((int)c2.messageTopic +
                (int)c2.messageType);
        }
        public static bool operator <= (Category c1, Category c2)
        {
            if (c1 == null || c2 == null) return false;
            if (c1.CompareTo(c2) == -1 || c1.CompareTo(c2) == 0) return true;
            else return false;
        }
        public static bool operator >=(Category c1, Category c2)
        {
            if (c1 == null || c2 == null) return false;
            if (c1.CompareTo(c2) == 1 || c1.CompareTo(c2) == 0) return true;
            else return false;
        }
    }
}
