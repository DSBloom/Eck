using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Eck.Models
{
    public class DataAccess
    {
        internal static void InsertTestData(TestData model)
        {
            SqlConnection conn = new SqlConnection(@"Server=tcp:ye8viai1bq.database.windows.net,1433;Database=Eck01_db;User ID=nadcraker@ye8viai1bq;Password=Fallout!@#$;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            string insertText = @"INSERT INTO Teams (TeamName, TeamOwner, Digit00, Digit01, Digit02, Digit03, Digit04, Digit05, Digit06, Digit07, Digit08, Digit09, TeamOwnerEmail) " +
                                 "VALUES (@TeamName, @TeamOwner, @Digit00, @Digit01, @Digit02, @Digit03, @Digit04, @Digit05, @Digit06, @Digit07, @Digit08, @Digit09, @TeamOwnerEmail)";
            SqlCommand cmd = new SqlCommand(insertText, conn);
            cmd.Parameters.AddWithValue("TeamName", model.TeamName);
            cmd.Parameters.AddWithValue("TeamOwner", model.TeamOwner);
            cmd.Parameters.AddWithValue("TeamOwnerEmail", model.TeamOwnerEmail);
            cmd.Parameters.AddWithValue("Digit00", model.Digit00);
            cmd.Parameters.AddWithValue("Digit01", model.Digit01);
            cmd.Parameters.AddWithValue("Digit02", model.Digit02);
            cmd.Parameters.AddWithValue("Digit03", model.Digit03);
            cmd.Parameters.AddWithValue("Digit04", model.Digit04);
            cmd.Parameters.AddWithValue("Digit05", model.Digit05);
            cmd.Parameters.AddWithValue("Digit06", model.Digit06);
            cmd.Parameters.AddWithValue("Digit07", model.Digit07);
            cmd.Parameters.AddWithValue("Digit08", model.Digit08);
            cmd.Parameters.AddWithValue("Digit09", model.Digit09);

            try
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {

            }
        }

        internal static List<TestData> GetTestData()
        {
            List<TestData> testList = new List<TestData>();
            SqlConnection conn = new SqlConnection(@"Server=tcp:ye8viai1bq.database.windows.net,1433;Database=Eck01_db;User ID=nadcraker@ye8viai1bq;Password=Fallout!@#$;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            string selectText = @"SELECT * From Teams";
            SqlCommand cmd = new SqlCommand(selectText, conn);

            try
            {
                conn.Open();
                if(conn.State == System.Data.ConnectionState.Open)
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        TestData test = new TestData();

                        test.Digit00 = (bool)reader["digit00"];
                        test.Digit01 = (bool)reader["digit01"];
                        test.Digit02 = (bool)reader["digit02"];
                        test.Digit03 = (bool)reader["digit03"];
                        test.Digit04 = (bool)reader["digit04"];
                        test.Digit05 = (bool)reader["digit05"];
                        test.Digit06 = (bool)reader["digit06"];
                        test.Digit07 = (bool)reader["digit07"];
                        test.Digit08 = (bool)reader["digit08"];
                        test.Digit09 = (bool)reader["digit09"];
                        test.Id = (int)reader["id"];
                        test.TeamName = (string)reader["TeamName"];
                        test.TeamOwner = (string)reader["TeamOwner"];
                        test.TeamOwnerEmail = (string)reader["TeamOwnerEmail"];

                        testList.Add(test);
                    }
                }
            }            
            catch(Exception ex)
            {

            }
            return testList;
        }

        //My code
        
    }
}