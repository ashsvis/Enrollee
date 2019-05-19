using System;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace ViewGenerator
{
    /// <summary>
    /// Класс для формирования панели свойств
    /// </summary>
    public class PropertyPanelBuilder
    {
        // ссылка на форму диалога
        private readonly Form _form;

        // частный конструктор
        private PropertyPanelBuilder(Form form)
        {
            _form = form;
        }

        /// <summary>
        /// Показ формы с полями для класса параметра
        /// </summary>
        /// <param name="userClass">Класс для формирования полей редактирования</param>
        /// <returns>Объект формы диалога</returns>
        public static Form ShowPropertyFormDialog(object userModel, object userClass, string fontName = "Microsoft Sans Serif", float fontSize = 8.25f)
        {
            // заготовка для формы диалога
            var frm = new Form()
            {
                Text = GetClassCaption(userClass),
                AutoSize = true,                                // размер подстраивается под содержимое
                AutoSizeMode = AutoSizeMode.GrowAndShrink,      // автоматически увеличивает или уменьшает размер
                StartPosition = FormStartPosition.CenterParent, // показывает форму по центру родительского окна
                FormBorderStyle = FormBorderStyle.FixedDialog,  // формат рамки формы
                MaximizeBox = false,                            // не показывать кнопку "Развернуть"
                MinimizeBox = false,                            // не показывать кнопку "Свернуть"
                ShowInTaskbar = false                           // не показывать в панели задач
            };
            frm.Font = new Font(fontName, fontSize);
            // создаём заготовку панели свойств 
            var ppb = new PropertyPanelBuilder(frm);
            // размещаем панель на форме
            frm.Controls.Add(ppb.BuildPropertyPanel(userModel, userClass));
            // показываем форму диалога
            frm.ShowDialog();
            // возвращаем ссылку на объект формы
            return frm;
        }

        /// <summary>
        /// Построение панели свойств по ссылке на экземпляр класса
        /// </summary>
        /// <param name="userClass">Cсылка на экземпляр класса</param>
        /// <returns>Ссылка на панель</returns>
        public UserControl BuildPropertyPanel(object userModel, object userClass)
        {
            // накопительный список действий, выполняемых при возврате изменённого значения в свойство объекта класса userClass
            var actions = new List<Action>();
            // кнопка "Ввод" панели свойств для диалога формы
            var btnOk = new Button
            {
                Text = "Ввод",
                DialogResult = DialogResult.None,   // специально не содержит закрытия по умолчанию
                Enabled = false,                    // изначально запрещена
                Anchor = AnchorStyles.Right,        // прижимается к правой стороне
                AutoSize = true,                    // увеличивается, если текст на кнопке не помещается
                Tag = _form                         // здесь передаётся ссылка на форму диалога, для обработчика нажатия
            };
            // кнопка "Отмена" панели свойств для диалога формы
            var btnCancel = new Button
            {
                Text = "Отменить",
                DialogResult = DialogResult.Cancel, // содержит по умолчанию отмену редактирования и закрытие формы диалога
                Anchor = AnchorStyles.Right,        // прижимается к правой стороне
                AutoSize = true                     // увеличивается, если текст на кнопке не помещается
            };
            // заготовка для панели свойств
            var userControl = new UserControl
            {
                AutoSize = true,                            // размер подстраивается под содержимое
                AutoSizeMode = AutoSizeMode.GrowAndShrink   // автоматически увеличивает или уменьшает размер
            };
            var type = userClass.GetType();         // получаем тип объекта, переданного через параметр
            MemberInfo[] m = type.GetProperties();  // получаем массив свойств объекта
            // создаём сетку для компоновки
            var grid = new TableLayoutPanel()
            {
                Padding = new Padding(10),                  // внеший отступ от краёв сетки, для красоты
                AutoSize = true,                            // размер подстраивается под содержимое
                AutoSizeMode = AutoSizeMode.GrowAndShrink,  // автоматически увеличивает или уменьшает размер
                ColumnCount = 2                             // будет два столбца - слева название, справа - поле редактирования
            };
            // добавим два столбца в сетку, с автоматическим изменением размера (поведение по умолчанию)
            grid.ColumnStyles.Add(new ColumnStyle());
            grid.ColumnStyles.Add(new ColumnStyle());
            // добавим сетку на панель свойств
            userControl.Controls.Add(grid);
            // вначале строк в сетке нет
            var row = 0;            
            foreach (var info in m) // для каждого свойства из массива свойств
            {
                // получаем ссылку на свойство по его имени
                var prop = type.GetProperty(info.Name);
                // получаем наименование свойства из дескриптора
                var caption = GetPropertyCaption(userClass, prop);
                // метка поля редактирования (размещаемая в первом столбце сетки, слева)
                var label = new Label
                {
                    Name = "lb" + prop.Name,    // имя метки, это имя свойства с перфиксом "lb"
                    // содержимое метки - наименование свойства из дескриптора, а если нет, то имя свойства из класса 
                    Text = (string.IsNullOrWhiteSpace(caption) ? prop.Name : caption) + ":",
                    Dock = DockStyle.Fill,                      // растягивается на всю возможную поверхность ячейки в сетке 
                    TextAlign = ContentAlignment.MiddleRight,   // текст метки прижимается вправо и по центру вертикального размера
                    AutoSize = true                             // размер подстраивается под содержимое
                };
                TextBox textBox;                // ссылка на редактор текстового свойства
                NumericUpDown numericUpDown;    // ссылка на редактор числового свойства
                ComboBox comboBox;              // ссылка на селектор значения
                DateTimePicker dateTimePicker;  // ссылка на редактор значения даты
                CheckBox checkBox;              // ссылка на редактор логического свойства
                // получим имя типа свойства как строку символов
                var typeName = prop.PropertyType.ToString();
                // для часто встречающихся случаев
                switch (typeName)
                {
                    case "System.String": // для строковых свойств
                        grid.RowCount = row + 1;            // указываем количество строк сетки
                        grid.RowStyles.Add(new RowStyle()); // добавляем стиль строки сетки, с автоматическим изменением размера (поведение по умолчанию)
                        grid.Controls.Add(label, 0, row);   // добавляем метку в первый столбец строки сетки
                        // формируем редактор текстовой строки
                        textBox = new TextBox
                        {
                            Name = "tb" + prop.Name,                        // имя компонента, это имя свойства с перфиксом "tb"
                            Text = prop.GetValue(userClass)?.ToString(),    // заполняем текст значением из свойства
                            Dock = DockStyle.Fill,                          // занимает всю ячейку сетки
                            BorderStyle = BorderStyle.FixedSingle,          // стильная рамка
                            Width = 160,                                    // ширина текстового поля (так настроил)
                            TabIndex = row                                  // индекс перехода по табуляции
                        };
                        // устанавливаем количество символов значения из атрибута DataRangeAttibute
                        SetRange(userClass, prop, textBox);
                        // устанавливаем размер текстового редактора и многострочный режим
                        SetSize(userClass, prop, textBox);
                        // установка символа защиты пароля по атрибуту DataPassword
                        var safeEditMode = SetPasswordMode(userClass, prop, textBox);
                        // при изменении содержимого в текстовом поле кнопка "Ввод" становится доступной
                        textBox.TextChanged += (o, e) => { btnOk.Enabled = true; };
                        // добавляем редактор текста во второй столбец строки сетки
                        grid.Controls.Add(textBox, 1, row);
                        // добавляем обработчик для этого редактора в список действий
                        actions.Add(safeEditMode ? PasswordValue(userClass, prop, textBox) : TextValue(userClass, prop, textBox));
                        if (safeEditMode)
                        {
                            row++;
                            grid.RowCount = row + 1;            // указываем количество строк сетки
                            grid.RowStyles.Add(new RowStyle()); // добавляем стиль строки сетки, с автоматическим изменением размера (поведение по умолчанию)
                            label = new Label
                            {
                                Name = "lbRepeat" + prop.Name,    // имя метки, это имя свойства с перфиксом "lb"
                                                                  // содержимое метки - наименование свойства из дескриптора, а если нет, то имя свойства из класса 
                                Text = (string.IsNullOrWhiteSpace(caption) ? $"{prop.Name} (repeat):" : $"{caption} (ещё раз):"),
                                Dock = DockStyle.Fill,                      // растягивается на всю возможную поверхность ячейки в сетке 
                                TextAlign = ContentAlignment.MiddleRight,   // текст метки прижимается вправо и по центру вертикального размера
                                AutoSize = true                             // размер подстраивается под содержимое
                            };
                            grid.Controls.Add(label, 0, row);   // добавляем метку в первый столбец строки сетки
                            // формируем редактор повтора текстовой строки
                            var textBoxRepeat = new TextBox
                            {
                                Name = "tbRepeat" + prop.Name,                  // имя компонента, это имя свойства с перфиксом "tb"
                                Text = prop.GetValue(userClass)?.ToString(),    // заполняем текст значением из свойства
                                Dock = DockStyle.Fill,                          // занимает всю ячейку сетки
                                BorderStyle = BorderStyle.FixedSingle,          // стильная рамка
                                Width = 160,                                    // ширина текстового поля (так настроил)
                                TabIndex = row                                  // индекс перехода по табуляции
                            };
                            // устанавливаем количество символов значения из атрибута DataRangeAttibute
                            SetRange(userClass, prop, textBoxRepeat);
                            SetPasswordMode(userClass, prop, textBoxRepeat);
                            grid.Controls.Add(textBoxRepeat, 1, row);
                            // добавляем обработчик для этого редактора в список действий
                            actions.Add(() =>
                            {
                                if (string.Compare(textBox.Text, textBoxRepeat.Text) != 0)
                                    throw new Exception("Значение паролей не совпадают!");
                            });
                        }
                        break;
                    case "System.Int32":    // для целочисленных свойств
                    case "System.Decimal":  // для свойств с ценой
                        grid.RowCount = row + 1;            // указываем количество строк сетки
                        grid.RowStyles.Add(new RowStyle()); // добавляем стиль строки сетки, с автоматическим изменением размера (поведение по умолчанию)
                        grid.Controls.Add(label, 0, row);   // добавляем метку в первый столбец строки сетки
                        // формируем редактор числового значения
                        numericUpDown = new NumericUpDown
                        {
                            Name = "nud" + prop.Name,                       // имя компонента, это имя свойства с перфиксом "nud"
                            Dock = DockStyle.Left,                          // прижимаем влево
                            TextAlign = HorizontalAlignment.Right,          // текст прижимаем вправо
                            BorderStyle = BorderStyle.FixedSingle,
                            DecimalPlaces = typeName == "System.Decimal" ? 2 : 0,       // количество знаков для копеек для decimal
                            Maximum = typeName == "System.Decimal" ? 1000000 : 32767,   // значения границ
                            Width = 70,
                            TabIndex = row
                        };
                        // устанавливаем диапазон изменения значения из атрибута DataRangeAttibute
                        SetRange(userClass, prop, numericUpDown);
                        // получаем текущее значение
                        var value = Convert.ToDecimal(prop.GetValue(userClass));
                        // если полученное значение в разрешенном диапазоне
                        if (value >= numericUpDown.Minimum && value <= numericUpDown.Maximum)
                            numericUpDown.Value = value;    // присваиваем как значение редактора числа
                        numericUpDown.TextChanged += (o, e) => { btnOk.Enabled = true; };
                        numericUpDown.ValueChanged += (o, e) => { btnOk.Enabled = true; };
                        // добавляем редактор чисел во второй столбец строки сетки
                        grid.Controls.Add(numericUpDown, 1, row);
                        // добавляем обработчик для этого редактора в список действий
                        actions.Add(NumberValue(userClass, prop, numericUpDown));
                        break;
                    case "System.Guid":         // для ключевых значений свойств
                        if (row == 0) break;    // принимаем, что если первым в классе объявлено свойство типа Guid - то это ключевое свойство и его не показываем
                        if (CheckTableFilterableAttribute(userClass, prop)) break;
                        grid.RowCount = row + 1;            // указываем количество строк сетки
                        grid.RowStyles.Add(new RowStyle()); // добавляем стиль строки сетки, с автоматическим изменением размера (поведение по умолчанию)
                        grid.Controls.Add(label, 0, row);   // добавляем метку в первый столбец строки сетки
                        // формируем селектор ключевых значении
                        comboBox = new ComboBox
                        {
                            Name = "cb" + prop.Name,                        // имя компонента, это имя свойства с перфиксом "cb"
                            Dock = DockStyle.Fill,
                            Width = 160,
                            DropDownStyle = ComboBoxStyle.DropDownList,     // только список для выбора
                            TabIndex = row
                        };
                        FillLookupCombobox(userModel, userClass, prop, comboBox);
                        // при подтверждении выбора разрешаем кнопку "Ввод"
                        comboBox.SelectionChangeCommitted += (o, e) => { btnOk.Enabled = true; };
                        // добавляем селектор во второй столбец строки сетки
                        grid.Controls.Add(comboBox, 1, row);
                        // добавляем обработчик для этого редактора в список действий
                        actions.Add(GuidValue(userClass, prop, comboBox));
                        break;
                    case "System.DateTime": // для свойств с датой
                        grid.RowCount = row + 1;
                        grid.RowStyles.Add(new RowStyle());
                        grid.Controls.Add(label, 0, row);
                        DateTime? dtvalue = (DateTime)prop.GetValue(userClass);
                        dateTimePicker = new DateTimePicker
                        {
                            Name = "dtp" + prop.Name,
                            Dock = DockStyle.Fill,
                            Width = 70,
                            TabIndex = row
                        };
                        // контролируем минимальное значение даты
                        if (dtvalue != null && dtvalue < dateTimePicker.MinDate) dtvalue = DateTime.Now;
                        dateTimePicker.Text = dtvalue?.ToString();
                        dateTimePicker.TextChanged += (o, e) => { btnOk.Enabled = true; };
                        dateTimePicker.ValueChanged += (o, e) => { btnOk.Enabled = true; };
                        // добавляем редактор даты во второй столбец строки сетки
                        grid.Controls.Add(dateTimePicker, 1, row);
                        // добавляем обработчик для этого редактора в список действий
                        actions.Add(DateTimeValue(userClass, prop, dateTimePicker));
                        break;
                    case "System.Boolean": // для логических свойств
                        grid.RowCount = row + 1;
                        grid.RowStyles.Add(new RowStyle());
                        grid.Controls.Add(label, 0, row);
                        checkBox = new CheckBox
                        {
                            Name = "cb" + prop.Name,
                            Text = (bool)prop.GetValue(userClass) ? "Да" : "Нет",
                            Dock = DockStyle.Fill,
                            Width = 70,
                            TabStop = true,
                            TabIndex = row
                        };
                        // получаем текущее значение
                        checkBox.Checked = Convert.ToBoolean(prop.GetValue(userClass));
                        checkBox.CheckedChanged += (o, e) => 
                        {
                            var ch = (CheckBox)o;
                            ch.Text = ch.Checked ? "Да" : "Нет";
                            btnOk.Enabled = true;
                        };
                        // добавляем редактор даты во второй столбец строки сетки
                        grid.Controls.Add(checkBox, 1, row);
                        // добавляем обработчик для этого редактора в список действий
                        actions.Add(BooleanValue(userClass, prop, checkBox));
                        break;
                    default:
                        var baseTypeName = prop.PropertyType.BaseType.FullName;
                        switch (baseTypeName)
                        {
                            case "System.Enum":
                                grid.RowCount = row + 1;            // указываем количество строк сетки
                                grid.RowStyles.Add(new RowStyle()); // добавляем стиль строки сетки, с автоматическим изменением размера (поведение по умолчанию)
                                grid.Controls.Add(label, 0, row);   // добавляем метку в первый столбец строки сетки
                                                                    // формируем селектор ключевых значении
                                comboBox = new ComboBox
                                {
                                    Name = "cb" + prop.Name,                        // имя компонента, это имя свойства с перфиксом "cb"
                                    Dock = DockStyle.Fill,
                                    Width = 160,
                                    DropDownStyle = ComboBoxStyle.DropDownList,     // только список для выбора
                                    TabIndex = row
                                };
                                FillEnumCombobox(userModel, userClass, prop, comboBox);
                                // при подтверждении выбора разрешаем кнопку "Ввод"
                                comboBox.SelectionChangeCommitted += (o, e) => { btnOk.Enabled = true; };
                                // добавляем селектор во второй столбец строки сетки
                                grid.Controls.Add(comboBox, 1, row);
                                // добавляем обработчик для этого редактора в список действий
                                actions.Add(EnumValue(userClass, prop, comboBox));
                                break;
                        }
                        break;
                }
                row++;
            }

            #region Панель кнопок "Ввод" и "Отмена"
            
            // создадим панель для размещения кнопок
            var flow = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Right,                     // прижимается вправо
            };
            // добавляем строку в сетку
            grid.RowCount = row + 1;
            grid.RowStyles.Add(new RowStyle()); // с авторазмером
            grid.Controls.Add(flow, 0, row);    // в первый столбец
            grid.SetColumnSpan(flow, 2);        // с растягиванием на два столбца
            // настроим кнопке ввод индекс табуляции
            btnOk.TabIndex = row + 1;
            // добавим её на панель свойств
            flow.Controls.Add(btnOk);
            // добавим обработчик нажатия кнопки "Ввод"
            btnOk.Click += (o, e) => 
            {
                var frm = (Form)((Button)o).Tag;    // получим ссылку на форму диалога
                try
                {
                    // перешлём все значения из полей редактирования в свойства объекта
                    foreach (var act in actions) act();
                    // если ошибок присвоения небыло, закрываем форму с признаком ОК
                    frm.DialogResult = DialogResult.OK;
                }
                catch (Exception ex)
                {
                    // выводим сообщение при первой ошибке
                    MessageBox.Show(frm, ex.Message, "Ошибка в значении свойства", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // форма диалога при этом не закрывается
                }
            };
            // настроим кнопке ввод индекс табуляции
            btnCancel.TabIndex = row + 2;
            // добавим её на панель свойств
            flow.Controls.Add(btnCancel);

            #endregion

            return userControl;
        }

        /// <summary>
        /// Заполнение селектора значений
        /// </summary>
        /// <param name="userModel">объект корневого объекта модели</param>
        /// <param name="userClass">объект со свойствами</param>
        /// <param name="prop">ссылка на свойство</param>
        /// <param name="comboBox">ссылка на компонент ComboBox</param>
        private void FillEnumCombobox(object userModel, object userClass, PropertyInfo prop, ComboBox comboBox)
        {
            comboBox.Items.Clear();
            var userValue = (Enum)prop.GetValue(userClass);
            foreach (var item in prop.PropertyType.GetEnumValues())
            {
                comboBox.Items.Add(item);
                if (item.ToString() == userValue.ToString())
                    comboBox.SelectedItem = item;
            }
        }

        /// <summary>
        /// Заполнение селектора значений
        /// </summary>
        /// <param name="userModel">объект корневого объекта модели</param>
        /// <param name="userClass">объект со свойствами</param>
        /// <param name="prop">ссылка на свойство</param>
        /// <param name="comboBox">ссылка на компонент ComboBox</param>
        private void FillLookupCombobox(object userModel, object userClass, PropertyInfo prop, ComboBox comboBox)
        {
            comboBox.Items.Clear();
            // получаем ссылку на коллекцию атрибутов свойства
            var attributes = TypeDescriptor.GetProperties(userClass)[prop.Name].Attributes;
            // получаем из коллекции атрибутов ссылку на атрибут источника данных свойства
            var mAttribute = (DataLookupAttribute)attributes[typeof(DataLookupAttribute)];
            if (mAttribute == null) return;
            var valueMember = mAttribute.ValueMember;    // имя ключевого свойства в коллекции
            var lookupMember = mAttribute.LookupMember;  // имя перечисления коллекции объектов
            var collection = userModel.GetType().GetProperty(lookupMember);
            if (collection == null) // коллекция LookupMember не определена в базовом классе
            {
                comboBox.Enabled = false;
                return;
            }
            var userValue = (Guid)prop.GetValue(userClass);
            foreach (var item in ((IEnumerable<object>)collection
                                   .GetValue(userModel))
                                   .OrderBy(item => item.ToString()))
            {
                comboBox.Items.Add(item);
                var itemValue = (Guid)item.GetType().GetProperty(valueMember).GetValue(item);
                if (itemValue == userValue)
                    comboBox.SelectedItem = item;
            }
        }

        /// <summary>
        /// Получение значения наименования свойства из дескриптора свойства
        /// </summary>
        /// <param name="userClass">объект со свойствами</param>
        /// <param name="prop">ссылка на свойство</param>
        /// <returns>Текст из дескриптора</returns>
        public static string GetPropertyCaption(object userClass, PropertyInfo prop)
        {
            // получаем ссылку на коллекцию атрибутов свойства
            var attributes = TypeDescriptor.GetProperties(userClass)[prop.Name].Attributes;
            // получаем из коллекции атрибутов ссылку на атрибут дескриптора свойства
            var mAttribute = (DescriptionAttribute)attributes[typeof(DescriptionAttribute)];
            // по атрибуту "Description" получаем наименование поля редактирования
            return mAttribute.Description;
        }

        private bool CheckNotEmptyAttribute(object userClass, PropertyInfo prop)
        {
            // получаем ссылку на коллекцию атрибутов свойства
            var attributes = TypeDescriptor.GetProperties(userClass)[prop.Name].Attributes;
            // получаем из коллекции атрибутов ссылку на атрибут дескриптора свойства
            var mAttribute = (DataNotEmptyAttribute)attributes[typeof(DataNotEmptyAttribute)];
            // атрибут существует, значит данные должны быть не пустыми
            return mAttribute != null;
        }

        private bool CheckTableFilterableAttribute(object userClass, PropertyInfo prop)
        {
            // получаем ссылку на коллекцию атрибутов свойства
            var attributes = TypeDescriptor.GetProperties(userClass)[prop.Name].Attributes;
            // получаем из коллекции атрибутов ссылку на атрибут дескриптора свойства
            var mAttribute = (TableFilterableAttribute)attributes[typeof(TableFilterableAttribute)];
            // атрибут существует, значит данные должны быть не пустыми
            return mAttribute != null;
        }

        /// <summary>
        /// Установка режима защищенного ввода информации в текстовом поле
        /// </summary>
        /// <param name="userClass">объект со свойствами</param>
        /// <param name="prop">ссылка на свойство</param>
        /// <param name="textBox">ссылка на компонент TextBox</param>
        /// <returns>Признак защищённого ввода текста</returns>
        public static bool SetPasswordMode(object userClass, PropertyInfo prop, TextBox textBox)
        {
            // получаем ссылку на коллекцию атрибутов свойства
            var attributes = TypeDescriptor.GetProperties(userClass)[prop.Name].Attributes;
            // получаем из коллекции атрибутов ссылку на атрибут дескриптора свойства
            var mAttribute = (DataPasswordAttribute)attributes[typeof(DataPasswordAttribute)];
            if (mAttribute == null) return false;
            textBox.Text = string.Empty;
            var ch = mAttribute.PasswordChar;
            if (ch == '\0')
                textBox.UseSystemPasswordChar = true;
            else
                textBox.PasswordChar = ch;
            return true;
        }

        /// <summary>
        /// Проверка признака защищённого ввода текста в атрибуте свойства
        /// </summary>
        /// <param name="userClass">объект со свойствами</param>
        /// <param name="prop">ссылка на свойство</param>
        /// <returns>Признак защищённого ввода текста</returns>
        public static bool CheckPasswordMode(object userClass, PropertyInfo prop)
        {
            // получаем ссылку на коллекцию атрибутов свойства
            var attributes = TypeDescriptor.GetProperties(userClass)[prop.Name].Attributes;
            // получаем из коллекции атрибутов ссылку на атрибут дескриптора свойства
            var mAttribute = (DataPasswordAttribute)attributes[typeof(DataPasswordAttribute)];
            return mAttribute != null;
        }

        /// <summary>
        /// Проверка признака показа столбца в таблице в атрибуте свойства
        /// </summary>
        /// <param name="userClass">объект со свойствами</param>
        /// <param name="prop">ссылка на свойство</param>
        /// <returns>Признак защищённого ввода текста</returns>
        public static bool CheckTableBrowsabeMode(object userClass, PropertyInfo prop)
        {
            // получаем ссылку на коллекцию атрибутов свойства
            var attributes = TypeDescriptor.GetProperties(userClass)[prop.Name].Attributes;
            // получаем из коллекции атрибутов ссылку на атрибут дескриптора свойства
            var mAttribute = (TableBrowsableAttribute)attributes[typeof(TableBrowsableAttribute)];
            return mAttribute != null && mAttribute.Browsable || mAttribute == null;
        }

        /// <summary>
        /// Установка границ числового значения
        /// </summary>
        /// <param name="userClass">объект со свойствами</param>
        /// <param name="prop">ссылка на свойство</param>
        /// <param name="numericUpDown">ссылка на компонент NumericUpDown</param>
        private void SetRange(object userClass, PropertyInfo prop, NumericUpDown numericUpDown)
        {
            // получаем ссылку на коллекцию атрибутов свойства
            var attributes = TypeDescriptor.GetProperties(userClass)[prop.Name].Attributes;
            // получаем из коллекции атрибутов ссылку на атрибут границ значения свойства
            var mAttribute = (DataRangeAttribute)attributes[typeof(DataRangeAttribute)];
            if (mAttribute == null) return;
            var low = mAttribute.Low;
            var high = mAttribute.High;
            if (low > high) return;
            numericUpDown.Minimum = (decimal)low;
            numericUpDown.Maximum = (decimal)high;
            numericUpDown.Value = (decimal)low;
        }

        /// <summary>
        /// Установка длины разрешенного текста
        /// </summary>
        /// <param name="userClass">объект со свойствами</param>
        /// <param name="prop">ссылка на свойство</param>
        /// <param name="textBox">ссылка на компонент TextBox</param>
        private void SetRange(object userClass, PropertyInfo prop, TextBox textBox)
        {
            // получаем ссылку на коллекцию атрибутов свойства
            var attributes = TypeDescriptor.GetProperties(userClass)[prop.Name].Attributes;
            // получаем из коллекции атрибутов ссылку на атрибут границ значения свойства
            var mAttribute = (DataRangeAttribute)attributes[typeof(DataRangeAttribute)];
            if (mAttribute == null) return;
            var high = mAttribute.High;
            if (high < 1 || high > 32767) return;
            textBox.MaxLength = Convert.ToInt32(high);
        }

        /// <summary>
        /// Установка размера текстового редактора
        /// </summary>
        /// <param name="userClass">объект со свойствами</param>
        /// <param name="prop">ссылка на свойство</param>
        /// <param name="textBox">ссылка на компонент TextBox</param>
        private void SetSize(object userClass, PropertyInfo prop, TextBox textBox)
        {
            // получаем ссылку на коллекцию атрибутов свойства
            var attributes = TypeDescriptor.GetProperties(userClass)[prop.Name].Attributes;
            // получаем из коллекции атрибутов ссылку на атрибут границ значения свойства
            var mAttribute = (TextSizeAttribute)attributes[typeof(TextSizeAttribute)];
            if (mAttribute == null) return;
            var width = mAttribute.Width;
            var height = mAttribute.Height;
            if (mAttribute.Multiline)
            {
                textBox.Multiline = true;
                textBox.WordWrap = false;
                textBox.ScrollBars = ScrollBars.Both;
            }
            if (width > 0) textBox.Width = width;
            if (height > 0) textBox.Height = height;
        }

        /// <summary>
        /// Получение значения наименования класса из дескриптора класса
        /// </summary>
        /// <param name="userClass">объект со свойствами</param>
        /// <returns>Текст из дескриптора</returns>
        public static string GetClassCaption(object userClass)
        {
            var attribute = userClass.GetType().GetCustomAttribute(typeof(DescriptionAttribute), false) as DescriptionAttribute;
            return attribute != null ? attribute.Description : string.Empty;
        }

        /// <summary>
        /// Обработчик значения из булева редактора
        /// </summary>
        /// <param name="userClass">объект со свойствами</param>
        /// <param name="prop">ссылка на свойство</param>
        /// <param name="checkBox">ссылка на компонент CheckBox</param>
        /// <returns></returns>
        private Action BooleanValue(object userClass, PropertyInfo prop, CheckBox checkBox)
        {
            return () =>
            {
                prop.SetValue(userClass, checkBox.Checked);
            };
        }

        /// <summary>
        /// Обработчик значения из редактора даты
        /// </summary>
        /// <param name="userClass">объект со свойствами</param>
        /// <param name="prop">ссылка на свойство</param>
        /// <param name="dateTimePicker">ссылка на компонент DateTimePicker</param>
        /// <returns>Ссылка на действие для присвоения значения из редактора даты свойству объекта userClass</returns>
        private Action DateTimeValue(object userClass, PropertyInfo prop, DateTimePicker dateTimePicker)
        {
            return () =>
            {
                prop.SetValue(userClass, dateTimePicker.Value);
            };
        }

        /// <summary>
        /// Обработчик значения из селектора значений
        /// </summary>
        /// <param name="userClass">объект со свойствами</param>
        /// <param name="prop">ссылка на свойство</param>
        /// <param name="comboBox">ссылка на компонент ComboBox</param>
        /// <returns>Ссылка на действие для присвоения значения из селектора значений свойству объекта userClass</returns>
        private Action GuidValue(object userClass, PropertyInfo prop, ComboBox comboBox)
        {
            return () =>
            {
                if (comboBox.SelectedItem == null)
                {
                    // получаем наименование свойства из дескриптора
                    var caption = GetPropertyCaption(userClass, prop);
                    throw new Exception($"Свойство \"{caption}\":{Environment.NewLine}Необходимо выбрать значение свойства из списка");
                }
                var value = (Guid)comboBox.SelectedItem.GetType().GetProperty(prop.Name).GetValue(comboBox.SelectedItem);
                prop.SetValue(userClass, value);
            };
        }

        /// <summary>
        /// Обработчик значения из селектора значений
        /// </summary>
        /// <param name="userClass">объект со свойствами</param>
        /// <param name="prop">ссылка на свойство</param>
        /// <param name="comboBox">ссылка на компонент ComboBox</param>
        /// <returns>Ссылка на действие для присвоения значения из селектора значений свойству объекта userClass</returns>
        private Action EnumValue(object userClass, PropertyInfo prop, ComboBox comboBox)
        {
            return () =>
            {
                if (comboBox.SelectedItem == null)
                {
                    // получаем наименование свойства из дескриптора
                    var caption = GetPropertyCaption(userClass, prop);
                    throw new Exception($"Свойство \"{caption}\":{Environment.NewLine}Необходимо выбрать значение свойства из списка");
                }
                var value = comboBox.SelectedItem;
                prop.SetValue(userClass, value);
            };
        }

        /// <summary>
        /// Обработчик значения из числового редактора
        /// </summary>
        /// <param name="userClass">объект со свойствами</param>
        /// <param name="prop">ссылка на свойство</param>
        /// <param name="numericUpDown">ссылка на компонент NumericUpDown</param>
        /// <returns>Ссылка на действие для присвоения значения из числового редактора свойству объекта userClass</returns>
        private Action NumberValue(object userClass, PropertyInfo prop, NumericUpDown numericUpDown)
        {
            return () =>
            {
                switch (prop.PropertyType.ToString())
                {
                    case "System.Int32": // для случая целочисленного свойства
                        prop.SetValue(userClass, (Int32)numericUpDown.Value);
                        break;
                    default: // для всех остальных случаев
                        prop.SetValue(userClass, numericUpDown.Value);
                        break;
                }
            };
        }

        /// <summary>
        /// Обработчик значения из текстового редактора
        /// </summary>
        /// <param name="userClass">объект со свойствами</param>
        /// <param name="prop">ссылка на свойство</param>
        /// <param name="textBox">ссылка на компонент TextBox</param>
        /// <param name="checkEmpty">признак проверки на пустое значение строки</param>
        /// <returns>Ссылка на действие для присвоения значения из текстового редактора свойству объекта userClass</returns>
        private Action TextValue(object userClass, PropertyInfo prop, TextBox textBox)
        {
            return () =>
            {
                var checkNotEmpty = CheckNotEmptyAttribute(userClass, prop);
                if (checkNotEmpty && string.IsNullOrWhiteSpace(textBox.Text))
                {
                    // получаем наименование свойства из дескриптора
                    var caption = GetPropertyCaption(userClass, prop);
                    throw new Exception($"Свойство \"{caption}\":{Environment.NewLine}Ожидалось не пустое значение свойства");
                }
                prop.SetValue(userClass, textBox.Text);
            };
        }

        /// <summary>
        /// Обработчик значения из текстового редактора пароля
        /// </summary>
        /// <param name="userClass">объект со свойствами</param>
        /// <param name="prop">ссылка на свойство</param>
        /// <param name="textBox">ссылка на компонент TextBox</param>
        /// <returns>Ссылка на действие для присвоения значения из текстового редактора свойству объекта userClass</returns>
        private Action PasswordValue(object userClass, PropertyInfo prop, TextBox textBox)
        {
            return () =>
            {
                if (!string.IsNullOrWhiteSpace(textBox.Text))
                    prop.SetValue(userClass, textBox.Text);
            };
        }
    }
}
