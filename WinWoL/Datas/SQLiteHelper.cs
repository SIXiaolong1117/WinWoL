using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace WinWoL.Datas
{
    public class SQLiteHelper
    {
        private string connectionString = "Data Source=wol.db";

        public SQLiteHelper()
        {
            // 初始化数据库连接
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // 创建示例表
                var createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = "CREATE TABLE IF NOT EXISTS MyTable (Id INTEGER PRIMARY KEY, Name TEXT)";
                createTableCommand.ExecuteNonQuery();
            }
        }

        public void InsertData(string name)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var insertCommand = connection.CreateCommand();
                insertCommand.CommandText = "INSERT INTO MyTable (Name) VALUES (@Name)";
                insertCommand.Parameters.AddWithValue("@Name", name);
                insertCommand.ExecuteNonQuery();
            }
        }

        public List<string> ReadData()
        {
            List<string> results = new List<string>();

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = "SELECT Name FROM MyTable";

                using (SqliteDataReader reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        results.Add(name);
                    }
                }
            }

            return results;
        }
    }

}
