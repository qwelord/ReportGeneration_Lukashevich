using ReportGeneration_Lukashevich.Classes;
using ReportGeneration_Lukashevich.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReportGeneration_Lukashevich.Items
{
    /// <summary>
    /// Логика взаимодействия для Student.xaml
    /// </summary>
    public partial class Student : UserControl
    {
        private Main _main;
        public Student()
        {
            InitializeComponent();
        }

        internal Student(StudentContext student, Main mainPage)
        {
            InitializeComponent();
            _main = mainPage;

            TBFio.Text = $"({student.Lastname}) {student.Firstname}";
            CBExplored.IsChecked = student.Expelled;

            List<DisciplineContext> StudentDisciplines = _main.AllDisciplines.FindAll(
                x => x.IdGroup == student.IdGroup);

            int NecessarilyCount = 0;
            int WorksCount = 0;
            int DoneCount = 0;
            int MissedCount = 0;

            foreach (DisciplineContext StudentDiscipline in StudentDisciplines)
            {
                List<WorkContext> StudentWorks = _main.AllWorks.FindAll(x =>
                    (x.IdType == 1 || x.IdType == 2 || x.IdType == 3) &&
                    x.IdDiscipline == StudentDiscipline.Id);

                NecessarilyCount += StudentWorks.Count;

                foreach (WorkContext StudentWork in StudentWorks)
                {
                    EvaluationContext Evaluation = _main.AllEvaluation.Find(x =>
                        x.IdWork == StudentWork.Id &&
                        x.IdStudent == student.Id);

                    if (Evaluation != null && Evaluation.Value.Trim() != "" && Evaluation.Value.Trim() != "2")
                        DoneCount++;
                }

                StudentWorks = _main.AllWorks.FindAll(x => x.IdType != 4 && x.IdType != 3);
                WorksCount += StudentWorks.Count;

                foreach (WorkContext StudentWork in StudentWorks)
                {
                    EvaluationContext Evaluation = _main.AllEvaluation.Find(x =>
                        x.IdWork == StudentWork.Id &&
                        x.IdStudent == student.Id);

                    if (Evaluation != null && Evaluation.Lateness.Trim() != "")
                        MissedCount += Convert.ToInt32(Evaluation.Lateness);
                }
            }

            doneWorkers.Value = (NecessarilyCount > 0) ? (100f / NecessarilyCount) * DoneCount : 0;
            missedCount.Value = (WorksCount > 0) ? (100f / (WorksCount * 90f)) * MissedCount : 0;

            TBGroup.Text = _main.AllGroups.Find(x => x.Id == student.IdGroup).Name;
        }
    }
}
