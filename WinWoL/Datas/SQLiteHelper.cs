using Microsoft.Data.Sqlite;
using PInvoke;
using System;
using System.Collections.Generic;
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
                CreateTableIfNotExists();
                UpgradeDatabaseVersion();
            }
        }
        // 建表
        public void CreateTableIfNotExists()
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // 创建WoL信息表
                var createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = "CREATE TABLE IF NOT EXISTS WoLTable (Id INTEGER PRIMARY KEY, Name TEXT, MacAddress TEXT, IPAddress TEXT, WoLAddress TEXT, WoLPort TEXT, RDPPort TEXT, SSHCommand TEXT, SSHPort TEXT, SSHUser TEXT, SSHKeyPath TEXT, WoLIsOpen TEXT, RDPIsOpen TEXT, SSHIsOpen TEXT, BroadcastIsOpen TEXT, SSHKeyIsOpen TEXT)";
                createTableCommand.ExecuteNonQuery();

                // 创建SSH信息表
                var createSSHTableCommand = connection.CreateCommand();
                createSSHTableCommand.CommandText = "CREATE TABLE IF NOT EXISTS SSHTable (Id INTEGER PRIMARY KEY, Name TEXT, IPAddress TEXT, SSHCommand TEXT, SSHPort TEXT, SSHUser TEXT, SSHKeyPath TEXT, SSHKeyIsOpen TEXT)";
                createSSHTableCommand.ExecuteNonQuery();

                // 创建版本表
                var createTableCommand2 = connection.CreateCommand();
                createTableCommand2.CommandText = "CREATE TABLE IF NOT EXISTS Version (VersionNumber INTEGER)";
                createTableCommand2.ExecuteNonQuery();
            }
        }
        // 删表
        public void DropTable()
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var dropTableCommand = connection.CreateCommand();
                dropTableCommand.CommandText = $"DROP TABLE IF EXISTS WoLTable;";
                dropTableCommand.ExecuteNonQuery();

                var dropSSHTableCommand = connection.CreateCommand();
                dropSSHTableCommand.CommandText = $"DROP TABLE IF EXISTS SSHTable;";
                dropSSHTableCommand.ExecuteNonQuery();

                var dropTableCommand2 = connection.CreateCommand();
                dropTableCommand2.CommandText = $"DROP TABLE IF EXISTS Version;";
                dropTableCommand2.ExecuteNonQuery();
            }
        }
        // 检查数据库版本
        public int GetDatabaseVersion()
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.CommandText = "SELECT VersionNumber FROM Version";
                    var result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int version))
                    {
                        return version;
                    }
                    // 如果没有版本信息，返回-1
                    return -1;
                }
            }
        }

        // 更新数据库版本信息
        public void UpgradeDatabaseVersion()
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                // 检查版本，新建数据库要插入版本号
                var cmd = connection.CreateCommand();
                cmd.Parameters.AddWithValue("@VersionNumber", 1);
                if (GetDatabaseVersion() == -1)
                {
                    cmd.CommandText = "INSERT INTO Version (VersionNumber) VALUES (@VersionNumber)";
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    cmd.CommandText = "UPDATE Version SET VersionNumber = @VersionNumber";
                    cmd.ExecuteNonQuery();

                }
            }

        }

        // 数据库升级
        public void UpgradeDatabase()
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                int currentVersion = GetDatabaseVersion();

                // 检查当前数据库版本并执行升级操作
                if (currentVersion < 1)
                {
                    // 执行升级操作，例如添加新字段
                    using (var cmd = connection.CreateCommand())
                    {
                        //cmd.CommandText = "ALTER TABLE WoLTable ADD COLUMN SSHKeyIsOpen TEXT";
                        //cmd.ExecuteNonQuery();
                    }

                    // 更新数据库版本信息
                    UpgradeDatabaseVersion();
                }
            }
        }

        // 插入数据
        public void InsertData(WoLModel model)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                UpgradeDatabase();

                var insertCommand = connection.CreateCommand();
                insertCommand.CommandText = "INSERT INTO WoLTable (Name, MacAddress, IPAddress, WoLAddress, WoLPort, RDPPort, SSHCommand, SSHPort, SSHUser, SSHKeyPath, WoLIsOpen, RDPIsOpen, SSHIsOpen, BroadcastIsOpen, SSHKeyIsOpen) VALUES (@Name, @MacAddress, @IPAddress, @WoLAddress, @WoLPort, @RDPPort, @SSHCommand, @SSHPort, @SSHUser, @SSHKeyPath, @WoLIsOpen, @RDPIsOpen, @SSHIsOpen, @BroadcastIsOpen, @SSHKeyIsOpen)";

                insertCommand.Parameters.AddWithValue("@Name", model.Name ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@MacAddress", model.MacAddress ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@IPAddress", model.IPAddress ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@WoLAddress", model.WoLAddress ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@WoLPort", model.WoLPort ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@RDPPort", model.RDPPort ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@SSHCommand", model.SSHCommand ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@SSHPort", model.SSHPort ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@SSHUser", model.SSHUser ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@SSHKeyPath", model.SSHKeyPath ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@WoLIsOpen", model.WoLIsOpen ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@RDPIsOpen", model.RDPIsOpen ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@SSHIsOpen", model.SSHIsOpen ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@BroadcastIsOpen", model.BroadcastIsOpen ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@SSHKeyIsOpen", model.SSHKeyIsOpen ?? (object)DBNull.Value);

                insertCommand.ExecuteNonQuery();
            }
        }
        public void InsertSSHData(SSHModel model)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                UpgradeDatabase();

                var insertCommand = connection.CreateCommand();
                insertCommand.CommandText = "INSERT INTO SSHTable (Name, IPAddress, SSHCommand, SSHPort, SSHUser, SSHKeyPath, SSHKeyIsOpen) VALUES (@Name, @IPAddress, @SSHCommand, @SSHPort, @SSHUser, @SSHKeyPath, @SSHKeyIsOpen)";

                insertCommand.Parameters.AddWithValue("@Name", model.Name ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@IPAddress", model.IPAddress ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@SSHCommand", model.SSHCommand ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@SSHPort", model.SSHPort ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@SSHUser", model.SSHUser ?? (object)DBNull.Value);
                insertCommand.Parameters.AddWithValue("@SSHKeyPath", model.SSHKeyPath ?? (object)DBNull.Value);
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
                UpgradeDatabase();

                var deleteCommand = connection.CreateCommand();
                deleteCommand.CommandText = "DELETE FROM WoLTable WHERE Id = @Id";
                deleteCommand.Parameters.AddWithValue("@Id", model.Id);

                deleteCommand.ExecuteNonQuery();
            }
        }
        public void DeleteSSHData(SSHModel model)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                UpgradeDatabase();

                var deleteCommand = connection.CreateCommand();
                deleteCommand.CommandText = "DELETE FROM SSHTable WHERE Id = @Id";
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
                UpgradeDatabase();

                var updateCommand = connection.CreateCommand();
                updateCommand.CommandText = "UPDATE WoLTable SET Name = @Name, MacAddress = @MacAddress, IPAddress = @IPAddress, WoLAddress = @WoLAddress, WoLPort = @WoLPort, RDPPort = @RDPPort, SSHCommand = @SSHCommand, SSHPort = @SSHPort, SSHUser = @SSHUser, SSHKeyPath = @SSHKeyPath, WoLIsOpen = @WoLIsOpen, RDPIsOpen = @RDPIsOpen, SSHIsOpen = @SSHIsOpen, BroadcastIsOpen = @BroadcastIsOpen, SSHKeyIsOpen = @SSHKeyIsOpen WHERE Id = @Id";

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
                updateCommand.Parameters.AddWithValue("@SSHKeyPath", model.SSHKeyPath ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@WoLIsOpen", model.WoLIsOpen ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@RDPIsOpen", model.RDPIsOpen ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@SSHIsOpen", model.SSHIsOpen ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@BroadcastIsOpen", model.BroadcastIsOpen ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@SSHKeyIsOpen", model.SSHKeyIsOpen ?? (object)DBNull.Value);

                updateCommand.ExecuteNonQuery();
            }
        }
        public void UpdateSSHData(SSHModel model)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                UpgradeDatabase();

                var updateCommand = connection.CreateCommand();
                updateCommand.CommandText = "UPDATE SSHTable SET Name = @Name, IPAddress = @IPAddress, SSHCommand = @SSHCommand, SSHPort = @SSHPort, SSHUser = @SSHUser, SSHKeyPath = @SSHKeyPath, SSHKeyIsOpen = @SSHKeyIsOpen WHERE Id = @Id";

                updateCommand.Parameters.AddWithValue("@Id", model.Id);
                updateCommand.Parameters.AddWithValue("@Name", model.Name ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@IPAddress", model.IPAddress ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@SSHCommand", model.SSHCommand ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@SSHPort", model.SSHPort ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@SSHUser", model.SSHUser ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@SSHKeyPath", model.SSHKeyPath ?? (object)DBNull.Value);
                updateCommand.Parameters.AddWithValue("@SSHKeyIsOpen", model.SSHKeyIsOpen ?? (object)DBNull.Value);

                updateCommand.ExecuteNonQuery();
            }
        }
        // 根据ID获得数据
        public WoLModel GetDataById(int id)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                UpgradeDatabase();

                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = "SELECT * FROM WoLTable WHERE Id = @Id";
                selectCommand.Parameters.AddWithValue("@Id", id);

                using (SqliteDataReader reader = selectCommand.ExecuteReader())
                {
                    if (reader.Read())
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
                            SSHKeyPath = reader.IsDBNull(10) ? "" : reader.GetString(10),
                            WoLIsOpen = reader.IsDBNull(11) ? "" : reader.GetString(11),
                            RDPIsOpen = reader.IsDBNull(12) ? "" : reader.GetString(12),
                            SSHIsOpen = reader.IsDBNull(13) ? "" : reader.GetString(13),
                            BroadcastIsOpen = reader.IsDBNull(14) ? "" : reader.GetString(14),
                            SSHKeyIsOpen = reader.IsDBNull(15) ? "" : reader.GetString(15)
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
                UpgradeDatabase();

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
                            SSHKeyPath = reader.IsDBNull(10) ? "" : reader.GetString(10),
                            WoLIsOpen = reader.IsDBNull(11) ? "" : reader.GetString(11),
                            RDPIsOpen = reader.IsDBNull(12) ? "" : reader.GetString(12),
                            SSHIsOpen = reader.IsDBNull(13) ? "" : reader.GetString(13),
                            BroadcastIsOpen = reader.IsDBNull(14) ? "" : reader.GetString(14),
                            SSHKeyIsOpen = reader.IsDBNull(15) ? "" : reader.GetString(15)
                        };
                        entries.Add(entry);
                    }
                }
            }

            return entries;
        }
        public List<WoLModel> GetDataListByIdHideAddress(int id)
        {
            List<WoLModel> entries = new List<WoLModel>();

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                UpgradeDatabase();

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
                            MacAddress = "**:**:**:**",
                            IPAddress = "***.***.***.***",
                            WoLAddress = "***.***.***.***",
                            WoLPort = "*",
                            RDPPort = "****",
                            SSHCommand = "*******",
                            SSHPort = "**",
                            SSHUser = "*",
                            SSHKeyPath = "*",
                            WoLIsOpen = reader.IsDBNull(11) ? "" : reader.GetString(11),
                            RDPIsOpen = reader.IsDBNull(12) ? "" : reader.GetString(12),
                            SSHIsOpen = reader.IsDBNull(13) ? "" : reader.GetString(13),
                            BroadcastIsOpen = reader.IsDBNull(14) ? "" : reader.GetString(14),
                            SSHKeyIsOpen = reader.IsDBNull(15) ? "" : reader.GetString(15)
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
                UpgradeDatabase();

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
                            SSHKeyPath = reader.IsDBNull(10) ? "" : reader.GetString(10),
                            WoLIsOpen = reader.IsDBNull(11) ? "" : reader.GetString(11),
                            RDPIsOpen = reader.IsDBNull(12) ? "" : reader.GetString(12),
                            SSHIsOpen = reader.IsDBNull(13) ? "" : reader.GetString(13),
                            BroadcastIsOpen = reader.IsDBNull(14) ? "" : reader.GetString(14),
                            SSHKeyIsOpen = reader.IsDBNull(15) ? "" : reader.GetString(15)
                        };
                        entries.Add(entry);
                    }
                }
            }

            return entries;
        }
        public List<SSHModel> QuerySSHData()
        {
            List<SSHModel> entries = new List<SSHModel>();

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                UpgradeDatabase();

                var queryCommand = connection.CreateCommand();
                queryCommand.CommandText = "SELECT * FROM SSHTable";

                using (SqliteDataReader reader = queryCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SSHModel entry = new SSHModel
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            IPAddress = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            SSHCommand = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            SSHPort = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            SSHUser = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            SSHKeyPath = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            SSHKeyIsOpen = reader.IsDBNull(7) ? "" : reader.GetString(7)
                        };
                        entries.Add(entry);
                    }
                }
            }

            return entries;
        }
        // 获取上一行的ID
        public int GetPreRowsId(WoLModel wolModel)
        {
            int srcId = wolModel.Id;
            int preId = -1; // 默认值，表示未找到上一行的ID
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    // 获取上一行的ID
                    command.CommandText = "SELECT MAX(Id) FROM WoLTable WHERE Id < @srcId";
                    command.Parameters.AddWithValue("@srcId", srcId);

                    var result = command.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        preId = Convert.ToInt32(result);
                    }
                }
            }
            return preId;
        }
        // 向上移动项
        public bool UpSwapRows(WoLModel wolModel)
        {
            int srcId = wolModel.Id;
            int preId = GetPreRowsId(wolModel);
            if (preId >= 0)
            {
                // 新建两个对象，分别获取两行数据
                WoLModel srcModel = GetDataById(srcId);
                WoLModel preModel = GetDataById(preId);

                // 重新指定ID，交换
                srcModel.Id = preId;
                preModel.Id = srcId;

                // 交换写入对方的位置
                UpdateData(srcModel);
                UpdateData(preModel);
                return true;
            }
            return false;
        }
        // 获取下一行的ID
        public int GetPosRowsId(WoLModel wolModel)
        {
            int srcId = wolModel.Id;
            int posId = -1; // 默认值，表示未找到上一行的ID
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    // 获取上一行的ID
                    command.CommandText = "SELECT MIN(Id) FROM WoLTable WHERE Id > @srcId";
                    command.Parameters.AddWithValue("@srcId", srcId);

                    var result = command.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        posId = Convert.ToInt32(result);
                    }
                }
            }
            return posId;
        }
        // 向下移动项
        public bool DownSwapRows(WoLModel wolModel)
        {
            int srcId = wolModel.Id;
            int posId = GetPosRowsId(wolModel);
            if (posId >= 0)
            {
                // 新建两个对象，分别获取两行数据
                WoLModel srcModel = GetDataById(srcId);
                WoLModel preModel = GetDataById(posId);

                // 重新指定ID，交换
                srcModel.Id = posId;
                preModel.Id = srcId;

                // 交换写入对方的位置
                UpdateData(srcModel);
                UpdateData(preModel);
                return true;
            }
            return false;
        }
    }

}
