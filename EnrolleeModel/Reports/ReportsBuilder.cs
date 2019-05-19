using EnrolleeModel;
using System.Drawing;
using System.Linq;

namespace Reports
{
    /// <summary>
    /// Построитель отчетов
    /// </summary>
    public static class ReportsBuilder
    {
        /// <summary>
        /// Отчет "Анкетные данные абитуриентов, имеющих красный диплом или медаль"
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static Report GetEnrolleesWithGoldMedal(Root root)
        {
            var caption = "Анкетные данные абитуриентов, имеющих красный диплом или медаль";
            var report = new Report
            {
                Caption = caption
            };
            report.ReportColumns.Add(
                new ReportColumn("Рег. №"),
                new ReportColumn("Фамилия", 120),
                new ReportColumn("Имя", 120),
                new ReportColumn("Отчество", 120),
                new ReportColumn("Дата рождения", 150),
                new ReportColumn("Специальность", 150));
            // добавляем строки в отчет
            foreach (var item in root.Enrollees.Where(item => item.GoldMedal))
            {
                report.ReportRows.Add(0, $"{item.RegistrationNumber}", 
                                      $"{item.Surname}", $"{item.FirstName}", $"{item.LastName}",
                                      $"{item.BirthDay.ToShortDateString()}", $"{Helper.SpecialityById(item.IdSpeciality)}");
            }
            // определение обработчика печати страницы отчета
            report.PrintPage = (o, e, rect, offset) => 
            {
                SizeF strSize = new SizeF();
                var strPoint = offset;
                // Печать заголовка таблицы
                strPoint.X = rect.X;
                using (var headerfont = new Font("Arial", 12, FontStyle.Italic))
                using (var sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    foreach (var header in report.ReportColumns)
                    {
                        strSize = e.Graphics.MeasureString(header.Text, headerfont);
                        var r = new Rectangle(Point.Ceiling(strPoint),
                            new Size(header.Width, (int)strSize.Height));
                        e.Graphics.DrawString(header.Text, headerfont, Brushes.Navy, r, sf);
                        strPoint.X += header.Width;
                    }
                }
                // Печать строк таблицы
                strPoint.Y += strSize.Height + 10;
                using (var rowfont = new Font("Arial", 10, FontStyle.Regular))
                using (var sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    foreach (var row in report.ReportRows)
                    {
                        strPoint.X = rect.X;
                        string value; // здесь будет значение
                        for (var i = 0; i < report.ReportColumns.Count; i++)
                        {
                            value = row.Items[i];
                            var r = new Rectangle(Point.Ceiling(strPoint),
                                new Size(report.ReportColumns[i].Width, (int)strSize.Height));
                            e.Graphics.DrawRectangle(Pens.Black, r);
                            e.Graphics.DrawString(value, rowfont, Brushes.Black, r, sf);
                            strPoint.X += report.ReportColumns[i].Width;
                        }
                        strPoint.Y += strSize.Height;
                    }
                }
            };
            return report;
        }

