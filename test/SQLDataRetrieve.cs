using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    class SQLDataRetrieve
    {
        public static SqlConnectionStringBuilder ConnectionString()
        {
            SqlConnectionStringBuilder sql = new SqlConnectionStringBuilder();

            sql.DataSource = "lorawan-hank.database.windows.net";
            sql.UserID = "Hank";
            sql.Password = "Lorawan1234";
            sql.InitialCatalog = "LoraWan Database";

            return sql;
        }

        public static PersonData GetPersonDatabaseData(string retrieve)
        {
            PersonData person = new PersonData();

            //build conenction string
            SqlConnectionStringBuilder sql = ConnectionString();

            using (SqlConnection sqlConn = new SqlConnection(sql.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand(retrieve, sqlConn);
                try
                {
                    sqlConn.Open();
                    sqlCommand.ExecuteNonQuery();
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            person.ID = reader.GetString(0);
                            person.Age = reader.GetString(0);
                            person.Gender = reader.GetString(0);
                            person.Angry = reader.GetString(0);
                            person.Disgust = reader.GetString(0);
                            person.Fear = reader.GetString(0);
                            person.Happy = reader.GetString(0);
                            person.Sad = reader.GetString(0);
                            person.Surprised = reader.GetString(0);
                            person.Neutral = reader.GetString(0);
                            person.Style1 = reader.GetString(0);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    DisplaySqlErrors(ex, true);
                }
                sqlConn.Close();
            }
            return person;
        }

        //display sql errors and display it on screen
        public async static void DisplaySqlErrors(SqlException exception, bool isdesktop)
        {
            if (isdesktop)
            {
                for (int i = 0; i < exception.Errors.Count; i++)
                {
                    Console.WriteLine("Index #" + i + "\n" +
                        "Error: " + exception.Errors[i].ToString() + "\n", "Wrong DateTime Format");
                }
            }
        }

        public class PersonData
        {
            public string ID
            {
                get;
                set;
            }

            public string Age
            {
                get;
                set;
            }

            public string Gender
            {
                get;
                set;
            }

            public string Angry
            {
                get;
                set;
            }

            public string Disgust
            {
                get;
                set;
            }

            public string Fear
            {
                get;
                set;
            }

            public string Happy
            {
                get;
                set;
            }

            public string Sad
            {
                get;
                set;
            }

            public string Surprised
            {
                get;
                set;
            }

            public string Neutral
            {
                get;
                set;
            }

            public string Style1
            {
                get;
                set;
            }
        }
    }
}
