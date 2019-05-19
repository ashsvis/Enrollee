using EnrolleeModel;
using Reports;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViewGenerator;

namespace EnrolleeQuestionnaire
{
    /// <summary>
    /// Класс главной формы программы
    /// </summary>
    public partial class MainForm : Form
    {
        private Root _root = new Root();                    // ссылка на корневой класс модели
        public static MattersForm MattersForm;              // ссылка на форму справочника предметов
        public static SpecialitiesForm SpecialitiesForm;    // ссылка на форму справочника специальностей
        public static PassMattersForm PassMattersForm;      // ссылка на форму справочника форм сдачи предметов
        public static EnrolleesForm EnrolleesForm;          // ссылка на форму списка анкет абитуриентов

        /// <summary>
        /// Конструктор главной формы
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            // передаем ссылку на корневой класс модели в класс-помощник
            Helper.DefineRoot(_root);
            // прицепляем обработчик события при ошибке ввода для панелей ввода данных
            GridPanelBuilder.Error += GridPanelBuilder_Error;
        }

        /// <summary>
        /// Обработчик события первой загрузки главной формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnrolleeForm_Load(object sender, EventArgs e)
        {
            // загрузка базы
            if (!SaverLoader.RestoreTables(_root, Properties.Settings.Default.ConnectionString))
            {
                // если загрузка с сервера не произошла, то пытаемся загрузится с локального файла
                var fileName = Path.ChangeExtension(Application.ExecutablePath, ".bin");
                if (File.Exists(fileName))
                {
                    _root = SaverLoader.LoadFromFile(fileName);
                    // при загрузке из файла корневой объект вновь создается, поэтому снова передаем ссылку на него
                    Helper.DefineRoot(_root);
                    // регистрируем таблицы сущностей после загрузки из файла
                    _root.RegistryTables();
                }
            }
            // ошибки операций с базой данных сохраняются в переменной OperationResult
            var result = SaverLoader.OperationResult;
            // показываем результат
            tsslStatusLabel.Text = string.IsNullOrWhiteSpace(result)
                                ? "Готово" : result.Substring(0, result.IndexOf('.') + 1);
            statusStrip1.Refresh();
            // небольшая задержка для показа заставки
            Thread.Sleep(1000);
            // заставку убираем
            Program.Splash.Close();
        }

        /// <summary>
        /// Обработчик при попытке закрыть главную форму
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnrolleeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // сохраняем базу в локальный файл
            SaverLoader.SaveToFile(Path.ChangeExtension(Application.ExecutablePath, ".bin"), _root);
            tsslStatusLabel.Text = "Сохранение данных на сервере...";
            statusStrip1.Refresh();
            // сохраняем базу на сервере
            SaverLoader.StoreTables(_root, Properties.Settings.Default.ConnectionString);
            var result = SaverLoader.OperationResult;
            // показываем результат
            tsslStatusLabel.Text = string.IsNullOrWhiteSpace(result)
                                ? "Готово" : result.Substring(0, result.IndexOf('.') + 1);
            statusStrip1.Refresh();
            // спрашиваем пользователя, закрывать ли приложение
            e.Cancel = MessageBox.Show(this, "Закрыть приложение?", "Выход", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes;
        }

