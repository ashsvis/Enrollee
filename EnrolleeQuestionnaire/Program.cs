using System;
using System.Windows.Forms;

namespace EnrolleeQuestionnaire
{
    static class Program
    {
        public static SplashForm Splash; // ссылка на форму-заставку

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // создаем и показываем форму заставку 
            Splash = new SplashForm();
            Splash.Show();
            Splash.Refresh();
            // запускаем главную форму
            Application.Run(new MainForm());
        }
    }
}
