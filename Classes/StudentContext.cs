using MySql.Data.MySqlClient;
using ReportGeneration_Lukashevich.Classes.Common;
using ReportGeneration_Lukashevich.Models;
using System;
using System.Collections.Generic;

namespace ReportGeneration_Lukashevich.Classes
{
    class StudentContext : Student
    {
        public StudentContext(int Id, string Firstname, string Lastname, int IdGroup, bool Expelled, DateTime DateExpelled) :
            base(Id, Firstname, Lastname, IdGroup, Expelled, DateExpelled)
        { }

        public static List<StudentContext> GetAllStudent()
        {
            List<StudentContext> allStudent = new List<StudentContext>();
            MySqlConnection connection = Connection.OpenConnection();
            MySqlDataReader BDSStudents = Connection.Query("SELECT * FROM `student` ORDER BY `last_name`", connection);
            while (BDSStudents.Read())
            {
                int id = BDSStudents.GetInt32(BDSStudents.GetOrdinal("id"));
                string firstName = BDSStudents.GetString(BDSStudents.GetOrdinal("first_name"));
                string lastName = BDSStudents.GetString(BDSStudents.GetOrdinal("last_name"));
                int groupId = BDSStudents.GetInt32(BDSStudents.GetOrdinal("group_id"));

                bool expelled = false;
                int ordinalExpelled = BDSStudents.GetOrdinal("expelled");
                if (!BDSStudents.IsDBNull(ordinalExpelled))
                {
                    try
                    {
                        object value = BDSStudents.GetValue(ordinalExpelled);
                        if (value is bool boolValue)
                            expelled = boolValue;
                        else if (value is sbyte sbyteValue)
                            expelled = sbyteValue != 0;
                        else if (value is byte byteValue)
                            expelled = byteValue != 0;
                        else if (value is string stringValue)
                            expelled = stringValue.ToLower() == "true" || stringValue == "1";
                        else if (value is int intValue)
                            expelled = intValue != 0;
                    }
                    catch { expelled = false; }
                }

                DateTime dateExpelled = DateTime.Now;
                int ordinalDateExpelled = BDSStudents.GetOrdinal("date_expelled");
                if (!BDSStudents.IsDBNull(ordinalDateExpelled))
                {
                    try
                    {
                        object value = BDSStudents.GetValue(ordinalDateExpelled);
                        if (value is DateTime dt)
                            dateExpelled = dt;
                        else if (value is string str && DateTime.TryParse(str, out DateTime parsed))
                            dateExpelled = parsed;
                        else if (value is long longValue)
                            dateExpelled = DateTime.FromBinary(longValue);
                        else if (value is DateTimeOffset dto)
                            dateExpelled = dto.DateTime;
                    }
                    catch { dateExpelled = DateTime.Now; }
                }

                allStudent.Add(new StudentContext(
                    id,
                    firstName,
                    lastName,
                    groupId,
                    expelled,
                    dateExpelled
                ));
            }
            Connection.CloseConnection(connection);
            return allStudent;
        }
    }
}