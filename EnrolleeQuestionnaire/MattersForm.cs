using EnrolleeModel;
using System.Windows.Forms;
using ViewGenerator;

namespace EnrolleeQuestionnaire
{
    /// <summary>
    /// Класс формы редактирования предметов
    /// </summary>
    public partial class MattersForm : Form
    {
        GridPanel _gridPanel;
        Root _root;

        public MattersForm(Root root)
        {
            InitializeComponent();
            _root = root;
            // содаем панель с таблицей автоматически по классу и списку
            _gridPanel = GridPanelBuilder.BuildPropertyPanel(root, new Matter(), root.Matters);
            panel1.Controls.Add(_gridPanel);
        }

        /// <summary>
        /// Прячем форму при закрытии
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MattersForm_FormClosing(object sender, FormClosingEventArgs e)
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
