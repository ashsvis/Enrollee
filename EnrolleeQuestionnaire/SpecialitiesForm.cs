using EnrolleeModel;
using System.Windows.Forms;
using ViewGenerator;

namespace EnrolleeQuestionnaire
{
    /// <summary>
    /// Класс формы редактирования специальностей
    /// </summary>
    public partial class SpecialitiesForm : Form
    {
        GridPanel _gridPanel;
        Root _root;

        public SpecialitiesForm(Root root)
        {
            InitializeComponent();
            _root = root;
            // создаем панель с таблицей автоматически по классу и списку из модели
            _gridPanel = GridPanelBuilder.BuildPropertyPanel(root, new Speciality(), root.Specialities);
            panel1.Controls.Add(_gridPanel);
        }

        /// <summary>
        /// Прячем форму при закрытии
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpecialitiesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void tsmiBack_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
