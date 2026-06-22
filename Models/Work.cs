using System;

namespace ReportGeneration_Lukashevich.Models
{
    class Work
    {
        [Obsolete]
        public int Id { get; set; }

        [Obsolete]
        public int IdDiscipline { get; set; }

        [Obsolete]
        public int IdType { get; set; }

        [Obsolete]
        public DateTime Date { get; set; }

        [Obsolete]
        public string Name { get; set; }

        [Obsolete]
        public int Semester { get; set; }

        [Obsolete]
        public Work(int Id, int IdDiscipline, int IdType, DateTime Date, string Name, int Semester)
        {
            this.Id = Id;
            this.IdDiscipline = IdDiscipline;
            this.IdType = IdType;
            this.Date = Date;
            this.Name = Name;
            this.Semester = Semester;
        }
    }
}