using MySql.Data.MySqlClient;
using ReportGeneration_Lukashevich.Classes.Common;
using ReportGeneration_Lukashevich.Models;
using System;
using System.Collections.Generic;

namespace ReportGeneration_Lukashevich.Classes
{
    class StudentContext : Student
    {
        public StudentContext(int Id, string Firstname, string Lastname, int IdGroup, bool Expelled, DateTime Datexpeilled) :
            base(Id, Firstname, Lastname, IdGroup, Expelled, Datexpeilled)
        { }

        public static List<StudentContext> GetAllStudent()
        {
            List<StudentContext> allStudent = new List<StudentContext>();
            MySqlConnection connection = Connection.OpenConnection();
            MySqlDataReader BDSStudents = Connection.Query("SELECT * FROM `student` ORDER BY `LastName`", connection);
            while (BDSStudents.Read())
            {
                allStudent.Add(new StudentContext(
                    BDSStudents.GetInt32(0),
                    BDSStudents.GetString(2),
                    BDSStudents.GetString(1),
                    BDSStudents.GetInt32(3),
                    BDSStudents.GetBoolean(4),
                    BDSStudents.IsDBNull(5) ? DateTime.Now : BDSStudents.GetDateTime(5)
                ));
            }
            Connection.CloseConnection(connection);
            return allStudent;
        }
    }
}