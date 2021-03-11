using System;
using System.Data.SQLite;

namespace SQLInjection
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathToDBFile = @"Data Source=/Users/trevorschmidt/Desktop/IS_414_Lab_5/WebSecurity.db"; // <== Replace <PATH TO SQLITE DB FILE> to the path of the file on your computer

        
            SQLiteConnection conn = new SQLiteConnection(pathToDBFile);
  
            // SusceptibleToSQLi(conn);
            //NotSusceptibleToSQLi(conn);
            FixSQLi(conn);
        }

        public static void SusceptibleToSQLi(SQLiteConnection conn)
        {
            try
            {
                conn.Open();
            }
            catch
            {
                Console.WriteLine("Problem connecting to database file.");
            }

            SQLiteCommand cmd = conn.CreateCommand();

            string expectedInput = "CA"; // This input is the two letter state code and it an expected input from users of our WebApp
            string maliciousInput = "CA' UNION SELECT ingredients from SecretRecipes;--"; // This input is designed to perform SQL injection. Basically it takes the expected input and adds additional SQL code to get information from another table in our DB.

            string state = expectedInput;
            //state = maliciousInput; // <---- UNCOMMENT LINE TO PERFORM SQLi ATTACK using malicious input

            cmd.CommandText = "SELECT BrandName FROM Competition WHERE State='" + state + "'";

            SQLiteDataReader sqlDR = cmd.ExecuteReader();

            while (sqlDR.Read())
            {
                Console.WriteLine(sqlDR.GetString(0));
            }

            conn.Close();
        }

        public static void NotSusceptibleToSQLi(SQLiteConnection conn)
        {
            try
            {
                conn.Open();
            }
            catch
            {
                Console.WriteLine("Problem connecting to database file.");
            }

            SQLiteCommand cmd = conn.CreateCommand();

            string expectedInput = "CA"; // This input is the two letter state code and it an expected input from users of our WebApp
            string maliciousInput = "CA' UNION SELECT ingredients from SecretRecipes;--"; // This input is designed to perform SQL injection. Basically it takes the expected input and adds additional SQL code to get information from another table in our DB.

            string state = expectedInput;
            //state = maliciousInput; // <---- UNCOMMENT LINE TO PERFORM SQLi ATTACK using malicious input

            cmd.CommandText = "SELECT BrandName FROM Competition WHERE State=@state";
            cmd.Parameters.AddWithValue("@state", state);

            SQLiteDataReader sqlDR = cmd.ExecuteReader();

            while (sqlDR.Read())
            {
                Console.WriteLine(sqlDR.GetString(0));
            }

            conn.Close();
        }

        public static void FixSQLi(SQLiteConnection conn)
        {
            // Db Setup
            try
            {
                conn.Open();
            }
            catch
            {
                Console.WriteLine("Problem connecting to database file.");
            }
            SQLiteCommand cmd = conn.CreateCommand();

            // parameter setup
            string expectedDescription = "%" + "origin%"; 
            string expectedCaneSugar = "1"; 
            string maliciousCaneSugar = "1 UNION SELECT ingredients from SecretRecipes;--"; 

            // CHANGE FOR MALICIOUS/EXPECTED
            string sugar = expectedCaneSugar;
            //sugar = maliciousCaneSugar; // <---- UNCOMMENT LINE TO PERFORM SQLi ATTACK using malicious input

            // NORMAL SQL
            // cmd.CommandText = "SELECT BrandName from Competition WHERE Description LIKE '" + expectedDescription + "' and CaneSugar=" + sugar;

            // PARAMETERIZED SQL
            cmd.CommandText = "SELECT BrandName FROM Competition WHERE Description LIKE @expectedDescription and CaneSugar=@sugar";
            cmd.Parameters.AddWithValue("@expectedDescription", expectedDescription); 
            cmd.Parameters.AddWithValue("@sugar", sugar);

            // Db CALL
            SQLiteDataReader sqlDR = cmd.ExecuteReader();
            while (sqlDR.Read())
            {
                Console.WriteLine(sqlDR.GetString(0));
            }
            conn.Close();
        }
    }
}
