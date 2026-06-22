using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using ReportGeneration_Lukashevich.Pages;
using System;
using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;

namespace ReportGeneration_Lukashevich.Classes.Common
{
    public class Report
    {
        public static void Group(int IdGroup, Main Main)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel files (*.xlsx)|*.xlsx";
            sfd.DefaultExt = "xlsx";

            if (sfd.ShowDialog() == true && sfd.FileName != "")
            {
                var Group = Main.AllGroups.Find(x => x.Id == IdGroup);
                var ExcelApp = new Excel.Application();

                try
                {
                    ExcelApp.Visible = false;
                    Excel.Workbook Workbook = ExcelApp.Workbooks.Add(Type.Missing);
                    Excel.Worksheet Worksheet = (Excel.Worksheet)Workbook.ActiveSheet;

                    ((Excel.Range)Worksheet.Cells[1, 1]).Value = $"Отчёт о группе {Group.Name}";
                    Worksheet.Range[(Excel.Range)Worksheet.Cells[1, 1], (Excel.Range)Worksheet.Cells[1, 5]].Merge();
                    Styles((Excel.Range)Worksheet.Cells[1, 1], 18);

                    ((Excel.Range)Worksheet.Cells[3, 1]).Value = "Список группы:";
                    Worksheet.Range[(Excel.Range)Worksheet.Cells[3, 1], (Excel.Range)Worksheet.Cells[3, 5]].Merge();
                    Styles((Excel.Range)Worksheet.Cells[3, 1], 12, Excel.XlHAlign.xlHAlignLeft);

                    ((Excel.Range)Worksheet.Cells[4, 1]).Value = "ФИО";
                    Styles((Excel.Range)Worksheet.Cells[4, 1], 12, Excel.XlHAlign.xlHAlignCenter, true);
                    ((Excel.Range)Worksheet.Cells[4, 1]).ColumnWidth = 35.0f;

                    ((Excel.Range)Worksheet.Cells[4, 2]).Value = "Кол-во не сданных практических";
                    Styles((Excel.Range)Worksheet.Cells[4, 2], 12, Excel.XlHAlign.xlHAlignCenter, true);

                    ((Excel.Range)Worksheet.Cells[4, 3]).Value = "Кол-во не сданных теоретических";
                    Styles((Excel.Range)Worksheet.Cells[4, 3], 12, Excel.XlHAlign.xlHAlignCenter, true);

                    ((Excel.Range)Worksheet.Cells[4, 4]).Value = "Отсутствовал на паре";
                    Styles((Excel.Range)Worksheet.Cells[4, 4], 12, Excel.XlHAlign.xlHAlignCenter, true);

                    ((Excel.Range)Worksheet.Cells[4, 5]).Value = "Опоздал";
                    Styles((Excel.Range)Worksheet.Cells[4, 5], 12, Excel.XlHAlign.xlHAlignCenter, true);

                    int Height = 5;
                    List<StudentContext> Students = Main.AllStudents.FindAll(x => x.IdGroup == IdGroup);

                    foreach (StudentContext Student in Students)
                    {
                        List<DisciplineContext> StudentDisciplines = Main.AllDisciplines.FindAll(
                            x => x.IdGroup == Student.IdGroup);

                        int PracticeCount = 0;
                        int TheoryCount = 0;
                        int AbsenteeismCount = 0;
                        int LateCount = 0;

                        foreach (DisciplineContext StudentDiscipline in StudentDisciplines)
                        {
                            List<WorkContext> StudentWorks = Main.AllWorks.FindAll(x =>
                                x.IdDiscipline == StudentDiscipline.Id);

                            foreach (WorkContext StudentWork in StudentWorks)
                            {
                                EvaluationContext Evaluation = Main.AllEvaluation.Find(x =>
                                    x.IdWork == StudentWork.Id &&
                                    x.IdStudent == Student.Id);

                                if ((Evaluation != null && (Evaluation.Value.Trim() == "" || Evaluation.Value.Trim() == "2"))
                                    || Evaluation == null)
                                {
                                    if (StudentWork.IdType == 1)
                                        PracticeCount++;
                                    else if (StudentWork.IdType == 2)
                                        TheoryCount++;
                                }

                                if (Evaluation != null && Evaluation.Lateness.Trim() != "")
                                {
                                    if (Convert.ToInt32(Evaluation.Lateness) == 90)
                                        AbsenteeismCount++;
                                    else
                                        LateCount++;
                                }
                            }
                        }

                        ((Excel.Range)Worksheet.Cells[Height, 1]).Value = $"{Student.Lastname} {Student.Firstname}";
                        Styles((Excel.Range)Worksheet.Cells[Height, 1], 12, Excel.XlHAlign.xlHAlignLeft, true);
                        ((Excel.Range)Worksheet.Cells[Height, 2]).Value = PracticeCount.ToString();
                        Styles((Excel.Range)Worksheet.Cells[Height, 2], 12, Excel.XlHAlign.xlHAlignCenter, true);
                        ((Excel.Range)Worksheet.Cells[Height, 3]).Value = TheoryCount.ToString();
                        Styles((Excel.Range)Worksheet.Cells[Height, 3], 12, Excel.XlHAlign.xlHAlignCenter, true);
                        ((Excel.Range)Worksheet.Cells[Height, 4]).Value = AbsenteeismCount.ToString();
                        Styles((Excel.Range)Worksheet.Cells[Height, 4], 12, Excel.XlHAlign.xlHAlignCenter, true);
                        ((Excel.Range)Worksheet.Cells[Height, 5]).Value = LateCount.ToString();
                        Styles((Excel.Range)Worksheet.Cells[Height, 5], 12, Excel.XlHAlign.xlHAlignCenter, true);

                        Height++;
                    }

                    Workbook.SaveAs(sfd.FileName);
                    Workbook.Close();
                }
                catch (Exception) { }

                ExcelApp.Quit();
            }
        }

        public static void Styles(Excel.Range Cell,
            int FontSize,
            Excel.XlHAlign Position = Excel.XlHAlign.xlHAlignCenter,
            bool Border = false)
        {
            Cell.Font.Name = "Bahnschrift Light Condensed";
            Cell.Font.Size = FontSize;
            Cell.HorizontalAlignment = Position;
            Cell.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

            if (Border)
            {
                Excel.Borders border = Cell.Borders;
                border.LineStyle = Excel.XlLineStyle.xlDouble;
                border.Weight = Excel.XlBorderWeight.xlThin;
                Cell.WrapText = true;
            }
        }
    }
}