        /// <summary>
        /// Отчет "Все инициалы абитуриентов по специальностям в алфавитном порядке с указанием сдаваемых предметов"
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static Report GetEnrolleesBySpecialityAphabet(Root root)
        {
            var caption = "Все инициалы абитуриентов по специальностям в алфавитном порядке с указанием сдаваемых предметов";
            var report = new Report
            {
                Caption = caption
            };
            report.ReportColumns.Add(
                new ReportColumn("Специальность", 200),
                new ReportColumn("Предмет"),
                new ReportColumn("Ф.И.О."),
                new ReportColumn("Форма сдачи"));
            // данные отчета группирутся по условиям
            foreach (var groupSpeciality in root.Enrollees.OrderBy(x => Helper.SpecialityById(x.IdSpeciality))
                                                          .GroupBy(x => x.IdSpeciality))
            {
                report.ReportRows.Add(0, "Специальность", $"{Helper.SpecialityById(groupSpeciality.Key)}", "", "");
                foreach (var enrollee in groupSpeciality.OrderBy(x => x.ToString()))
                {
                    report.ReportRows.Add(1, $"{Helper.EnrolleeName(enrollee.IdEnrollee)}", "", "", "");
                    foreach (var item in root.PassMatters.Where(x => x.IdSpeciality == enrollee.IdSpeciality)
                                                         .OrderBy(x => Helper.SpecialityById(x.IdSpeciality)))
                        report.ReportRows.Add(2, $"{Helper.MatterById(item.IdMatter)}", $"{item.PassForm}", "", "");
                }
            }
            // определение обработчика печати страницы отчета
            report.PrintPage = (o, e, rect, offset) =>
            {
                SizeF strSize = new SizeF();
                var strPoint = offset;
                strPoint.X = rect.X;
                strPoint.Y -= 10;
                using (var headerfont = new Font("Arial", 12, FontStyle.Italic))
                    strSize = e.Graphics.MeasureString("Xxy", headerfont);
                foreach (var row in report.ReportRows)
                {
                    if (row.Level == 0)
                    {
                        strPoint.X = rect.X;
                        strPoint.Y += 10;
                        string value; // здесь будет значение
                        for (var i = 0; i < report.ReportColumns.Count; i++)
                        {
                            value = row.Items[i];
                            if (i == 0)
                                using (var rowfont = new Font("Arial", 12, FontStyle.Bold | FontStyle.Italic))
                                    e.Graphics.DrawString(value, rowfont, Brushes.Navy, strPoint);
                            else
                                using (var rowfont = new Font("Arial", 12, FontStyle.Bold))
                                    e.Graphics.DrawString(value, rowfont, Brushes.Black, strPoint);

                            strPoint.X += report.ReportColumns[i].Width;
                        }
                        strPoint.Y += strSize.Height;
                        strPoint.X = rect.X;
                        using (var headerfont = new Font("Arial", 12, FontStyle.Italic))
                        using (var sf = new StringFormat())
                        {
                            sf.Alignment = StringAlignment.Center;
                            sf.LineAlignment = StringAlignment.Center;
                            var r = new Rectangle(Point.Ceiling(strPoint),
                                new Size((int)rect.Width, (int)strSize.Height));
                            e.Graphics.DrawString("ФИО", headerfont, Brushes.Navy, r, sf);
                            strPoint.Y += strSize.Height + 5;
                        }
                    }
                    else if (row.Level == 1)
                    {
                        strPoint.X = rect.X;
                        strPoint.Y += 5;
                        using (var pen = new Pen(Color.Black))
                        {
                            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                            e.Graphics.DrawRectangle(pen,
                                new Rectangle(Point.Ceiling(strPoint),
                                              new Size((int)rect.Width, (int)strSize.Height * 3 + 10)));
                        }
                        using (var headerfont = new Font("Arial", 12, FontStyle.Italic | FontStyle.Bold))
                        using (var sf = new StringFormat())
                        {
                            sf.Alignment = StringAlignment.Center;
                            sf.LineAlignment = StringAlignment.Center;
                            var r = new Rectangle(Point.Ceiling(strPoint),
                                new Size((int)rect.Width, (int)strSize.Height));
                            e.Graphics.DrawString(row.Items[0], headerfont, Brushes.Black, r, sf);
                            strPoint.Y += strSize.Height;
                        }
                        strPoint.X = rect.X;
                        using (var headerfont = new Font("Arial", 12, FontStyle.Italic))
                        using (var sf = new StringFormat())
                        {
                            e.Graphics.DrawString("Сдаваемые предметы", headerfont, Brushes.Navy, strPoint);
                            strPoint.Y += strSize.Height + 10;
                            sf.Alignment = StringAlignment.Center;
                            sf.LineAlignment = StringAlignment.Center;
                            var r1 = new Rectangle(Point.Ceiling(strPoint),
                                new Size((int)rect.Width / 2, (int)strSize.Height));
                            var r2 = r1;
                            r2.Offset(r1.Width, 0);
                            e.Graphics.DrawString("Предмет", headerfont, Brushes.Navy, r1, sf);
                            e.Graphics.DrawString("Форма сдачи", headerfont, Brushes.Navy, r2, sf);
                            strPoint.Y += strSize.Height;
                        }
                    }
                    else if (row.Level == 2)
                    {
                        strPoint.X = rect.X;
                        strPoint.Y += 5;
                        e.Graphics.DrawRectangle(Pens.Black,
                                                 new Rectangle(Point.Ceiling(strPoint),
                                                               new Size((int)rect.Width, (int)strSize.Height)));
                        using (var rowfont = new Font("Arial", 10, FontStyle.Regular))
                        using (var sf = new StringFormat())
                        {
                            sf.Alignment = StringAlignment.Center;
                            sf.LineAlignment = StringAlignment.Center;
                            var r1 = new Rectangle(Point.Ceiling(strPoint),
                                new Size((int)rect.Width / 2, (int)strSize.Height));
                            var r2 = r1;
                            r2.Offset(r1.Width, 0);
                            e.Graphics.DrawString(row.Items[0], rowfont, Brushes.Black, r1, sf);
                            e.Graphics.DrawString(row.Items[1], rowfont, Brushes.Black, r2, sf);
                            strPoint.Y += strSize.Height;
                        }
                    }
                }
            };
            return report;
        }

