using ReportGeneration_Lukashevich.Classes;
using ReportGeneration_Lukashevich.Models;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReportGeneration_Lukashevich.Pages
{
    public partial class Main : Page
    {
        internal List<GroupContext> AllGroups = GroupContext.AllGroups();
        internal List<StudentContext> AllStudents = StudentContext.GetAllStudent();
        internal List<WorkContext> AllWorks = WorkContext.GetAllWork();
        internal List<EvaluationContext> AllEvaluation = EvaluationContext.AllEvaluations();
        internal List<DisciplineContext> AllDisciplines = DisciplineContext.AllDisciplines();

        public Main()
        {
            InitializeComponent();
            CreateGroupUI();
            CreateStudents(AllStudents);
        }

        public void CreateGroupUI()
        {
            foreach (GroupContext Group in AllGroups)
                CBGroups.Items.Add(Group.Name);
            CBGroups.Items.Add("Выберите");
            CBGroups.SelectedIndex = CBGroups.Items.Count - 1;
        }

        internal void CreateStudents(List<StudentContext> StudentsList)
        {
            Parent.Children.Clear();
            foreach (StudentContext Student in StudentsList)
                Parent.Children.Add(new Items.Student(Student, this));
        }

        private void SelectGroup(object sender, SelectionChangedEventArgs e)
        {
            if (CBGroups.SelectedIndex != CBGroups.Items.Count - 1)
            {
                int IdGroup = AllGroups.Find(x => x.Name == CBGroups.SelectedItem.ToString()).Id;
                CreateStudents(AllStudents.FindAll(x => x.IdGroup == IdGroup));
            }
        }

        private void SelectStudents(object sender, KeyEventArgs e)
        {
            List<StudentContext> SearchStudent = AllStudents;
            if (CBGroups.SelectedIndex != CBGroups.Items.Count - 1)
            {
                int IdGroup = AllGroups.Find(x => x.Name == CBGroups.SelectedItem.ToString()).Id;
                SearchStudent = AllStudents.FindAll(x => x.IdGroup == IdGroup);
            }
            string filter = TBFIO.Text.Trim();
            if (!string.IsNullOrEmpty(filter))
                SearchStudent = SearchStudent.FindAll(x =>
                    (x.Lastname + " " + x.Firstname).ToLower().Contains(filter.ToLower()));
            CreateStudents(SearchStudent);
        }

        private void ReportGeneration(object sender, System.Windows.RoutedEventArgs e)
        {
            if (CBGroups.SelectedIndex != CBGroups.Items.Count - 1)
            {
                int IdGroup = AllGroups.Find(x => x.Name == CBGroups.SelectedItem.ToString()).Id;
                Classes.Common.Report.Group(IdGroup, this);
            }
        }
    }
}