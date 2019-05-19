using EnrolleeModel;
using System;
using System.Windows.Forms;
using ViewGenerator;

namespace EnrolleeQuestionnaire
{
    /// <summary>
    /// Класс для редактирования форм сдачи предметов
    /// </summary>
    public partial class PassMattersForm : Form
    {
        GridPanel _gridPanel;
        Root _root;

        public PassMattersForm(Root root)
        {
            InitializeComponent();
            _root = root;
            // создаем панель с таблицей автоматически по классу и списку из модели
            _gridPanel = GridPanelBuilder.BuildPropertyPanel(root, new PassMatter(), root.PassMatters);
            panel1.Controls.Add(_gridPanel);
        }

        /// <summary>
        /// Прячем форму при закрытии
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PassMattersForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void tsmiBack_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
