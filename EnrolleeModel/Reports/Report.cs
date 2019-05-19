using System;
using System.Drawing;
using System.Drawing.Printing;

namespace Reports
{
    /// <summary>
    /// Отчёт, содержит заголовки и строки со столбцами
    /// </summary>
    public class Report
    {
        public string Caption { get; set; } = string.Empty; // заголовок отчета
        public ReportColumns ReportColumns { get; set; } = new ReportColumns(); // колонки отчета
        public ReportRows ReportRows { get; set; } = new ReportRows(); // строки отчета
        public Action<object, PrintPageEventArgs, RectangleF, PointF> PrintPage { get; set; }
        /// <summary>
        /// Инициализация
        /// </summary>
        public void Clear()
        {
            Caption = string.Empty;
            ReportColumns.Clear();
            ReportRows.Clear();
        }
    }
}
