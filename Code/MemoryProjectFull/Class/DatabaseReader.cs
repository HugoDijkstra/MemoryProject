using System;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;
using NotificationsWPF;

namespace MemoryProjectFull
{
    /// <summary>
    /// 
    /// </summary>
    public static class MemoryDatabase {
        public static DatabaseReader database;
        public static void init() {
            reload();
        }

        public static void reload() {
            try
            {
                database = new DatabaseReader("185.216.163.49", "8000", "database", "root", "root");

                if (isConnected() && !MemoryDatabase.database.CheckTableExistence("users"))
                {
                    SortedList<string, DatabaseReader.MySqlDataType> paramList = new SortedList<string, DatabaseReader.MySqlDataType>();
                    paramList.Add("id", DatabaseReader.MySqlDataType.Float);
                    paramList.Add("name", DatabaseReader.MySqlDataType.Text);
                    paramList.Add("password", DatabaseReader.MySqlDataType.Text);
                    paramList.Add("wins", DatabaseReader.MySqlDataType.Float);
                    paramList.Add("loses", DatabaseReader.MySqlDataType.Float);
                    MemoryDatabase.database.CreateTable("users", paramList);
                }
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public static bool isConnected() {
            return database._connected;
        }
    }

    /// <summary>
    /// Reader for databases
    /// </summary>
    public class DatabaseReader
    {
        /// <summary>
        /// The active connection
        /// </summary>
        private MySqlConnection _Connection;
        public bool _connected;

        /// <summary>
        /// The constructor for the database reader object
        /// </summary>
        /// <param name="IPadress">Ip/URL for the database</param>
        /// <param name="DatabaseName">Name of the targeted database</param>
        /// <param name="username">Username for the databse</param>
        /// <param name="password">Password for the database</param>
        public DatabaseReader(string IPadress, string Port, string DatabaseName, string username, string password)
        {
            _Connection = new MySqlConnection(
                "user id=" + username + ";" +
                "port=" + Port + ";" +
                "password=" + password + ";" +
                "server=" + IPadress + ";" +
                "DATABASE=" + DatabaseName + ";" +
                "connection timeout=5"
                );
            Console.WriteLine("Starting connection");

            try{
                _Connection.Open();
                _connected = true;
            }
            catch (Exception){
                NotificationManager.RequestNotification("Could not connect to game servers, please try againor contact out support!");
                _connected = false;
                throw;
            }

            Console.WriteLine("connection made");
        }

        /// <summary>
        /// Get data from the table
        /// </summary>
        /// <param name="table">table to get from</param>
        /// <param name="column">column to get from</param>
        /// <returns></returns>
        public string GetDataFromTable(string table, string column)
        {
            if (!_connected)
                return "";

            string command = "SELECT * FROM " + table;
            MySqlCommand sqlCommand = new MySqlCommand(command, _Connection);

            string returnvalue = "";

            using (MySqlDataReader dataReader = sqlCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    returnvalue += dataReader[column] + " , ";
                }
            }
            return returnvalue;
        }

        /// <summary>
        /// Get certain info from a table
        /// </summary>
        /// <param name="table">Get values from the table </param>
        /// <param name="column">Get the value from the tabble from column</param>
        /// <param name="Where">The variable you want to check the value for</param>
        /// <param name="WhareIs">the value you want to check against</param>
        /// <returns>the data from the table</returns>
        public string GetDataFromTableFilter(string table, string column, string Where, string WhereIs)
        {
            if (!_connected)
                return "";

            string command = "SELECT * FROM " + table + " Where " + Where + " = " + WhereIs;
            MySqlCommand sqlCommand = new MySqlCommand(command, _Connection);

            string returnvalue = "";

            using (MySqlDataReader dataReader = sqlCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    returnvalue += dataReader[column] + " , ";
                }
            }
            return returnvalue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="Where"></param>
        /// <returns>the data from the table</returns>
        public string GetDataFromTableFilter(string table, string Where)
        {
            if (!_connected)
                return "";

            string command = "SELECT * FROM " + table + " Where " + Where;
            MySqlCommand sqlCommand = new MySqlCommand(command, _Connection);

            string returnvalue = "";

            using (MySqlDataReader dataReader = sqlCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        returnvalue += dataReader[i] + " , ";
                    }
                }
            }
            return returnvalue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="Where"></param>
        /// <returns></returns>
        public bool TableContainsData(string table, string Where)
        {
            if (!_connected)
                return false;

            string command = "SELECT * FROM " + table + " Where " + Where;
            MySqlCommand sqlCommand = new MySqlCommand(command, _Connection);

            using (MySqlDataReader dataReader = sqlCommand.ExecuteReader())
            {
                return dataReader.FieldCount > 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="Where"></param>
        /// <param name="WhereIs"></param>
        /// <returns></returns>
        public bool TableContainsData(string table, string Where, string WhereIs)
        {
            if (!_connected)
                return false;

            string command = "SELECT * FROM " + table + " Where " + Where + "=" + WhereIs;
            MySqlCommand sqlCommand = new MySqlCommand(command, _Connection);

            using (MySqlDataReader dataReader = sqlCommand.ExecuteReader())
            {
                return dataReader.HasRows;
            }
        }


        /// <summary>
        /// Get all data from a table
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public string GetDataFromTable(string table)
        {
            if (!_connected)
                return "";

            string command = "SELECT * FROM " + table;
            MySqlCommand sqlCommand = new MySqlCommand(command, _Connection);

            string returnvalue = "";

            using (MySqlDataReader dataReader = sqlCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        returnvalue += dataReader[i] + " , ";
                    }
                }
            }
            return returnvalue;
        }