        /// <summary>
        /// Обработчик события ошибок ввода данных с панелей свойств
        /// </summary>
        /// <param name="message">Текст ошибки</param>
        /// <param name="caption">Текст заголовка окна с ошибкой</param>
        private void GridPanelBuilder_Error(string message, string caption)
        {
            MessageBox.Show(this, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Метод для показа дочених форм
        /// </summary>
        /// <param name="form">Ссылка на объект формы</param>
        public static void ShowForm(Form form)
        {
            // показывае форму
            form.Show();
            // если была свернута, то разворачиваем
            if (form.WindowState == FormWindowState.Minimized)
                form.WindowState = FormWindowState.Normal;
            // перемещаем наверх
            form.BringToFront();
        }

        /// <summary>
        /// Обработчик меню выхода из приложения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiExit_Click(object sender, EventArgs e)
        {
            // закрыть приложение
            Close();
        }

        /// <summary>
        /// Обработчик меню помощи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiHelp_Click(object sender, EventArgs e)
        {
            // показываем окно "О программе"
            new AboutForm().ShowDialog();
        }

        /// <summary>
        /// Обработчик меню редактирования справочника предметов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiMatters_Click(object sender, EventArgs e)
        {
            // если ранее не показывался, то создаем новое окно
            if (MattersForm == null)
            {
                MattersForm = new MattersForm(_root);
                // присоединяем обработчик изменения видимости окна
                MattersForm.VisibleChanged += ChildForm_VisibleChanged;
            }
            // показываем окно 
            ShowForm(MattersForm);
        }

        /// <summary>
        /// Обработчик меню редактирования специальностей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiSpecialities_Click(object sender, EventArgs e)
        {
            // если ранее не показывался, то создаем новое окно
            if (SpecialitiesForm == null)
            {
                SpecialitiesForm = new SpecialitiesForm(_root);
                // присоединяем обработчик изменения видимости окна
                SpecialitiesForm.VisibleChanged += ChildForm_VisibleChanged;
            }
            // показываем окно 
            ShowForm(SpecialitiesForm);
        }

        /// <summary>
        /// Обработчик меню редактирования форм сдаваемых предметов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiPassMatters_Click(object sender, EventArgs e)
        {
            if (PassMattersForm == null)
            {
                PassMattersForm = new PassMattersForm(_root);
                PassMattersForm.VisibleChanged += ChildForm_VisibleChanged;
            }
            ShowForm(PassMattersForm);
        }

        /// <summary>
        /// Обработчик менб редактирования списка анкет абитуриентов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiQuestionnaires_Click(object sender, EventArgs e)
        {
            if (EnrolleesForm == null)
            {
                EnrolleesForm = new EnrolleesForm(_root);
                EnrolleesForm.VisibleChanged += ChildForm_VisibleChanged;
            }
            ShowForm(EnrolleesForm);
        }

        /// <summary>
        /// Обработчик изменения видимости дочерних форм
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChildForm_VisibleChanged(object sender, EventArgs e)
        {
            var frm = (Form)sender;
            this.Visible = !frm.Visible; // если дочернее окно показывается, то главное окно - прячется
        }

        private void tsmiReport1_Click(object sender, EventArgs e)
        {
            // показываем окно первого отчета
            new ReportsForm(ReportsBuilder.GetEnrolleesWithGoldMedal(_root)).ShowDialog();
        }

        private void tsmiReport2_Click(object sender, EventArgs e)
        {
            // показываем окно второго отчета
            new ReportsForm(ReportsBuilder.GetEnrolleesBySpecialityAphabet(_root)).ShowDialog();
        }

        private void tsmiReport3_Click(object sender, EventArgs e)
        {
            // показываем окно третьего отчета
            new ReportsForm(ReportsBuilder.GetEnrolleesBySpeciality(_root)).ShowDialog();
        }

        /// <summary>
        /// Вызов окна для изменения строки подключения к серверу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiConnectionString_Click(object sender, EventArgs e)
        {
            var frm = new ConnectionStringForm();
            frm.Build(Properties.Settings.Default.ConnectionString);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                Properties.Settings.Default.ConnectionString = frm.Data;
                Properties.Settings.Default.Save();
                // после изменения строки подключения база данных загружается в модель
                LoadFromBaseAsync();
            }
        }

        /// <summary>
        /// Загрузка данных из базы асинхронно
        /// </summary>
        private void LoadFromBaseAsync()
        {
            // создается отдельная задача
            Task.Run(() =>
            {
                // загрузка модели из сервера
                SaverLoader.RestoreTables(_root, Properties.Settings.Default.ConnectionString);
                // создаем тело метода для показа результатов загрузки
                var method = new MethodInvoker(() =>
                {
                    var result = SaverLoader.OperationResult;
                    tsslStatusLabel.Text = string.IsNullOrWhiteSpace(result)
                                     ? "Готово" : result.Substring(0, result.IndexOf('.') + 1);
                    statusStrip1.Refresh();
                });
                // вызываем метод показа результатов из отдельного потока
                if (InvokeRequired)
                    BeginInvoke(method);
                else
                    method();
            });
        }
    }
}
