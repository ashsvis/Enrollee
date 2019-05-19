using System;
using System.Collections;

namespace EnrolleeModel
{
    /// <summary>
    /// Корневой класс модели, содержит списки (таблицы) сущностей
    /// </summary>
    [Serializable]
    public class Root
    {
        public Matters Matters { get; set; }
        public Specialities Specialities { get; set; }
        public PassMatters PassMatters { get; set; }
        public Enrollees Enrollees { get; set; }

        public Root()
        {
            RegistryTables();
        }

        public void RegistryTables()
        {
            Tables.Clear();

            if (Matters == null) Matters = new Matters();
            RegistryTable("Matters", new Matter(), Matters);
            if (Specialities == null) Specialities = new Specialities();
            RegistryTable("Specialities", new Speciality(), Specialities);
            if (PassMatters == null) PassMatters = new PassMatters();
            RegistryTable("PassMatters", new PassMatter(), PassMatters);
            if (Enrollees == null) Enrollees = new Enrollees();
            RegistryTable("Enrollees", new Enrollee(), Enrollees);

        }

        public Hashtable Tables { get; private set; } = new Hashtable();

        private void RegistryTable(string name, object item, object table)
        {
            if (Tables.ContainsKey(name)) return;
            Tables[name] = new TableInfo
            {
                TableName = name,
                Table = table,
                Item = item
            };
        }

    }

    [Serializable]
    public class TableInfo
    {
        public string TableName { get; set; }
        public object Table { get; set; }
        public object Item { get; set; }
    }
}
