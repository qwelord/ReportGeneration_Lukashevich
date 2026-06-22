using MySql.Data.MySqlClient;
using ReportGeneration_Lukashevich.Classes.Common;
using ReportGeneration_Lukashevich.Models;
using System.Collections.Generic;

namespace ReportGeneration_Lukashevich.Classes
{
    public class DisciplineContext : Discipline
    {
        public DisciplineContext(int Id, string Name, int IdGroup) : base(Id, Name, IdGroup) { }

        public static List<DisciplineContext> AllDisciplines()
        {
            List<DisciplineContext> allDisciplines = new List<DisciplineContext>();
            MySqlConnection connection = Connection.OpenConnection();
            MySqlDataReader BDOdisciplines = Connection.Query("SELECT * FROM `discipline` ORDER BY `Name`", connection);
            while (BDOdisciplines.Read())
            {
                allDisciplines.Add(new DisciplineContext(
                    BDOdisciplines.GetInt32(0),
                    BDOdisciplines.GetString(1),
                    BDOdisciplines.GetInt32(2)));
            }
            Connection.CloseConnection(connection);
            return allDisciplines;
        }
    }
}