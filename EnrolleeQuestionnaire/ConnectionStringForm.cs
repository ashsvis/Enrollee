using System;
using System.Windows.Forms;

namespace EnrolleeQuestionnaire
{
    /// <summary>
    /// Класс формы редактирования строки подключения к серверу
    /// </summary>
    public partial class ConnectionStringForm : Form
    {
        public string Data { get; set; }

        public ConnectionStringForm()
        {
            InitializeComponent();
            btnOk.Enabled = false;
        }

        public void Build(string text)
        {
            tbContent.Text = text;
        }

        /// <summary>
        /// Проверяем на непустой текст
        /// </summary>
        public void ApplyUpdate()
        {
            btnOk.Enabled = !string.IsNullOrWhiteSpace(tbContent.Text);
            Data = tbContent.Text;
        }

        private void tbContent_TextChanged(object sender, EventArgs e)
        {
            ApplyUpdate();
        }
    }
}
