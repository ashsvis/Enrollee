using EnrolleeModel;
using System;
using System.Windows.Forms;
using ViewGenerator;

namespace EnrolleeQuestionnaire
{
    /// <summary>
    /// Класс формы редактирования анкет абитуриентов
    /// </summary>
    public partial class EnrolleesForm : Form
    {
        Root _root;
        GridPanel _panel;

        public EnrolleesForm(Root root)
        {
            InitializeComponent();
            _root = root;
            // панель с таблицей создается автоматичекси по типу класса и используя список из модели
            _panel = GridPanelBuilder.BuildPropertyPanel(root, new Enrollee(), root.Enrollees);
            panel1.Controls.Add(_panel);
            // заполняем список для фильтра специальностей
            tscbSpeciality.Items.Add(new SpecialityItem { IdSpeciality = Guid.Empty, Name = "Все специальности" });
            foreach (var item in root.Specialities)
            {
                tscbSpeciality.Items.Add(new SpecialityItem { IdSpeciality = item.IdSpeciality, Name = item.ToString() });
            }
            tscbSpeciality.SelectedItem = tscbSpeciality.Items[0];
        }

        /// <summary>
        /// При закрытии форма прячется
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnrolleesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void tsmiBack_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Обработчик выбора фильтра специалности
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tscbSpeciality_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = (SpecialityItem)tscbSpeciality.SelectedItem;
            // выбран фильтр для всех специальностей
            if (item.IdSpeciality == Guid.Empty)
            {
                _panel = GridPanelBuilder.BuildPropertyPanel(_root, new Enrollee(), _root.Enrollees);
                panel1.Controls.Add(_panel);
            }
            else // выбрана одна из специальностей
            {
                // таблица строится с учетом выбранной специальности
                _panel = GridPanelBuilder.BuildPropertyPanel(_root, new Enrollee(), _root.Enrollees,
                    _root.Enrollees.FilteredBySpeciality(item.IdSpeciality), "IdSpeciality", item.IdSpeciality);
                panel1.Controls.Add(_panel);
            }
            // прежняя панель удаляется
            panel1.Controls.RemoveAt(0);
        }

        /// <summary>
        /// Обработчик изменения текста в строке поиска по фамилии
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tstbFind_TextChanged(object sender, EventArgs e)
        {
            // вызываем метод поиска по фамилии
            GridPanelBuilder.FindText(_panel, tstbFind.Text);
        }
    }

    /// <summary>
    /// Вспомогательный класс для списка фильтров по специальности
    /// </summary>
    public class SpecialityItem
    {
        public Guid IdSpeciality { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
