using System;
using System.Collections.Generic;

namespace Generics.Tables
{

    public class Table<TypeRow, TypeColumn, TypeValue>
    {
        public Table()
        {
            Rows = new HashSet<TypeRow>();
            Columns = new HashSet<TypeColumn>();
            Values = new Dictionary<Tuple<TypeRow, TypeColumn>, TypeValue>();
        }
        public HashSet<TypeRow> Rows { get; set; }
        public HashSet<TypeColumn> Columns { get; set; }
        public Dictionary<Tuple<TypeRow, TypeColumn>, TypeValue> Values { get; set; }
        public void AddRow(TypeRow row) { Rows.Add(row); }
        public void AddColumn(TypeColumn column) { Columns.Add(column); }
        public OpenIndexator Open => new OpenIndexator(this);
        public ExistIndexator Existed => new ExistIndexator(this);


        public class OpenIndexator
        {
            private Table<TypeRow, TypeColumn, TypeValue> leftTable;
            public OpenIndexator(Table<TypeRow, TypeColumn, TypeValue> myTable) { this.leftTable = myTable; }
            public TypeValue this[TypeRow row, TypeColumn column]
            {
                get 
                {
                    var key = Tuple.Create(row, column);
                    if (leftTable.Values.ContainsKey(key)) return leftTable.Values[key];
                    return default;
                }
                set
                {
                    var key = Tuple.Create(row, column);
                    if (leftTable.Values.ContainsKey(key))
                        leftTable.Values[key] = value;
                    else
                    {
                        leftTable.AddRow(row);
                        leftTable.AddColumn(column);
                        leftTable.Values.Add(key, value);
                    }
                }
            }

        }
        public class ExistIndexator
        {
            private Table<TypeRow, TypeColumn, TypeValue> leftTable;
            public ExistIndexator(Table<TypeRow, TypeColumn, TypeValue> myTable) { this.leftTable = myTable; }
            public TypeValue this[TypeRow row, TypeColumn column]
            {
                get
                {
                    var key = Tuple.Create(row, column);
                    if (leftTable.Values.ContainsKey(key)) return leftTable.Values[key];
                    if (!leftTable.Rows.Contains(row) || !leftTable.Columns.Contains(column))
                        throw new ArgumentException();
                    return default;
                }
                set
                {
                    var key = Tuple.Create(row, column);
                    if (leftTable.Values.ContainsKey(key)) leftTable.Values[key] = value;
                    else if (leftTable.Rows.Contains(row) && leftTable.Columns.Contains(column))
                        leftTable.Values.Add(key, value);
                    else throw new ArgumentException();
                }
            }
        }
    }
}
