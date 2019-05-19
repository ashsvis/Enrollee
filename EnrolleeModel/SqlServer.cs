using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Database
{
    /// <summary>
    /// Класс для работы с базой данных SQL сервера
    /// </summary>
    public class SqlServer
    {
        public string Connection { get; set; } = string.Empty; // строка подключения
        public string LastError { get; set; } = string.Empty; // последняя ошибка

        /// <summary>
        /// Запрос на вставку данных
        /// </summary>
        /// <param name="table">Имя таблицы</param>
        /// <param name="row">Набор данных для вставки</param>
        /// <returns></returns>
        public bool InsertInto(string table, Dictionary<string, string> columns)
        {
            using (var con = new SqlConnection(Connection))
            {
                try
                {
                    con.Open();
                    // формирование запроса для вставки
                    var names = new List<string>();
                    var values = new List<string>();
                    foreach (var key in columns.Keys)
                    {
                        names.Add("[" + key + "]");
                        values.Add("N'" + columns[key] + "'");
                    }
                    var sql = string.Format("INSERT INTO [{0}] ({1}) VALUES({2})",
                            table.ToLower(), string.Join(", ", names), string.Join(", ", values));
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                    LastError = "";
                    return true;
                }
                catch (Exception ex)
                {
                    LastError = ex.Message;
                    return false;
                }
            }
        }

        /// <summary>
        /// Запрос на изменение данных
        /// </summary>
        /// <param name="table">Имя таблицы</param>
        /// <param name="columns">Набор данных для изменения</param>
        /// <returns></returns>
        public bool UpdateInto(string table, Dictionary<string, string> columns)
        {
            using (var con = new SqlConnection(Connection))
            {
                try
                {
                    con.Open();
                    // формирование запроса для изменения
                    var values = new List<string>();
                    var indexName = columns.Keys.First();
                    var indexValue = columns[indexName];
                    foreach (var key in columns.Keys.Skip(1))
                    {
                        values.Add("[" + key + "] = N'" + columns[key] + "'");
                    }
                    var sql = string.Format("UPDATE [{0}] SET {1} WHERE [{2}]=N'{3}'",
                            table.ToLower(), string.Join(", ", values), indexName, indexValue);
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                    LastError = "";
                    return true;
                }
                catch (Exception ex)
                {
                    LastError = ex.Message;
                    return false;
                }
            }
        }

        /// <summary>
        /// Удаление строки из таблицы
        /// </summary>
        /// <param name="table">Имя таблицы</param>
        /// <param name="columns">Данные ключа для удаления</param>
        /// <returns></returns>
        public bool DeleteInto(string table, Dictionary<string, string> columns)
        {
            using (var con = new SqlConnection(Connection))
            {
                try
                {
                    con.Open();
                    // формирование запроса для удаления
                    var indexName = columns.Keys.First();
                    var indexValue = columns[indexName];
                    var sql = string.Format("DELETE FROM [{0}] WHERE [{1}]=N'{2}'", table, indexName, indexValue);
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                    LastError = "";
                    return true;
                }
                catch (Exception ex)
                {
                    LastError = ex.Message;
                    return false;
                }
            }
        }

        /// <summary>
        /// Удаление записей таблицы, которых больше нет в модели, а есть в базе данных
        /// </summary>
        /// <param name="table">Имя таблицы</param>
        /// <param name="keyfield">Ключевой столбец</param>
        /// <param name="modelKeyList">Спискок ключей в модели</param>
        /// <returns></returns>
        public bool DeleteInto(string table, string keyfield, IEnumerable<Guid> modelKeyList)
        {
            var ds = GetRows(table);
            if (ds.Tables.Count == 0) return true;
            var fordelete = new List<Guid>(); // список ключевых значений для удаления
            // получаем ключи из базы данных
            foreach (var row in ds.Tables[0].Rows.Cast<DataRow>())
            {
                var key = Guid.Parse(row.ItemArray[0].ToString());
                // если ключ базы данных отсутствует в списке ключей модели
                if (!modelKeyList.Contains(key))
                    fordelete.Add(key); // то добавляем его в список
            }
            if (fordelete.Count == 0) return true;
            using (var con = new SqlConnection(Connection))
            {
                try
                {
                    con.Open();
                    // удаление выбранных ключей
                    foreach (var key in fordelete)
                    {
                        var sql = string.Format("DELETE FROM [{0}] WHERE [{1}]=N'{2}'", table, keyfield, key);
                        using (SqlCommand cmd = new SqlCommand(sql, con))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    con.Close();
                    LastError = "";
                    return true;
                }
                catch (Exception ex)
                {
                    LastError = ex.Message;
                    return false;
                }
            }
        }

        /// <summary>
        /// Получение набора данных из таблицы
        /// </summary>
        /// <param name="table">Имя таблицы</param>
        /// <param name="likefield">Имя поля для фильтра</param>
        /// <param name="text2find">Значение для фильтра</param>
        /// <returns></returns>
        public DataSet GetRows(string table, string likefield = null, string text2find = null)
        {
            using (var con = new SqlConnection(Connection))
            {
                var sql = BuildQuery(table, likefield, text2find);
                using (var da = new SqlDataAdapter(sql, con))
                {
                    var ds = new DataSet();
                    try
                    {
                        da.Fill(ds, table);
                        LastError = "";
                    }
                    catch (Exception ex)
                    {
                        LastError = ex.Message;
                    }
                    return ds;
                }
            }
        }

        /// <summary>
        /// Построение текста запроса SELECT
        /// </summary>
        /// <param name="table">Имя таблицы</param>
        /// <param name="likefield">Имя поля для фильтра</param>
        /// <param name="text2find">Значение для фильтра</param>
        /// <returns></returns>
        private string BuildQuery(string table, string likefield = null, string text2find = null)
        {
            var sql = string.Format("SELECT * FROM [{0}]", table);
            if (!string.IsNullOrWhiteSpace(likefield) && !string.IsNullOrWhiteSpace(text2find))
                sql += string.Format(" WHERE ([{0}] LIKE N'{1}%')", likefield, text2find);

            return sql;
        }

    }
}
