using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging.Abstractions;
using Renci.SshNet;
using Validation;
using WinWoL.Models;

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

                // 创建WoL信息表
                var createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = "CREATE TABLE IF NOT EXISTS WoLTable (" +
                "Id INTEGER PRIMARY KEY, " +
                "Name TEXT, " +
                "MacAddress TEXT, " +
                "IPAddress TEXT, " +
                "WoLAddress TEXT, " +
                "WoLPort TEXT, " +
                "RDPPort TEXT, " +
                "SSHCommand TEXT, " +
                "SSHPort TEXT, " +
                "SSHUser TEXT, " +
                "SSHPasswd TEXT, " +
                "SSHKeyPath TEXT, " +
                "WoLIsOpen TEXT, " +
                "RDPIsOpen TEXT, " +
                "SSHIsOpen TEXT, " +
                "BroadcastIsOpen TEXT, " +
                "SSHKeyIsOpen TEXT" +
                ")";
                createTableCommand.ExecuteNonQuery();

                // 创建版本表
                var createTableCommand2 = connection.CreateCommand();
                createTableCommand2.CommandText = "CREATE TABLE IF NOT EXISTS Version (VersionNumber INTEGER)";
                createTableCommand2.ExecuteNonQuery();
            }
        }

        // 插入数据
        public void InsertData(WoLModel model)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var insertCommand = connection.CreateCommand();
                insertCommand.CommandText = "INSERT INTO WoLTable (Name, MacAddress, IPAddress, WoLAddress, WoLPort, RDPPort, SSHCommand, SSHPort, SSHUser, SSHPasswd, SSHKeyPath, WoLIsOpen, RDPIsOpen, SSHIsOpen, BroadcastIsOpen， SSHKeyIsOpen) " +
                    "VALUES (@Name, @MacAddress, @IPAddress, @WoLAddress, @WoLPort, @RDPPort, @SSHCommand, @SSHPort, @SSHUser, @SSHPasswd, @SSHKeyPath, @WoLIsOpen, @RDPIsOpen, @SSHIsOpen, @BroadcastIsOpen, @SSHKeyIsOpen)";

                insertCommand.Parameters.AddWithValue("@Name", model.Name ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@MacAddress", model.MacAddress ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@IPAddress", model.IPAddress ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@WoLAddress", model.WoLAddress ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@WoLPort", model.WoLPort ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@RDPPort", model.RDPPort ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@SSHCommand", model.SSHCommand ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@SSHPort", model.SSHPort ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@SSHUser", model.SSHUser ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@SSHPasswd", model.SSHPasswd ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@SSHKeyPath", model.SSHKeyPath ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@WoLIsOpen", model.WoLIsOpen ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@RDPIsOpen", model.RDPIsOpen ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@SSHIsOpen", model.SSHIsOpen ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@BroadcastIsOpen", model.BroadcastIsOpen ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@SSHKeyIsOpen", model.SSHKeyIsOpen ?? (object)DBNull.Value);

                insertCommand.ExecuteNonQuery();
            }
        }

        // 删除数据
        public void DeleteData(WoLModel model)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var deleteCommand = connection.CreateCommand();
                deleteCommand.CommandText = "DELETE FROM WoLTable WHERE Id = @Id";
                deleteCommand.Parameters.AddWithValue("@Id", model.Id);

                deleteCommand.ExecuteNonQuery();
            }
        }

        // 更新数据
        public void UpdateData(WoLModel model)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var updateCommand = connection.CreateCommand();
                updateCommand.CommandText = "UPDATE WoLTable SET Name = @Name, MacAddress = @MacAddress, IPAddress = @IPAddress, WoLAddress = @WoLAddress, WoLPort = @WoLPort, RDPPort = @RDPPort, SSHCommand = @SSHCommand, SSHPort = @SSHPort, SSHUser = @SSHUser, SSHPasswd = @SSHPasswd, SSHKeyPath = @SSHKeyPath, WoLIsOpen = @WoLIsOpen, RDPIsOpen = @RDPIsOpen, SSHIsOpen = @SSHIsOpen, BroadcastIsOpen = @BroadcastIsOpen, SSHKeyIsOpen = @SSHKeyIsOpen WHERE Id = @Id";

                updateCommand.Parameters.AddWithValue("@Id", model.Id);
                updateCommand.Parameters.AddWithValue("@Name", model.Name ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@MacAddress", model.MacAddress ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@IPAddress", model.IPAddress ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@WoLAddress", model.WoLAddress ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@WoLPort", model.WoLPort ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@RDPPort", model.RDPPort ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@SSHCommand", model.SSHCommand ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@SSHPort", model.SSHPort ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@SSHUser", model.SSHUser ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@SSHPasswd", model.SSHPasswd ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@SSHKeyPath", model.SSHKeyPath ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@WoLIsOpen", model.WoLIsOpen ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@RDPIsOpen", model.RDPIsOpen ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@SSHIsOpen", model.SSHIsOpen ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@BroadcastIsOpen", model.BroadcastIsOpen ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@SSHKeyIsOpen", model.SSHKeyIsOpen ?? (object)DBNull.Value);

                updateCommand.ExecuteNonQuery();
            }
        }
        // 根据ID获得数据
        public WoLModel GetDataById(WoLModel model)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = "SELECT * FROM WoLTable WHERE Id = @Id";
                selectCommand.Parameters.AddWithValue("@Id", model.Id);

                using (SqliteDataReader reader = selectCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        WoLModel entry = new WoLModel
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                            MacAddress = reader.IsDBNull(2) ? null : reader.GetString(2),
                            IPAddress = reader.IsDBNull(3) ? null : reader.GetString(3),
                            WoLAddress = reader.IsDBNull(4) ? null : reader.GetString(4),
                            WoLPort = reader.IsDBNull(5) ? null : reader.GetString(5),
                            RDPPort = reader.IsDBNull(6) ? null : reader.GetString(6),
                            SSHCommand = reader.IsDBNull(7) ? null : reader.GetString(7),
                            SSHPort = reader.IsDBNull(8) ? null : reader.GetString(8),
                            SSHUser = reader.IsDBNull(9) ? null : reader.GetString(9),
                            SSHPasswd = reader.IsDBNull(10) ? null : reader.GetString(10),
                            SSHKeyPath = reader.IsDBNull(11) ? null : reader.GetString(11),
                            WoLIsOpen = reader.IsDBNull(12) ? null : reader.GetString(12),
                            RDPIsOpen = reader.IsDBNull(13) ? null : reader.GetString(13),
                            SSHIsOpen = reader.IsDBNull(14) ? null : reader.GetString(14),
                            BroadcastIsOpen = reader.IsDBNull(15) ? null : reader.GetString(15),
                            SSHKeyIsOpen = reader.IsDBNull(16) ? null : reader.GetString(16)
                        };

                        return entry;
                    }
                }
            }

            // 如果未找到匹配的数据，可以返回null或者抛出异常，具体根据需求来决定
            return null;
        }

        public List<WoLModel> GetDataListById(int id)
        {
            List<WoLModel> entries = new List<WoLModel>();

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var queryCommand = connection.CreateCommand();
                queryCommand.CommandText = "SELECT * FROM WoLTable WHERE Id = @Id";
                queryCommand.Parameters.AddWithValue("@Id", id);

                using (SqliteDataReader reader = queryCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        WoLModel entry = new WoLModel
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            MacAddress = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            IPAddress = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            WoLAddress = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            WoLPort = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            RDPPort = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            SSHCommand = reader.IsDBNull(7) ? "" : reader.GetString(7),
                            SSHPort = reader.IsDBNull(8) ? "" : reader.GetString(8),
                            SSHUser = reader.IsDBNull(9) ? "" : reader.GetString(9),
                            SSHPasswd = reader.IsDBNull(10) ? "" : reader.GetString(10),
                            SSHKeyPath = reader.IsDBNull(11) ? "" : reader.GetString(11),
                            WoLIsOpen = reader.IsDBNull(12) ? "" : reader.GetString(12),
                            RDPIsOpen = reader.IsDBNull(13) ? "" : reader.GetString(13),
                            SSHIsOpen = reader.IsDBNull(14) ? "" : reader.GetString(14),
                            BroadcastIsOpen = reader.IsDBNull(15) ? "" : reader.GetString(15),
                            SSHKeyIsOpen = reader.IsDBNull(16) ? "" : reader.GetString(16)
                        };
                        entries.Add(entry);
                    }
                }
            }

            return entries;
        }

        // 查询数据
        public List<WoLModel> QueryData()
        {
            List<WoLModel> entries = new List<WoLModel>();

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var queryCommand = connection.CreateCommand();
                queryCommand.CommandText = "SELECT * FROM WoLTable";

                using (SqliteDataReader reader = queryCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        WoLModel entry = new WoLModel
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            MacAddress = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            IPAddress = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            WoLAddress = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            WoLPort = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            RDPPort = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            SSHCommand = reader.IsDBNull(7) ? "" : reader.GetString(7),
                            SSHPort = reader.IsDBNull(8) ? "" : reader.GetString(8),
                            SSHUser = reader.IsDBNull(9) ? "" : reader.GetString(9),
                            SSHPasswd = reader.IsDBNull(10) ? "" : reader.GetString(10),
                            SSHKeyPath = reader.IsDBNull(11) ? "" : reader.GetString(11),
                            WoLIsOpen = reader.IsDBNull(12) ? "" : reader.GetString(12),
                            RDPIsOpen = reader.IsDBNull(13) ? "" : reader.GetString(13),
                            SSHIsOpen = reader.IsDBNull(14) ? "" : reader.GetString(14),
                            BroadcastIsOpen = reader.IsDBNull(15) ? "" : reader.GetString(15),
                            SSHKeyIsOpen = reader.IsDBNull(16) ? "" : reader.GetString(16)
                        };
                        entries.Add(entry);
                    }
                }
            }

            return entries;
        }
    }

}
