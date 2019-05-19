using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ViewGenerator
{
    public static class GridPanelBuilder
    {
        public static event Action<string, string> Error = delegate { };

        private static void OnError(string message, string caption)
        {
            Error(message, caption);
        }

        /// <summary>
        /// Поиск строки с текстом
        /// </summary>
        /// <param name="userControl"></param>
        /// <param name="text"></param>
        public static void FindText(GridPanel userControl, string text)
        {
            var controls = userControl.Controls.Find("listView", true) ;
            if (controls.Length == 0) return;
            var lv = (ListView)controls[0];
            if (string.IsNullOrWhiteSpace(text))
            {
                lv.SelectedIndices.Clear();
                return;
            }
            var lvi = lv.FindItemWithText(text);
            if (lvi != null) // если найдена, то делаем ее текущей
            {
                lvi.Selected = true;
                lvi.EnsureVisible();
            }
            else
                lv.SelectedIndices.Clear();
        }

        /// <summary>
        /// Построение панели с таблицей
        /// </summary>
        /// <param name="userModel">Корневой класс модели</param>
        /// <param name="userClass">Класс сущности</param>
        /// <param name="userCollection">Коллекция сущностей</param>
        /// <param name="userFilteredCollection">Коллекция сущностей фильтрованная</param>
        /// <param name="propertyName">Имя свойства для фильтра</param>
        /// <param name="propertyValue">Значение свойства для фильтра</param>
        /// <returns></returns>
        public static GridPanel BuildPropertyPanel(object userModel, object userClass, object userCollection,
            object userFilteredCollection = null, string propertyName = null, Guid propertyValue = new Guid())
        {
            // заготовка для таблицы записей
            var userControl = new GridPanel
            {
                Dock = DockStyle.Fill
            };
            // создаём сетку для компоновки
            var grid = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            grid.ColumnStyles.Add(new ColumnStyle());
            grid.RowStyles.Add(new RowStyle());
            grid.RowStyles.Add(new RowStyle() { SizeType = SizeType.Percent, Height = 100 });
            // кнопка "Добавить"
            var btnAdd = new Button
            {
                Text = "Добавить",
                Anchor = AnchorStyles.Left,
                AutoSize = true
            };
            // заготовка для таблицы
            var listView = new ListView
            {
                Name = "listView",
                Dock = DockStyle.Fill,
                MultiSelect = false,
                FullRowSelect = true,
                HideSelection = false,
                ShowItemToolTips = true,
                VirtualMode = true,
                GridLines = true,
                View = View.Details
            };
            var btnEdit = new Button
            {
                Text = "Изменить",
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Enabled = false
            };
            var btnDelete = new Button
            {
                Text = "Удалить",
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Enabled = false
            };
            // обработчик действия при нажатии кнопки "Добавить"
            btnAdd.Click += (o, e) =>
            {
                // создаём пустой объект требуемого типа
                var item = Activator.CreateInstance(userClass.GetType());

                // задаём значение фильтрованного свойства из propertyName и propertyValue
                if (userFilteredCollection != null && !string.IsNullOrWhiteSpace(propertyName) && propertyValue != Guid.Empty)
                {
                    PropertyInfo piInstance = item.GetType().GetProperty(propertyName);
                    piInstance.SetValue(item, propertyValue);
                }

                // вызываем диалог для заполнения свойств объекта
                var frm = PropertyPanelBuilder.ShowPropertyFormDialog(userModel, item, userControl.Font.Name, userControl.Font.Size);
                // если не была нажата клавиша "Ввод", выходим
                if (frm.DialogResult != DialogResult.OK) return;

                // формируем данные для вызова метода Add коллекции объектов:
                // заказываем типы параметров для вызова метода
                Type[] parameterTypes = { item.GetType() };
                // создаём ссылку на метод Add, с формальным списком параметров 
                MethodInfo method = userCollection.GetType().GetMethod("Add", parameterTypes);
                // формируем массив значений параметров для передачи при вызове метода
                object[] arguments = { item };
                try
                {
                    // вызываем метод на коллекции объектов с аргументами
                    method.Invoke(userCollection, arguments);
                    if (userFilteredCollection != null)
                    {
                        method = userFilteredCollection.GetType().GetMethod("Add", parameterTypes);
                        method.Invoke(userFilteredCollection, arguments);
                    }
                }
                catch (Exception ex)
                {
                    OnError(ex.InnerException != null 
                        ? ex.InnerException.Message 
                        : ex.Message, "Добавление записи");
                }
                listView.VirtualListSize = 0; // сбросим виртуальный размер
                listView.VirtualListSize = ((IEnumerable<object>)(userFilteredCollection != null ? userFilteredCollection : userCollection)).Count(); // установим размер по размеру коллекции
                listView.Invalidate(); // просим обновить вид
                btnEdit.Enabled = btnDelete.Enabled = false;
                userControl.OnGridSelectedChanged(item);
                // ищем новую строку в списке
                var lvi = listView.FindItemWithText(item.ToString());
                if (lvi != null) // если найдена, то делаем ее текущей
                {
                    lvi.Selected = true;
                    lvi.EnsureVisible();
                }
            };
            btnEdit.Click += (o, e) =>
            {
                if (listView.SelectedIndices.Count == 0) return;
                // получаем ссылку на коллекцию
                var collection = (IEnumerable<object>)(userFilteredCollection != null ? userFilteredCollection : userCollection);
                // получаем ссылку на редактируемый элемент
                var oldItem = collection.ElementAt(listView.SelectedIndices[0]);
                // сохраняем старую версию объекта
                var item = oldItem.DeepClone();
                // вызываем диалог для заполнения свойств объекта
                var frm = PropertyPanelBuilder.ShowPropertyFormDialog(userModel, item, userControl.Font.Name, userControl.Font.Size);
                // если не была нажата клавиша "Ввод", выходим
                if (frm.DialogResult != DialogResult.OK) return;

                // формируем данные для вызова метода Add коллекции объектов:
                // заказываем типы параметров для вызова метода
                Type[] parameterTypes = { oldItem.GetType(), item.GetType() };
                // создаём ссылку на метод Add, с формальным списком параметров 
                MethodInfo method = userCollection.GetType().GetMethod("ChangeTo", parameterTypes);
                // формируем массив значений параметров для передачи при вызове метода
                object[] arguments = { oldItem, item };
                try
                {
                    // вызываем метод на коллекции объектов с аргументами
                    method.Invoke(userCollection, arguments);
                    if (userFilteredCollection != null)
                    {
                        method = userFilteredCollection.GetType().GetMethod("ChangeTo", parameterTypes);
                        method.Invoke(userFilteredCollection, arguments);
                    }
                }
                catch (Exception ex)
                {
                    OnError(ex.InnerException != null
                        ? ex.InnerException.Message
                        : ex.Message, "Изменение записи");
                }
                listView.VirtualListSize = 0; // сбросим виртуальный размер
                listView.VirtualListSize = ((IEnumerable<object>)(userFilteredCollection != null ? userFilteredCollection : userCollection)).Count(); // установим размер по размеру коллекции
                listView.Invalidate(); // просим обновить вид
                btnEdit.Enabled = btnDelete.Enabled = false;
                userControl.OnGridSelectedChanged(item);
                // ищем новую строку в списке
                var lvi = listView.FindItemWithText(item.ToString());
                if (lvi != null) // если найдена, то делаем ее текущей
                {
                    lvi.Selected = true;
                    lvi.EnsureVisible();
                }
            };
            btnDelete.Click += (o, e) =>
            {
                if (listView.SelectedIndices.Count == 0) return;
                if (MessageBox.Show("Удалить текущую строку?", "Удаление строки",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                                    MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
                // получаем ссылку на коллекцию
                var collection = (IEnumerable<object>)(userFilteredCollection != null ? userFilteredCollection : userCollection);
                // получаем ссылку на редактируемый элемент
                var item = collection.ElementAt(listView.SelectedIndices[0]);
                // формируем данные для вызова метода Add коллекции объектов:
                // заказываем типы параметров для вызова метода
                Type[] parameterTypes = { item.GetType() };
                // создаём ссылку на метод Remove, с формальным списком параметров 
                MethodInfo method = userCollection.GetType().GetMethod("Remove", parameterTypes);
                // формируем массив значений параметров для передачи при вызове метода
                object[] arguments = { item };
                try
                {
                    // вызываем метод на коллекции объектов с аргументами
                    method.Invoke(userCollection, arguments);
                    if (userFilteredCollection != null)
                    {
                        method = userFilteredCollection.GetType().GetMethod("Remove", parameterTypes);
                        method.Invoke(userFilteredCollection, arguments);
                    }
                }
                catch (Exception ex)
                {
                    OnError(ex.InnerException != null
                        ? ex.InnerException.Message
                        : ex.Message, "Удаление записи");
                }
                listView.VirtualListSize = 0; // сбросим виртуальный размер
                listView.VirtualListSize = ((IEnumerable<object>)(userFilteredCollection != null ? userFilteredCollection : userCollection)).Count(); // установим размер по размеру коллекции
                listView.Invalidate(); // просим обновить вид
                btnEdit.Enabled = btnDelete.Enabled = false;
                userControl.OnGridSelectedChanged(null);
            };
            // создадим панель для размещения кнопок
            var flow = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Left
            };
            // добавим кнопки в панель кнопок
            flow.Controls.Add(btnAdd);
            flow.Controls.Add(btnEdit);
            flow.Controls.Add(btnDelete);
            // добавим панель кнопок в сетку
            grid.Controls.Add(flow, 0, 0);
            var gr = listView.CreateGraphics();
            var colsmax = new List<float>();
            // получаем тип объекта, переданного через параметр
            var type = userClass.GetType();
            MemberInfo[] m = type.GetProperties();  // получаем массив свойств объекта
            var count = 0;
            foreach (var info in m) // для каждого свойства из массива свойств
            {
                // получаем ссылку на свойство по его имени
                var prop = type.GetProperty(info.Name);
                // если столбец не показывается в таблице, пропускаем
                if (!PropertyPanelBuilder.CheckTableBrowsabeMode(userClass, prop)) continue;
                // получаем наименование свойства из дескриптора
                var caption = PropertyPanelBuilder.GetPropertyCaption(userClass, prop);
                var text = string.IsNullOrWhiteSpace(caption) ? prop.Name : caption;
                var width = gr.MeasureString(text, listView.Font).Width + 16;
                var typeName = prop.PropertyType.ToString();
                var textAlign = HorizontalAlignment.Left;
                switch (typeName)
                {
                    case "System.Guid":
                        if (count++ == 0) continue;
                        colsmax.Add(width);
                        break;
                    case "System.String":
                        if (PropertyPanelBuilder.CheckPasswordMode(userClass, prop)) continue;
                        colsmax.Add(width);
                        break;
                    case "System.Int32":    // для целочисленных свойств
                        textAlign = HorizontalAlignment.Right;
                        colsmax.Add(width);
                        break;
                    case "System.Decimal":  // для свойств с ценой
                        textAlign = HorizontalAlignment.Right;
                        colsmax.Add(width);
                        break;
                    case "System.DateTime": // для свойств с датой
                        textAlign = HorizontalAlignment.Center;
                        colsmax.Add(width);
                        break;
                    case "System.Boolean": // для логических свойств
                        textAlign = HorizontalAlignment.Center;
                        colsmax.Add(width);
                        break;
                    default:
                        if (prop.PropertyType.BaseType.FullName == "System.Enum")
                        {
                            textAlign = HorizontalAlignment.Center;
                            colsmax.Add(width);
                            break;
                        }
                        continue;
                }
                listView.Columns.Add(new ColumnHeader
                {
                    Text = text,
                    TextAlign = textAlign,
                    Width = (int)width
                });
            }
            // цепляем обработчик для виртуального режима
            listView.RetrieveVirtualItem += (o, e) =>
            {
                var lv = (ListView)o;
                e.Item = new ListViewItem();
                // получаем ссылку на коллекцию
                var collection = (IEnumerable<object>)(userFilteredCollection != null ? userFilteredCollection : userCollection);
                if (e.ItemIndex >= collection.Count())
                {
                    for (var i = 1; i < lv.Columns.Count; i++)
                        e.Item.SubItems.Add("");
                    return;
                }
                // получаем ссылку на рисуемый элемент
                var item = collection.ElementAt(e.ItemIndex);
                // получаем его тип
                type = item.GetType();
                m = type.GetProperties();  // получаем массив свойств объекта
                count = 0; // счетчик столбцов
                for (var i = 0; i < m.Length; i++)      // для каждого свойства из массива свойств
                {
                    // получаем ссылку на свойство по его имени
                    var prop = type.GetProperty(m[i].Name);
                    // если столбец не показывается в таблице, пропускаем
                    if (!PropertyPanelBuilder.CheckTableBrowsabeMode(userClass, prop)) continue;
                    // если свойство первое и его тип Guid (ключевой столбец) - пропускаем
                    if (i == 0 && prop.PropertyType == typeof(Guid)) continue;
                    string value; // здесь будет значение
                    if (prop.PropertyType == typeof(DateTime))
                        value = ((DateTime)prop.GetValue(item)).ToShortDateString();
                    else if (prop.PropertyType == typeof(bool))
                        value = ((bool)prop.GetValue(item)) ? "Да" : "Нет";
                    else if (prop.PropertyType == typeof(decimal))
                        value = ((decimal)prop.GetValue(item)).ToString("0.00");
                    else if (prop.PropertyType == typeof(int))
                        value = ((int)prop.GetValue(item)).ToString("0");
                    else if (prop.PropertyType == typeof(string))
                    {
                        if (PropertyPanelBuilder.CheckPasswordMode(userClass, prop)) continue;
                        value = prop.GetValue(item)?.ToString();
                    }
                    else if (prop.PropertyType == typeof(Guid))
                        value = GetLookupName(prop, item, userModel);
                    else if (prop.PropertyType.BaseType == typeof(Enum))
                        value = GetEnumName(prop, item, userModel);
                    else
                        continue;
                    var width = gr.MeasureString(value, lv.Font).Width + 16;
                    // делаем столбец шире, если нужно
                    if (count < lv.Columns.Count && lv.Columns[count].Width < (int)width)
                        lv.Columns[count].Width = (int)width;
                    // если не было столбцов, пишем в первый
                    if (count++ == 0)
                        e.Item.Text = value;
                    else // иначе добавляем
                        e.Item.SubItems.Add(value);
                }
            };
            // цепляем обработчик для виртуального поиска
            listView.SearchForVirtualItem += (o, e) => 
            {
                // e.Text содержит строковое представление объекта (перегрузкой метода ToString())
                // получаем ссылку на коллекцию
                var collection = (IEnumerable<object>)(userFilteredCollection != null ? userFilteredCollection : userCollection);
                var n = 0;
                foreach (var item in collection)
                {
                    if (item.ToString().StartsWith(e.Text, true, 
                        System.Globalization.CultureInfo.GetCultureInfo("Ru-ru")))
                    {
                        e.Index = n;
                        break;
                    }
                    n++;
                }
            };
            // указываем размер коллекции
            listView.VirtualListSize = ((IEnumerable<object>)(userFilteredCollection != null ? userFilteredCollection : userCollection)).Count();
            // разрешаем кнопки редактирования
            listView.SelectedIndexChanged += (o, e) => 
            {
                var lv = (ListView)o;
                btnEdit.Enabled = btnDelete.Enabled = lv.SelectedIndices.Count > 0;
                if (lv.SelectedIndices.Count > 0)
                {
                    var collection = (IEnumerable<object>)(userFilteredCollection != null ? userFilteredCollection : userCollection);
                    var item = collection.ElementAt(lv.SelectedIndices[0]);
                    userControl.OnGridSelectedChanged(item);
                }
                else
                    userControl.OnGridSelectedChanged(null);
            };

            // добавим детальный список в сетку
            grid.Controls.Add(listView, 0, 1);
            // добавим сетку на панель свойств
            userControl.Controls.Add(grid);
            // возвращаем сформированный контрол
            return userControl;
        }

        /// <summary>
        /// Извлечение текстового представления связанного перечисляемого поля
        /// </summary>
        /// <param name="prop">Enum - свойство для подключения связанного справочника</param>
        /// <param name="item">текущий объект строки</param>
        /// <param name="userModel">объект корневого объекта модели</param>
        /// <returns>Найденное строковое представление</returns>
        public static string GetEnumName(PropertyInfo prop, object item, object userModel)
        {
            if (prop.PropertyType.BaseType != typeof(Enum)) return "(error: enum prop expecteded)";
            var value = (Enum)prop.GetValue(item);
            return value.ToString();
        }

        /// <summary>
        /// Извлечение текстового представления связанного ключевого поля
        /// </summary>
        /// <param name="prop">Guid - свойство для подключения связанного справочника</param>
        /// <param name="item">текущий объект строки</param>
        /// <param name="userModel">объект корневого объекта модели</param>
        /// <returns>Найденное строковое представление</returns>
        public static string GetLookupName(PropertyInfo prop, object item, object userModel)
        {
            if (prop.PropertyType != typeof(Guid)) return "(error: guid prop expecteded)";
            var value = (Guid)prop.GetValue(item);
            // получаем ссылку на коллекцию атрибутов свойства
            var attributes = TypeDescriptor.GetProperties(item)[prop.Name].Attributes;
            // получаем из коллекции атрибутов ссылку на атрибут источника данных свойства
            var mAttribute = (DataLookupAttribute)attributes[typeof(DataLookupAttribute)];
            // возвращаем текстовое представления ключа, если атрибут для поиска не определён
            if (mAttribute == null) return value.ToString();
            var valueMember = mAttribute.ValueMember;    // имя ключевого свойства в коллекции
            var lookupMember = mAttribute.LookupMember;  // имя перечисления коллекции объектов ("ключевой" список)
            // ищем соответствие ключу в "ключевом" списке, заданном в атрибуте DataLookupAttribute
            foreach (var lookup in (IEnumerable<object>)userModel.GetType()
                                   .GetProperty(lookupMember).GetValue(userModel))
            {
                // получаем свойство, заданное в аттрибуте DataLookupAttribute, объекта ключевой коллеции
                var lookprop = lookup.GetType().GetProperty(valueMember);
                // работаем только с типом Guid
                if (lookprop.PropertyType != typeof(Guid)) continue;
                // получаем значение ключа
                var itemValue = (Guid)lookprop.GetValue(lookup);
                if (itemValue == value) // если ключи совпадают, то возвращаем заданнае текстовое представление
                    return lookup.ToString();
            }
            // иначе возвращаем строковое представление ключа
            return value.ToString();
        }
    }
}
