using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreKeeper
{
    static class DataAccess
    {
        static SqlCeConnection _conn;

        public static void Open()
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, "Scores.sdf");
            string connectionString = String.Format("Data Source={0}", filePath);

            if (!File.Exists(filePath))
            {
                using (SqlCeEngine engine = new SqlCeEngine(connectionString))
                {
                    engine.CreateDatabase();
                }

                //scores
                using (SqlCeConnection conn = new SqlCeConnection(connectionString))
                using (SqlCeCommand cmd = new SqlCeCommand("CREATE TABLE Scores (Name NVARCHAR(255), Points INT, Picture IMAGE);", conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                //funds
                using (SqlCeConnection conn = new SqlCeConnection(connectionString))
                using (SqlCeCommand cmd = new SqlCeCommand("CREATE TABLE Funds (Amount MONEY);", conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            _conn = new SqlCeConnection(connectionString);
            _conn.Open();
        }

        public static void ReportFunds(decimal amount)
        {
            using (SqlCeCommand cmd = new SqlCeCommand("INSERT INTO Funds (Amount) VALUES (@amt);", _conn))
            {
                AddParam(cmd, "@amt", amount, SqlDbType.Money);
                cmd.ExecuteNonQuery();
            }
        }

        public static Decimal GetFunds()
        {
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT SUM(Amount) FROM Funds;", _conn))
            {
                var amt = cmd.ExecuteScalar();
                if (amt == DBNull.Value)
                {
                    return 0;
                }
                return (decimal)amt;
            }
        }

        public static void ReportScore(Score score)
        {
            using (SqlCeCommand cmd = new SqlCeCommand("INSERT INTO Scores (Name, Points, Picture) VALUES (@name, @points, @picture);", _conn))
            {
                AddParam(cmd, "@name", score.Name, SqlDbType.NVarChar);
                AddParam(cmd, "@points", score.Points, SqlDbType.Int);
                AddParam(cmd, "@picture", score.Picture, SqlDbType.Image);
                cmd.ExecuteNonQuery();
            }
        }

        public static Score[] GetTopScores()
        {
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT TOP 20 Name, Points, Picture FROM Scores ORDER BY Points DESC;", _conn))
            {
                List<Score> scores = new List<Score>();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    scores.Add(new Score()
                    {
                        Name = reader.GetString(0),
                        Points = reader.GetInt32(1),
                        Picture = (byte[])reader.GetValue(2),
                    });
                }

                return scores.ToArray();
            }
        }

        public static void Close()
        {
            if (_conn != null)
            {
                _conn.Dispose();
                _conn = null;
            }
        }

        private static void AddParam(SqlCeCommand cmd, string name, object value, SqlDbType type)
        {
            var param = ((SqlCeCommand)cmd).CreateParameter();
            param.ParameterName = name;
            param.Value = value;
            param.SqlDbType = type;
            cmd.Parameters.Add(param);
        }

    }
}
