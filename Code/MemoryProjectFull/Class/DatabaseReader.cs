using System;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace MemoryProjectFull
{

    public static class MemoryDatabase {
        public static DatabaseReader database;
        public static void init() {
            database = new DatabaseReader("185.216.163.49", "8000", "database", "root", "root");
        }
    }

    public class DatabaseReader
    {
        /// <summary>
        /// The active connection
        /// </summary>
        private MySqlConnection _Connection;

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
            _Connection.Open();
            Console.WriteLine("connection made");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        public void SendNonReturnCommand(string command)
        {
            MySqlCommand sqlCommand = new MySqlCommand(command, _Connection);
            sqlCommand.ExecuteNonQuery();
        }

        public string GetDataFromTable(string table, string column)
        {
            string command = "SELECT * FROM " + table;
            MySqlCommand sqlCommand = new MySqlCommand(command, _Connection);

            string returnvalue = "";

            MySqlDataReader dataReader = sqlCommand.ExecuteReader();
            while (dataReader.Read())
            {
                returnvalue += dataReader[column] + " , ";
            }
            dataReader.Close();
            return returnvalue;
        }

        /// <summary>
        /// Get certain info from a table
        /// </summary>
        /// <param name="table">Get values from the table </param>
        /// <param name="column">Get the value from the tabble from column</param>
        /// <param name="Where">The variable you want to check the value for</param>
        /// <param name="WhareIs">the value you want to check against</param>
        /// <returns></returns>
        public string GetDataFromTable(string table, string column, string Where, string WhareIs)
        {
            string command = "SELECT * FROM " + table + "Where " + Where + " = " + Where;
            MySqlCommand sqlCommand = new MySqlCommand(command, _Connection);

            string returnvalue = "";

            MySqlDataReader dataReader = sqlCommand.ExecuteReader();
            while (dataReader.Read())
            {
                returnvalue += dataReader[column] + " , ";
            }
            dataReader.Close();
            return returnvalue;
        }

        /// <summary>
        /// Get all data from a table
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public string GetDataFromTable(string table)
        {
            string command = "SELECT * FROM " + table;
            MySqlCommand sqlCommand = new MySqlCommand(command, _Connection);

            string returnvalue = "";

            MySqlDataReader dataReader = sqlCommand.ExecuteReader();
            while (dataReader.Read())
            {
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    returnvalue += dataReader[i] + " , ";
                }
            }
            dataReader.Close();
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
        /// Check if certain table exists
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <returns> if table exists</returns>
        public bool CheckTableExistence(string tableName)
        {
            MySqlCommand command = new MySqlCommand("SHOW TABLES LIKE '" + tableName + "';", _Connection);
            try
            {
                MySqlDataReader dataReader = command.ExecuteReader();
                return dataReader.HasRows;
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

        public enum MySqlDataType
        {
            Text,
            Integer,
            Float,
            Boolean
        }
    }

}