        /// <summary>
        /// Отчет "Анкетные данные по специальностям"
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static Report GetEnrolleesBySpeciality(Root root)
        {
            var caption = "Анкетные данные по специальностям";
            var report = new Report
            {
                Caption = caption
            };
            report.ReportColumns.Add(
                new ReportColumn("Фамилия", 150),
                new ReportColumn("Имя", 100),
                new ReportColumn("Отчество", 100),
                new ReportColumn("Дата рождения", 150), 
                new ReportColumn("Медаль/Кр.дип.", 150), 
                new ReportColumn("Телефон", 120));
            foreach (var groupSpeciality in root.Enrollees.OrderBy(x => Helper.SpecialityById(x.IdSpeciality))
                                                          .GroupBy(x => x.IdSpeciality))
            {
                report.ReportRows.Add(0, "Специальность", $"{Helper.SpecialityById(groupSpeciality.Key)}", "", "", "", "");
                foreach (var enrollee in groupSpeciality.OrderBy(x => x.ToString()))
                {
                    report.ReportRows.Add(1, $"{enrollee.Surname}", $"{enrollee.FirstName}", $"{enrollee.LastName}", 
                                          $"{enrollee.BirthDay.ToShortDateString()}",
                                          $"{(enrollee.GoldMedal ? "Есть":"Нет")}", $"{enrollee.PhoneNumber}");
                }
            }
            // определение обработчика печати страницы отчета
            report.PrintPage = (o, e, rect, offset) =>
            {
                SizeF strSize = new SizeF();
                var strPoint = offset;
                strPoint.X = rect.X;
                strPoint.Y -= 10;
                using (var headerfont = new Font("Arial", 12, FontStyle.Italic))
                    strSize = e.Graphics.MeasureString("Xxy", headerfont);
                foreach (var row in report.ReportRows)
                {
                    if (row.Level == 0)
                    {
                        strPoint.X = rect.X;
                        strPoint.Y += 10;
                        string value; // здесь будет значение
                        for (var i = 0; i < report.ReportColumns.Count; i++)
                        {
                            value = row.Items[i];
                            if (i == 0)
                                using (var rowfont = new Font("Arial", 12, FontStyle.Bold | FontStyle.Italic))
                                    e.Graphics.DrawString(value, rowfont, Brushes.Navy, strPoint);
                            else
                                using (var rowfont = new Font("Arial", 12, FontStyle.Bold))
                                    e.Graphics.DrawString(value, rowfont, Brushes.Black, strPoint);

                            strPoint.X += report.ReportColumns[i].Width;
                        }
                        strPoint.Y += strSize.Height;
                    }
                    else if (row.Level == 1)
                    {
                        strPoint.X = rect.X;
                        strPoint.Y += 5;
                        // Печать заголовка таблицы
                        strPoint.X = rect.X;
                        using (var headerfont = new Font("Arial", 11, FontStyle.Italic))
                        using (var sf = new StringFormat())
                        {
                            sf.Alignment = StringAlignment.Center;
                            sf.LineAlignment = StringAlignment.Center;
                            foreach (var header in report.ReportColumns)
                            {
                                strSize = e.Graphics.MeasureString(header.Text, headerfont);
                                var r = new Rectangle(Point.Ceiling(strPoint),
                                    new Size(header.Width, (int)strSize.Height));
                                e.Graphics.DrawString(header.Text, headerfont, Brushes.Navy, r, sf);
                                strPoint.X += header.Width;
                            }
                        }
                        // Печать строк таблицы
                        strPoint.Y += strSize.Height + 10;
                        using (var rowfont = new Font("Arial", 10, FontStyle.Regular))
                        using (var sf = new StringFormat())
                        {
                            sf.Alignment = StringAlignment.Center;
                            sf.LineAlignment = StringAlignment.Center;
                            strPoint.X = rect.X;
                            string value; // здесь будет значение
                            for (var i = 0; i < report.ReportColumns.Count; i++)
                            {
                                value = row.Items[i];
                                var r = new Rectangle(Point.Ceiling(strPoint),
                                    new Size(report.ReportColumns[i].Width, (int)strSize.Height));
                                e.Graphics.DrawLine(Pens.Black, r.Left, r.Bottom, r.Left + r.Width, r.Bottom);
                                e.Graphics.DrawString(value, rowfont, Brushes.Black, r, sf);
                                strPoint.X += report.ReportColumns[i].Width;
                            }
                            strPoint.Y += strSize.Height;
                        }
                    }
                }
            };
            return report;
        }
    }
}
