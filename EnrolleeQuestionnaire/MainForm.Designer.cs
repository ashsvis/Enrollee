namespace EnrolleeQuestionnaire
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmiEnterEditData = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiQuestionnaires = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSpecialities = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMatters = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPassMatters = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiReports = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiReport1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiReport2 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiReport3 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTuning = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiConnectionString = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsslStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiEnterEditData,
            this.tsmiReports,
            this.tsmiHelp,
            this.tsmiTuning,
            this.tsmiExit});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(445, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmiEnterEditData
            // 
            this.tsmiEnterEditData.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiQuestionnaires,
            this.tsmiSpecialities,
            this.tsmiMatters,
            this.tsmiPassMatters});
            this.tsmiEnterEditData.Name = "tsmiEnterEditData";
            this.tsmiEnterEditData.Size = new System.Drawing.Size(160, 20);
            this.tsmiEnterEditData.Text = "Ввод и коррекция данных";
            // 
            // tsmiQuestionnaires
            // 
            this.tsmiQuestionnaires.Name = "tsmiQuestionnaires";
            this.tsmiQuestionnaires.Size = new System.Drawing.Size(324, 22);
            this.tsmiQuestionnaires.Text = "Анкетные данные";
            this.tsmiQuestionnaires.Click += new System.EventHandler(this.tsmiQuestionnaires_Click);
            // 
            // tsmiSpecialities
            // 
            this.tsmiSpecialities.Name = "tsmiSpecialities";
            this.tsmiSpecialities.Size = new System.Drawing.Size(324, 22);
            this.tsmiSpecialities.Text = "Специальности";
            this.tsmiSpecialities.Click += new System.EventHandler(this.tsmiSpecialities_Click);
            // 
            // tsmiMatters
            // 
            this.tsmiMatters.Name = "tsmiMatters";
            this.tsmiMatters.Size = new System.Drawing.Size(324, 22);
            this.tsmiMatters.Text = "Предметы";
            this.tsmiMatters.Click += new System.EventHandler(this.tsmiMatters_Click);
            // 
            // tsmiPassMatters
            // 
            this.tsmiPassMatters.Name = "tsmiPassMatters";
            this.tsmiPassMatters.Size = new System.Drawing.Size(324, 22);
            this.tsmiPassMatters.Text = "Формы сдачи предметов по специальностям";
            this.tsmiPassMatters.Click += new System.EventHandler(this.tsmiPassMatters_Click);
            // 
            // tsmiReports
            // 
            this.tsmiReports.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiReport1,
            this.tsmiReport2,
            this.tsmiReport3});
            this.tsmiReports.Name = "tsmiReports";
            this.tsmiReports.Size = new System.Drawing.Size(60, 20);
            this.tsmiReports.Text = "Отчёты";
            // 
            // tsmiReport1
            // 
            this.tsmiReport1.Name = "tsmiReport1";
            this.tsmiReport1.Size = new System.Drawing.Size(667, 22);
            this.tsmiReport1.Text = "Анкетные данные абитуриентов, имеющих красный диплом или медаль";
            this.tsmiReport1.Click += new System.EventHandler(this.tsmiReport1_Click);
            // 
            // tsmiReport2
            // 
            this.tsmiReport2.Name = "tsmiReport2";
            this.tsmiReport2.Size = new System.Drawing.Size(667, 22);
            this.tsmiReport2.Text = "Все инициалы абитуриентов по специальностям в алфавитном порядке с указанием сдав" +
    "аемых предметов";
            this.tsmiReport2.Click += new System.EventHandler(this.tsmiReport2_Click);
            // 
            // tsmiReport3
            // 
            this.tsmiReport3.Name = "tsmiReport3";
            this.tsmiReport3.Size = new System.Drawing.Size(667, 22);
            this.tsmiReport3.Text = "Анкетные данные по специальностям";
            this.tsmiReport3.Click += new System.EventHandler(this.tsmiReport3_Click);
            // 
            // tsmiHelp
            // 
            this.tsmiHelp.Name = "tsmiHelp";
            this.tsmiHelp.Size = new System.Drawing.Size(65, 20);
            this.tsmiHelp.Text = "Справка";
            this.tsmiHelp.Click += new System.EventHandler(this.tsmiHelp_Click);
            // 
            // tsmiTuning
            // 
            this.tsmiTuning.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiConnectionString});
            this.tsmiTuning.Name = "tsmiTuning";
            this.tsmiTuning.Size = new System.Drawing.Size(78, 20);
            this.tsmiTuning.Text = "Настройка";
            // 
            // tsmiConnectionString
            // 
            this.tsmiConnectionString.Name = "tsmiConnectionString";
            this.tsmiConnectionString.Size = new System.Drawing.Size(257, 22);
            this.tsmiConnectionString.Text = "Строка подключения к серверу...";
            this.tsmiConnectionString.Click += new System.EventHandler(this.tsmiConnectionString_Click);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.Size = new System.Drawing.Size(53, 20);
            this.tsmiExit.Text = "Выход";
            this.tsmiExit.Click += new System.EventHandler(this.tsmiExit_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.Font = new System.Drawing.Font("Monotype Corsiva", 20.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(32, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(380, 81);
            this.label1.TabIndex = 1;
            this.label1.Text = "Автоматизация работы приёмной комиссии вуза";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 133);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(445, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsslStatusLabel
            // 
            this.tsslStatusLabel.Name = "tsslStatusLabel";
            this.tsslStatusLabel.Size = new System.Drawing.Size(64, 17);
            this.tsslStatusLabel.Text = "Загрузка...";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 155);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Абитуриент";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EnrolleeForm_FormClosing);
            this.Load += new System.EventHandler(this.EnrolleeForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiEnterEditData;
        private System.Windows.Forms.ToolStripMenuItem tsmiReports;
        private System.Windows.Forms.ToolStripMenuItem tsmiHelp;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.ToolStripMenuItem tsmiQuestionnaires;
        private System.Windows.Forms.ToolStripMenuItem tsmiSpecialities;
        private System.Windows.Forms.ToolStripMenuItem tsmiMatters;
        private System.Windows.Forms.ToolStripMenuItem tsmiReport1;
        private System.Windows.Forms.ToolStripMenuItem tsmiReport2;
        private System.Windows.Forms.ToolStripMenuItem tsmiReport3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem tsmiPassMatters;
        private System.Windows.Forms.ToolStripMenuItem tsmiTuning;
        private System.Windows.Forms.ToolStripMenuItem tsmiConnectionString;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatusLabel;
    }
}