        /// <summary>
        /// Add data to the database
        /// </summary>
        /// <param name="table">The table to add to</param>
        /// <param name="values">The values sorted in a list[name,value]</param>
        /// <example>
        /// SortedList[string, string] values = new SortedList[string, string]();
        /// values.Add("kiss", "10");
        /// AddDataToTable("darp", values);
        ///</example>
        public void AddDataToTable(string table, SortedList<string, string> values)
        {
            if (!_connected)
                return;

            string command = "INSERT INTO `" + table + "` (";

            for (int i = 0; i < values.Count; i++)
            {
                command += "`" + values.Keys[i] + "`";
                if (i < values.Count - 1)
                    command += ',';
            }
            command += ") VALUES (";

            for (int i = 0; i < values.Count; i++)
            {
                command += "'" + values.Values[i] + "'";
                if (i < values.Count - 1)
                    command += ',';
            }
            command += ");";

            MySqlCommand sqlCommand = new MySqlCommand(command, _Connection);
            sqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="column"></param>
        /// <param name="values"></param>
        public void UpdateDataToTable(string table, string column, SortedList<string, string> values)
        {
            if (!_connected)
                return;

            string command = "UPDATE " + table + " SET ";

            for (int i = 0; i < values.Count; i++)
            {
                command += "`" + values.Keys[i] + "` = " + values.Values[i];
                if (i < values.Count - 1)
                    command += ',';
            }

            MySqlCommand sqlCommand = new MySqlCommand(command, _Connection);
            sqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="where"></param>
        /// <param name="values"></param>
        public void UpdateDataToTableFilter(string table, string where, SortedList<string, string> values)
        {
            if (!_connected)
                return;

            string command = "UPDATE " + table + " SET ";

            for (int i = 0; i < values.Count; i++)
            {
                command += "`" + values.Keys[i] + "` = " + values.Values[i];
                if (i < values.Count - 1)
                    command += ',';
            }

            command += " WHERE " + where;

            MySqlCommand sqlCommand = new MySqlCommand(command, _Connection);
            sqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Check if certain table exists
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <returns> if table exists</returns>
        public bool CheckTableExistence(string tableName)
        {
            if (!_connected)
                return false;

            MySqlCommand command = new MySqlCommand("SHOW TABLES LIKE '" + tableName + "';", _Connection);
            try
            {
                using (MySqlDataReader dataReader = command.ExecuteReader())
                {
                    return dataReader.HasRows;
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        /// <summary>
        /// Create a table
        /// </summary>
        /// <param name="tableName">Name for the table</param>
        /// <param name="NamesAndTypes">Names and types</param>
        /// SortedList[string, DatabaseReader.MySqlDataType] columnNames = new SortedList[string, DatabaseReader.MySqlDataType]();
        /// columnNames.Add("name", DatabaseReader.MySqlDataType.Text);
        /// columnNames.Add("age", DatabaseReader.MySqlDataType.Integer);
        /// columnNames.Add("goodboy", DatabaseReader.MySqlDataType.Boolean);
        /// reader.CreateTable("goodboys", columnNames);
        public void CreateTable(string tableName, SortedList<string, MySqlDataType> NamesAndTypes)
        {
            if (!_connected)
                return;

            string command = "CREATE TABLE `" + tableName + "` (";
            for (int i = 0; i < NamesAndTypes.Count; i++)
            {
                command += '`' + NamesAndTypes.Keys[i] + "` ";

                switch (NamesAndTypes.Values[i])
                {
                    case MySqlDataType.Boolean:
                        command += "BOOLEAN NOT NULL";
                        break;
                    case MySqlDataType.Float:
                        command += "FLOAT NOT NULL";
                        break;
                    case MySqlDataType.Integer:
                        command += "INT NOT NULL";
                        break;
                    case MySqlDataType.Text:
                        command += "TEXT NOT NULL";
                        break;
                }

                if (i < NamesAndTypes.Count - 1)
                    command += ", ";
            }
            command += ") ENGINE = InnoDB";
            MySqlCommand sqlCommand = new MySqlCommand(command, _Connection);
            sqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// SQL Data type
        /// </summary>
        public enum MySqlDataType
        {
            Text,
            Integer,
            Float,
            Boolean
        }
    }

}