using System.Collections.Generic;

namespace Reports
{
    /// <summary>
    /// Колонки (заголовки) отчета
    /// </summary>
    public class ReportColumns : List<ReportColumn>
    {
        /// <summary>
        /// Добавить заголовки
        /// </summary>
        /// <param name="args"></param>
        public void Add(params ReportColumn[] args)
        {
            foreach (var item in args)
                base.Add(item);
        }
    }

    /// <summary>
    /// Ширины колонок (заголовков) отчета
    /// </summary>
    public class ReportColumn
    {
        public ReportColumn(string text = "", int width = 100)
        {
            Text = text;
            Width = width;
        }

        public string Text { get; set; }
        public int Width { get; set; } = 100;
    }

}
