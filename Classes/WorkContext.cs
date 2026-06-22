using MySql.Data.MySqlClient;
using ReportGeneration_Lukashevich.Classes.Common;
using ReportGeneration_Lukashevich.Models;
using System;
using System.Collections.Generic;

namespace ReportGeneration_Lukashevich.Classes
{
    class WorkContext : Work
    {
        public WorkContext(int Id, int IdDiscipline, int IdType, DateTime Date, string Name, int Semester) :
            base(Id, IdDiscipline, IdType, Date, Name, Semester)
        { }

        public static List<WorkContext> GetAllWork()
        {
            List<WorkContext> allWorks = new List<WorkContext>();
            MySqlConnection connection = Connection.OpenConnection();
            MySqlDataReader BDSWorks = Connection.Query("SELECT * FROM `work` ORDER BY `Date`", connection);
            while (BDSWorks.Read())
            {
                allWorks.Add(new WorkContext(
                    BDSWorks.GetInt32(0),
                    BDSWorks.GetInt32(1),
                    BDSWorks.GetInt32(2),
                    BDSWorks.GetDateTime(3),
                    BDSWorks.GetString(4),
                    BDSWorks.GetInt32(5)));
            }
            Connection.CloseConnection(connection);
            return allWorks;
        }
    }
}