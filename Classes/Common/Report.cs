using Microsoft.Win32;
using OfficeOpenXml;
using ReportGeneration_Lukashevich.Pages;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;

namespace ReportGeneration_Lukashevich.Classes.Common
{
    public class Report
    {
        public static void Group(int IdGroup, Main Main)
        {
            // Установка лицензии EPPlus (обязательно!)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel files (*.xlsx)|*.xlsx";
                sfd.DefaultExt = "xlsx";
                sfd.FileName = $"Отчёт_группы_{DateTime.Now:yyyy-MM-dd}";

                if (sfd.ShowDialog() == true && !string.IsNullOrEmpty(sfd.FileName))
                {
                    using (var package = new ExcelPackage())
                    {
                        var Group = Main.AllGroups.Find(x => x.Id == IdGroup);
                        if (Group == null)
                        {
                            MessageBox.Show("Группа не найдена!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        var students = Main.AllStudents.FindAll(x => x.IdGroup == IdGroup);

                        // Основной лист
                        var worksheet = package.Workbook.Worksheets.Add("Отчёт");
                        CreateMainSheet(worksheet, Group, Main);

                        // Отдельные листы для студентов (оценка "отлично")
                        foreach (var student in students)
                        {
                            CreateStudentSheet(package, student, Main);
                        }

                        // Сохраняем файл
                        File.WriteAllBytes(sfd.FileName, package.GetAsByteArray());
                    }

                    MessageBox.Show($"Отчёт успешно сохранён!\n{sfd.FileName}",
                        "Успешно",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании отчёта:\n{ex.Message}\n\n{ex.StackTrace}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private static void CreateMainSheet(OfficeOpenXml.ExcelWorksheet Worksheet, GroupContext Group, Main Main)
        {
            // Заголовок
            Worksheet.Cells[1, 1].Value = $"Отчёт о группе {Group.Name}";
            Worksheet.Cells[1, 1, 1, 5].Merge = true;
            Styles(Worksheet.Cells[1, 1], 18);

            Worksheet.Cells[3, 1].Value = "Список группы:";
            Worksheet.Cells[3, 1, 3, 5].Merge = true;
            Styles(Worksheet.Cells[3, 1], 12, OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);

            // Заголовки таблицы
            string[] headers = { "ФИО", "Кол-во не сданных практических", "Кол-во не сданных теоретических",
                                 "Отсутствовал на паре", "Опоздал" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = Worksheet.Cells[4, i + 1];
                cell.Value = headers[i];
                Styles(cell, 12, OfficeOpenXml.Style.ExcelHorizontalAlignment.Center, true);
                if (i == 0) Worksheet.Column(i + 1).Width = 35;
            }

            int Height = 5;
            var students = Main.AllStudents.FindAll(x => x.IdGroup == Group.Id);

            int bestRow = -1;
            int bestDebtSum = int.MaxValue;
            int bestMissSum = int.MaxValue;

            foreach (StudentContext Student in students)
            {
                var studentDisciplines = Main.AllDisciplines.FindAll(x => x.IdGroup == Student.IdGroup);

                int PracticeCount = 0, TheoryCount = 0, AbsenteeismCount = 0, LateCount = 0;

                foreach (DisciplineContext Discipline in studentDisciplines)
                {
                    var works = Main.AllWorks.FindAll(x => x.IdDiscipline == Discipline.Id);
                    foreach (WorkContext work in works)
                    {
                        var eval = Main.AllEvaluation.Find(x => x.IdWork == work.Id && x.IdStudent == Student.Id);

                        if ((eval != null && (eval.Value.Trim() == "" || eval.Value.Trim() == "2")) || eval == null)
                        {
                            if (work.IdType == 1) PracticeCount++;
                            else if (work.IdType == 2) TheoryCount++;
                        }

                        if (eval != null && eval.Lateness.Trim() != "")
                        {
                            if (Convert.ToInt32(eval.Lateness) == 90)
                                AbsenteeismCount++;
                            else
                                LateCount++;
                        }
                    }
                }

                Worksheet.Cells[Height, 1].Value = $"{Student.Lastname} {Student.Firstname}";
                Styles(Worksheet.Cells[Height, 1], 12, OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, true);
                Worksheet.Cells[Height, 2].Value = PracticeCount;
                Styles(Worksheet.Cells[Height, 2], 12, OfficeOpenXml.Style.ExcelHorizontalAlignment.Center, true);
                Worksheet.Cells[Height, 3].Value = TheoryCount;
                Styles(Worksheet.Cells[Height, 3], 12, OfficeOpenXml.Style.ExcelHorizontalAlignment.Center, true);
                Worksheet.Cells[Height, 4].Value = AbsenteeismCount;
                Styles(Worksheet.Cells[Height, 4], 12, OfficeOpenXml.Style.ExcelHorizontalAlignment.Center, true);
                Worksheet.Cells[Height, 5].Value = LateCount;
                Styles(Worksheet.Cells[Height, 5], 12, OfficeOpenXml.Style.ExcelHorizontalAlignment.Center, true);

                int debtSum = PracticeCount + TheoryCount;
                int missSum = AbsenteeismCount + LateCount;
                if (debtSum < bestDebtSum || (debtSum == bestDebtSum && missSum < bestMissSum))
                {
                    bestDebtSum = debtSum;
                    bestMissSum = missSum;
                    bestRow = Height;
                }

                Height++;
            }

            // Выделение лучшего студента (оценка "хорошо")
            if (bestRow > 0)
            {
                Worksheet.Cells[bestRow, 1, bestRow, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Worksheet.Cells[bestRow, 1, bestRow, 5].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                Worksheet.Cells[bestRow, 1, bestRow, 5].Style.Font.Bold = true;
            }

            Worksheet.Cells.AutoFitColumns();
        }

        private static void CreateStudentSheet(ExcelPackage package, StudentContext student, Main Main)
        {
            string sheetName = $"{student.Lastname} {student.Firstname[0]}.";
            if (!string.IsNullOrEmpty(student.Othername))
                sheetName += $"{student.Othername[0]}.";
            if (sheetName.Length > 31) sheetName = sheetName.Substring(0, 31);

            var sheet = package.Workbook.Worksheets.Add(sheetName);

            // Заголовок
            sheet.Cells[1, 1].Value = $"Студент: {student.Lastname} {student.Firstname}";
            sheet.Cells[1, 1, 1, 5].Merge = true;
            Styles(sheet.Cells[1, 1], 14, OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);

            // Заголовки таблицы работ
            string[] headers = { "Дисциплина", "Название работы", "Тип", "Оценка", "Статус" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = sheet.Cells[3, i + 1];
                cell.Value = headers[i];
                Styles(cell, 12, OfficeOpenXml.Style.ExcelHorizontalAlignment.Center, true);
            }

            int row = 4;
            var disciplines = Main.AllDisciplines.FindAll(x => x.IdGroup == student.IdGroup);
            foreach (var discipline in disciplines)
            {
                var works = Main.AllWorks.FindAll(x => x.IdDiscipline == discipline.Id);
                foreach (var work in works)
                {
                    var eval = Main.AllEvaluation.Find(x => x.IdWork == work.Id && x.IdStudent == student.Id);
                    string grade = (eval != null && !string.IsNullOrEmpty(eval.Value)) ? eval.Value : "—";
                    string status;
                    if (eval != null && !string.IsNullOrEmpty(eval.Value) && eval.Value.Trim() != "2")
                        status = "Сдано";
                    else
                        status = "Не сдано";

                    sheet.Cells[row, 1].Value = discipline.Name;
                    sheet.Cells[row, 2].Value = work.Name;
                    sheet.Cells[row, 3].Value = work.IdType == 1 ? "Практика" : work.IdType == 2 ? "Теория" : "Прочее";
                    sheet.Cells[row, 4].Value = grade;
                    sheet.Cells[row, 5].Value = status;

                    for (int col = 1; col <= 5; col++)
                        Styles(sheet.Cells[row, col], 11, OfficeOpenXml.Style.ExcelHorizontalAlignment.Center, false);
                    row++;
                }
            }

            sheet.Cells.AutoFitColumns();
        }

        public static void Styles(OfficeOpenXml.ExcelRange Cell,
            int FontSize,
            OfficeOpenXml.Style.ExcelHorizontalAlignment Position = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center,
            bool Border = false)
        {
            Cell.Style.Font.Name = "Bahnschrift Light Condensed";
            Cell.Style.Font.Size = FontSize;
            Cell.Style.HorizontalAlignment = Position;
            Cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            if (Border)
            {
                Cell.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                Cell.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                Cell.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                Cell.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                Cell.Style.WrapText = true;
            }
        }
    }
}