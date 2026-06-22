using System;

namespace ReportGeneration_Lukashevich.Models
{
    class Student
    {
        [Obsolete]
        public int Id { get; set; }

        [Obsolete]
        public string Firstname { get; set; }

        [Obsolete]
        public string Lastname { get; set; }

        [Obsolete]
        public int IdGroup { get; set; }

        [Obsolete]
        public string Othername { get; set; }

        [Obsolete]
        public DateTime DateExpelled { get; set; }

        [Obsolete]
        public bool Expelled { get; set; }

        [Obsolete]
        public Student(int Id, string Firstname, string Lastname, int IdGroup, bool Expelled, DateTime DateExpelled)
        {
            this.Id = Id;
            this.Firstname = Firstname;
            this.Lastname = Lastname;
            this.IdGroup = IdGroup;
            this.Expelled = Expelled;
            this.DateExpelled = DateExpelled;
        }
    }
}