using MySql.Data.MySqlClient;
using ReportGeneration_Lukashevich.Classes.Common;
using ReportGeneration_Lukashevich.Models;
using System.Collections.Generic;

namespace ReportGeneration_Lukashevich.Classes
{
    public class GroupContext : Group
    {
        public GroupContext(int Id, string Name) : base(Id, Name) { }

        public static List<GroupContext> AllGroups()
        {
            List<GroupContext> allGroups = new List<GroupContext>();
            MySqlConnection connection = Connection.OpenConnection();
            MySqlDataReader BDOgroups = Connection.Query("SELECT * FROM `group` ORDER BY `Name`", connection);
            while (BDOgroups.Read())
            {
                allGroups.Add(new GroupContext(
                    BDOgroups.GetInt32(0),
                    BDOgroups.GetString(1)));
            }
            Connection.CloseConnection(connection);
            return allGroups;
        }
    }
}