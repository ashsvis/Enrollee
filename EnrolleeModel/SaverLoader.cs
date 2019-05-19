using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace EnrolleeModel
{
    /// <summary>
    /// Класс поддержки чтения/записи конфигурации в файл на локальном диске
    /// </summary>
    public static class SaverLoader
    {
        /// <summary>
        /// Метод загрузки сохранённой ранее конфигурации из локального файла
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Root LoadFromFile(string fileName)
        {
            using (var fs = File.OpenRead(fileName))
            using (var zip = new GZipStream(fs, CompressionMode.Decompress))
            {
                var formatter = new BinaryFormatter();
                return (Root)formatter.Deserialize(zip);
            }
        }

        /// <summary>
        /// Метод сохранения конфигурации в файл на локальном диске
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="root"></param>
        public static void SaveToFile(string fileName, Root root)
        {
            using (var fs = File.Create(fileName))
            using (var zip = new GZipStream(fs, CompressionMode.Compress))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(zip, root);
            }
        }

        /// <summary>
        /// Свойство для хранения результата (текста ошибки) последней операции
        /// </summary>
        public static string OperationResult { get; private set; } = string.Empty;

        /// <summary>
        /// Метод восстановления содержимого модели из сервера
        /// </summary>
        /// <param name="root">Ссылка на корневой объект модели</param>
        /// <param name="connection">Строка подключения</param>
        /// <returns></returns>
        public static bool RestoreTables(Root root, string connection)
        {
            var server = new Database.SqlServer { Connection = connection };
            // предметы
            var dataSet = server.GetRows("Matters");
            if (dataSet.Tables.Count > 0)
            {
                root.Matters.Clear();
                foreach (var row in dataSet.Tables[0].Rows.Cast<DataRow>())
                {
                    if (row.ItemArray.Length != 2) continue;
                    root.Matters.Add(new Matter
                    {
                        IdMatter = Guid.Parse(row.ItemArray[0].ToString()),
                        Name = row.ItemArray[1].ToString()
                    });
                }
            }
            OperationResult = server.LastError;
            if (!string.IsNullOrWhiteSpace(OperationResult)) return false;
            // специальности
            dataSet = server.GetRows("Specialities");
            if (dataSet.Tables.Count > 0)
            {
                root.Specialities.Clear();
                foreach (var row in dataSet.Tables[0].Rows.Cast<DataRow>())
                {
                    if (row.ItemArray.Length != 2) continue;
                    root.Specialities.Add(new Speciality
                    {
                        IdSpeciality = Guid.Parse(row.ItemArray[0].ToString()),
                        Name = row.ItemArray[1].ToString()
                    });
                }
            }
            OperationResult = server.LastError;
            if (!string.IsNullOrWhiteSpace(OperationResult)) return false;
            // сдаваемые предметы
            dataSet = server.GetRows("PassMatters");
            if (dataSet.Tables.Count > 0)
            {
                root.PassMatters.Clear();
                foreach (var row in dataSet.Tables[0].Rows.Cast<DataRow>())
                {
                    if (row.ItemArray.Length != 4) continue;
                    root.PassMatters.Add(new PassMatter
                    {
                        IdPassMatter = Guid.Parse(row.ItemArray[0].ToString()),
                        IdSpeciality = Guid.Parse(row.ItemArray[1].ToString()),
                        IdMatter = Guid.Parse(row.ItemArray[2].ToString()),
                        PassForm = (PassKind)Enum.Parse(typeof(PassKind), row.ItemArray[3].ToString())
                    });
                }
            }
            OperationResult = server.LastError;
            if (!string.IsNullOrWhiteSpace(OperationResult)) return false;
            // абитуриенты
            dataSet = server.GetRows("Enrollees");
            if (dataSet.Tables.Count > 0)
            {
                root.Enrollees.Clear();
                foreach (var row in dataSet.Tables[0].Rows.Cast<DataRow>())
                {
                    if (row.ItemArray.Length != 16) continue;
                    root.Enrollees.Add(new Enrollee
                    {
                        IdEnrollee = Guid.Parse(row.ItemArray[0].ToString()),
                        RegistrationNumber = row.ItemArray[1].ToString(),
                        Surname = row.ItemArray[2].ToString(),
                        FirstName = row.ItemArray[3].ToString(),
                        LastName = row.ItemArray[4].ToString(),
                        BirthDay = DateTime.Parse(row.ItemArray[5].ToString()),
                        SecodarySchoolName = row.ItemArray[6].ToString(),
                        SecodarySchoolNumber = row.ItemArray[7].ToString(),
                        SecodarySchoolTown = row.ItemArray[8].ToString(),
                        GraduationDate = DateTime.Parse(row.ItemArray[9].ToString()),
                        GoldMedal = bool.Parse(row.ItemArray[10].ToString()),
                        Town = row.ItemArray[11].ToString(),
                        Street = row.ItemArray[12].ToString(),
                        HouseNumber = row.ItemArray[13].ToString(),
                        PhoneNumber = row.ItemArray[14].ToString(),
                        IdSpeciality = Guid.Parse(row.ItemArray[15].ToString())
                    });
                }
            }
            OperationResult = server.LastError;
            if (!string.IsNullOrWhiteSpace(OperationResult)) return false;
            return true;
        }

        /// <summary>
        /// Метод для сохранения содержимого модели на сервере
        /// </summary>
        /// <param name="root">Ссылка на корневой объект модели</param>
        /// <param name="connection">Строка подключения</param>
        /// <returns></returns>
        public static bool StoreTables(Root root, string connection)
        {
            var server = new Database.SqlServer { Connection = connection };
            // предметы
            server.DeleteInto("Matters", "IdMatter", root.Matters.Select(item => item.IdMatter));
            OperationResult = server.LastError;
            if (!string.IsNullOrWhiteSpace(OperationResult)) return false;
            foreach (var item in root.Matters)
            {
                var columns = new Dictionary<string, string>
                {
                    { "IdMatter", item.IdMatter.ToString() },
                    { "Name", item.Name }
                };
                if (!server.InsertInto("Matters", columns)) server.UpdateInto("Matters", columns);
                OperationResult = server.LastError;
            }
            if (!string.IsNullOrWhiteSpace(OperationResult)) return false;
            // специальности
            server.DeleteInto("Specialities", "IdSpeciality", root.Specialities.Select(item => item.IdSpeciality));
            OperationResult = server.LastError;
            if (!string.IsNullOrWhiteSpace(OperationResult)) return false;
            foreach (var item in root.Specialities)
            {
                var columns = new Dictionary<string, string>
                {
                    { "IdSpeciality", item.IdSpeciality.ToString() },
                    { "Name", item.Name }
                };
                if (!server.InsertInto("Specialities", columns)) server.UpdateInto("Specialities", columns);
                OperationResult = server.LastError;
            }
            if (!string.IsNullOrWhiteSpace(OperationResult)) return false;
            // сдаваемые предметы
            server.DeleteInto("PassMatters", "IdPassMatter", root.PassMatters.Select(item => item.IdPassMatter));
            OperationResult = server.LastError;
            if (!string.IsNullOrWhiteSpace(OperationResult)) return false;
            foreach (var item in root.PassMatters)
            {
                var columns = new Dictionary<string, string>
                {
                    { "IdPassMatter", item.IdPassMatter.ToString() },
                    { "IdSpeciality", item.IdSpeciality.ToString() },
                    { "IdMatter", item.IdMatter.ToString() },
                    { "PassForm", item.PassForm.ToString() }
                };
                if (!server.InsertInto("PassMatters", columns)) server.UpdateInto("PassMatters", columns);
                OperationResult = server.LastError;
            }
            if (!string.IsNullOrWhiteSpace(OperationResult)) return false;
            // абитуриенты
            server.DeleteInto("Enrollees", "IdEnrollee", root.Enrollees.Select(item => item.IdEnrollee));
            OperationResult = server.LastError;
            if (!string.IsNullOrWhiteSpace(OperationResult)) return false;
            foreach (var item in root.Enrollees)
            {
                var columns = new Dictionary<string, string>
                {
                    { "IdEnrollee", item.IdEnrollee.ToString() },
                    { "RegistrationNumber", item.RegistrationNumber },
                    { "Surname", item.Surname },
                    { "FirstName", item.FirstName },
                    { "LastName", item.LastName },
                    { "BirthDay", item.BirthDay.ToString("O") },
                    { "SecodarySchoolName", item.SecodarySchoolName },
                    { "SecodarySchoolNumber", item.SecodarySchoolNumber },
                    { "SecodarySchoolTown", item.SecodarySchoolTown },
                    { "GraduationDate", item.GraduationDate.ToString("O") },
                    { "GoldMedal", item.GoldMedal.ToString() },
                    { "Town", item.Town },
                    { "Street", item.Street },
                    { "HouseNumber", item.HouseNumber },
                    { "PhoneNumber", item.PhoneNumber },
                    { "IdSpeciality", item.IdSpeciality.ToString() }
                };
                if (!server.InsertInto("Enrollees", columns)) server.UpdateInto("Enrollees", columns);
                OperationResult = server.LastError;
            }
            if (!string.IsNullOrWhiteSpace(OperationResult)) return false;
            return true;
        }
    }
}
