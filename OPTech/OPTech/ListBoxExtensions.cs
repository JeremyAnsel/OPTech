using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace OPTech
{
    static class ListBoxExtensions
    {
        public static void CopyItems(this ListBox list, ListBox right, bool useIndex = true)
        {
            right.Items.Clear();

            for (int i = 0; i < list.Items.Count; i++)
            {
                string text = list.GetText(i, useIndex);
                bool isChecked = list.IsChecked(i);

                var checkBox = GetCheckBox(list.Items[i]);

                if (checkBox != null)
                {
                    IDrawableItem tag = checkBox.Item1.Tag as IDrawableItem;

                    if (tag == null)
                    {
                        right.AddCheck(text, isChecked, true, useIndex);
                    }
                    else
                    {
                        right.AddDrawableCheck(text, tag, isChecked);
                    }
                }
                else
                {
                    right.AddText(text, false, useIndex);
                }
            }

            if (list.SelectedItems.Count == 0)
            {
                right.SelectedItem = null;
            }
            else
            {
                var selectedIndexes = new int[list.SelectedItems.Count];

                for (int i = 0; i < list.SelectedItems.Count; i++)
                {
                    var item = list.SelectedItems[i];

                    selectedIndexes[i] = list.Items.IndexOf(item);
                }

                right.SelectedIndex = selectedIndexes.Last();

                foreach (int index in selectedIndexes)
                {
                    right.SelectedItems.Add(right.Items[index]);
                }

                right.ScrollIntoView(right.SelectedItem);
            }
        }

        public static void UpdateSelectedItems(this ListBox list)
        {
            list.AddToSelection(list.Items.IndexOf(list.SelectedItems.Cast<object>().Last()));
        }

        public static void AddToSelection(this ListBox list, string text, bool useIndex = true)
        {
            int index = list.GetTextIndex(text, useIndex);

            if (index == -1)
            {
                return;
            }

            list.AddToSelection(index);
        }

        public static void AddToSelection(this ListBox list, int index)
        {
            //if (list.SelectionMode == SelectionMode.Single)
            //{
            //    list.SelectedIndex = index;
            //}
            //else
            //{

            int count = list.SelectedItems.Count;

            object[] selectedItems = null;

            if (count > 0)
            {
                selectedItems = new object[count];
                list.SelectedItems.CopyTo(selectedItems, 0);
            }

            list.SelectedIndex = index;

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    list.SelectedItems.Add(selectedItems[i]);
                }
            }

            //}

            //list.ScrollIntoView(list.SelectedItem);
        }

        public static bool IsSelected(this ListBox list, string text, bool useIndex = true)
        {
            int index = list.GetTextIndex(text, useIndex);

            if (index == -1)
            {
                return false;
            }

            return list.IsSelected(index);
        }

        public static bool IsSelected(this ListBox list, int index)
        {
            if (index == -1)
            {
                return false;
            }

            if (list.SelectedIndex == -1)
            {
                return false;
            }

            if (index == list.SelectedIndex)
            {
                return true;
            }

            return list.SelectedItems.Contains(list.Items[index]);
        }

        public static void SetSelected(this ListBox list, string text, bool selected, bool useIndex = true)
        {
            int index = list.GetTextIndex(text, useIndex);

            if (index == -1)
            {
                return;
            }

            list.SetSelected(index, selected);
        }

        public static void SetSelected(this ListBox list, int index, bool selected, bool scrollIntoView = true)
        {
            if (index == -1)
            {
                return;
            }

            var item = list.Items[index];

            if (selected)
            {
                if (!list.SelectedItems.Contains(item))
                {
                    list.SelectedItems.Add(item);
                }
            }
            else
            {
                if (list.SelectedItems.Contains(item))
                {
                    list.SelectedItems.Remove(item);
                }
            }

            if (scrollIntoView && selected)
            {
                list.ScrollIntoView(item);
            }
        }

        public static int GetTextIndex(this ListBox list, string text, bool useIndex = true)
        {
            if (string.IsNullOrEmpty(text))
            {
                return -1;
            }

            for (int i = 0; i < list.Items.Count; i++)
            {
                string currentText = list.GetText(i, useIndex);

                if (string.Equals(currentText, text))
                {
                    return i;
                }
            }

            return -1;
        }

        public static string GetText(this ListBox list, int index, bool useIndex = true)
        {
            if (index == -1)
            {
                return null;
            }

            var item = list.Items[index];

            if (item == null)
            {
                return null;
            }

            var text = item as string;

            if (text == null)
            {
                var textBlock = item as ListBoxItem;
                if (textBlock != null)
                {
                    text = (string)textBlock.Content;
                }
            }

            if (text == null)
            {
                var checkBox = GetCheckBox(item);

                if (checkBox != null)
                {
                    text = checkBox.Item2;
                }
            }

            if (text == null)
            {
                throw new NotSupportedException();
            }

            if (list.ItemsSource != null)
            {
                useIndex = false;
            }

            return useIndex ? RemoveTextLineNumber(text) : text;
        }

        public static string[] GetAllText(this ListBox list, bool useIndex = true)
        {
            var array = new string[list.Items.Count];

            for (int i = 0; i < list.Items.Count; i++)
            {
                array[i] = list.GetText(i, useIndex);
            }

            return array;
        }

        public static Tuple<string, bool>[] GetAllTextWithSelected(this ListBox list, bool useIndex = true)
        {
            var array = new Tuple<string, bool>[list.Items.Count];

            for (int i = 0; i < list.Items.Count; i++)
            {
                array[i] = Tuple.Create(list.GetText(i, useIndex), list.IsSelected(i));
            }

            return array;
        }

        public static string GetSelectedText(this ListBox list, bool useIndex = true)
        {
            if (list.SelectedIndex == -1)
            {
                return null;
            }

            return list.GetText(list.SelectedIndex, useIndex);
        }

        public static void AddText(this ListBox list, string newItem, bool selected = false, bool useIndex = true)
        {
            var textBlock = list.CreateTextItem(list.Items.Count, newItem, useIndex);
            list.Items.Add(textBlock);

            if (selected)
            {
                if (list.SelectionMode == SelectionMode.Single)
                {
                    list.SelectedItem = textBlock;
                }
                else
                {
                    list.SelectedItems.Add(textBlock);
                }
            }
        }

        public static void AddAllText(this ListBox list, string[] array, bool useIndex = true)
        {
            for (int index = 0; index < array.Length; index++)
            {
                var item = array[index];
                list.AddText(item, useIndex);
            }
        }

        public static void AddAllTextWithSelected(this ListBox list, Tuple<string, bool>[] array, bool useIndex = true)
        {
            for (int index = 0; index < array.Length; index++)
            {
                var item = array[index];
                list.AddText(item.Item1, item.Item2, useIndex);
            }
        }

        public static void SetText(this ListBox list, string text, string newItem, bool useIndex = true)
        {
            int index = list.GetTextIndex(text, useIndex);

            if (index == -1)
            {
                return;
            }

            list.SetText(index, newItem, useIndex);
        }

        public static void SetText(this ListBox list, int index, string newItem, bool useIndex = true)
        {
            if (index == -1)
            {
                return;
            }

            var checkBox = GetCheckBox(list.Items[index]);

            if (checkBox == null)
            {
                var newTextBlock = list.CreateTextItem(index, newItem, useIndex);
                list.Items[index] = newTextBlock;
            }
            else
            {
                bool isChecked = list.IsChecked(index);
                IDrawableItem drawableItem = checkBox.Item1.Tag as IDrawableItem;

                if (drawableItem == null)
                {
                    var newCheckBox = list.CreateCheckItem(index, newItem, isChecked, true, useIndex);
                    list.Items[index] = newCheckBox;
                }
                else
                {
                    var newCheckBox = list.CreateDrawableCheckItem(index, newItem, drawableItem, isChecked);
                    list.Items[index] = newCheckBox;
                }
            }
        }

        public static void SetAllText(this ListBox list, string[] array, bool useIndex = true)
        {
            list.Items.Clear();

            for (int index = 0; index < array.Length; index++)
            {
                var item = array[index];
                list.AddText(item, useIndex);
            }

            list.ScrollIntoView(list.SelectedItem);
        }

        public static void SetAllTextWithSelected(this ListBox list, Tuple<string, bool>[] array, bool useIndex = true)
        {
            list.Items.Clear();

            for (int index = 0; index < array.Length; index++)
            {
                var item = array[index];
                list.AddText(item.Item1, item.Item2, useIndex);
            }

            list.ScrollIntoView(list.SelectedItem);
        }

        public static void UpdateTextLineNumbers(this ListBox list, bool useIndex = true)
        {
            for (int index = 0; index < list.Items.Count; index++)
            {
                string text = list.GetText(index, useIndex);
                bool selected = list.IsSelected(index);
                list.SetText(index, text, useIndex);
                list.SetSelected(index, selected);
            }
        }

        public static bool IsChecked(this ListBox list, int index)
        {
            if (index == -1)
            {
                return false;
            }

            var checkBox = GetCheckBox(list.Items[index]);

            if (checkBox == null)
            {
                return false;
            }

            return checkBox.Item1.IsChecked.Value;
        }

        public static Tuple<string, bool>[] GetAllCheck(this ListBox list, bool useIndex = true)
        {
            var array = new Tuple<string, bool>[list.Items.Count];

            for (int i = 0; i < list.Items.Count; i++)
            {
                array[i] = Tuple.Create(list.GetText(i, useIndex), list.IsChecked(i));
            }

            return array;
        }

        public static void SetAllCheck(this ListBox list, Tuple<string, bool>[] array, bool isReadOnly = true, bool useIndex = true)
        {
            list.Items.Clear();

            for (int index = 0; index < array.Length; index++)
            {
                var item = array[index];
                list.AddCheck(item.Item1, item.Item2, isReadOnly, useIndex);
            }
        }

        public static void AddCheck(this ListBox list, string newItem, bool isChecked = false, bool isReadOnly = true, bool useIndex = true)
        {
            var checkBox = list.CreateCheckItem(list.Items.Count, newItem, isChecked, isReadOnly, useIndex);

            list.Items.Add(checkBox);
        }

        public static void AddDrawableCheck(this ListBox list, string newItem, IDrawableItem drawableItem, bool isChecked = true)
        {
            var checkBox = list.CreateDrawableCheckItem(list.Items.Count, newItem, drawableItem, isChecked);

            list.Items.Add(checkBox);
        }

        public static void SetCheck(this ListBox list, string text, string newItem, bool useIndex = true)
        {
            int index = list.GetTextIndex(text, useIndex);

            if (index == -1)
            {
                return;
            }

            list.SetCheck(index, newItem, useIndex);
        }

        public static void SetCheck(this ListBox list, int index, string newItem, bool useIndex = true)
        {
            if (index == -1)
            {
                return;
            }

            var checkBox = list.CreateCheckItem(index, newItem, false, true, useIndex);
            list.Items[index] = checkBox;
        }

        public static void SelectCheck(this ListBox list, string text, bool selected)
        {
            int index = list.GetTextIndex(text);

            if (index == -1)
            {
                return;
            }

            list.SelectCheck(index, selected);
        }

        public static void SelectCheck(this ListBox list, int index, bool selected)
        {
            if (index == -1)
            {
                return;
            }

            var checkBox = GetCheckBox(list.Items[index]);

            if (checkBox == null)
            {
                return;
            }

            checkBox.Item1.IsChecked = selected;
        }

        public static void SortText(this ListBox list)
        {
            Tuple<string, bool>[] allText = list.GetAllTextWithSelected();
            var sortedText = allText.OrderBy(t => t.Item1).ToArray();
            list.SetAllTextWithSelected(sortedText);
        }

        public static void SortCheck(this ListBox list)
        {
            Tuple<string, bool>[] all = list.GetAllCheck();
            var sorted = all.OrderBy(t => t.Item1).ToArray();
            list.SetAllCheck(sorted);
        }

        private static string AddTextLineNumber(int index, string item)
        {
            return index.ToString(CultureInfo.InvariantCulture) + " - " + item;
        }

        private static string RemoveTextLineNumber(string text)
        {
            return text.Substring(text.IndexOf('-') + 2);
        }

        private static FrameworkElement CreateTextItem(this ListBox list, int index, string newItem, bool useIndex)
        {
            var textBlock = new ListBoxItem
            {
                Content = useIndex ? AddTextLineNumber(index, newItem) : newItem,
                Foreground = System.Windows.Media.Brushes.Black,
                Background = System.Windows.Media.Brushes.White
            };

            return textBlock;
        }

        private static FrameworkElement CreateCheckItem(this ListBox list, int index, string newItem, bool isChecked, bool isReadOnly, bool useIndex)
        {
            var panel = new DockPanel();

            var checkBox = new CheckBox
            {
                IsEnabled = !isReadOnly,
                IsChecked = isChecked,
                Content = " "
            };

            var textBlock = new TextBlock
            {
                Text = useIndex ? AddTextLineNumber(index, newItem) : newItem
            };

            checkBox.SetValue(DockPanel.DockProperty, Dock.Left);
            panel.Children.Add(checkBox);
            panel.Children.Add(textBlock);

            return panel;
        }

        private static FrameworkElement CreateDrawableCheckItem(this ListBox list, int index, string newItem, IDrawableItem drawableItem, bool isChecked)
        {
            var item = list.CreateCheckItem(index, newItem, isChecked, false, true);
            var checkBox = GetCheckBox(item).Item1;

            checkBox.Tag = drawableItem;
            checkBox.Checked += CheckBox_Checked;
            checkBox.Unchecked += CheckBox_Checked;

            return item;
        }

        private static void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var drawableItem = checkBox.Tag as IDrawableItem;

            if (drawableItem == null)
            {
                return;
            }

            drawableItem.Drawable = checkBox.IsChecked.Value;
        }

        private static Tuple<CheckBox, string> GetCheckBox(object item)
        {
            var panel = item as DockPanel;

            if (panel == null)
            {
                return null;
            }

            if (panel.Children.Count < 2)
            {
                return null;
            }

            var checkBox = panel.Children[0] as CheckBox;
            string text = ((TextBlock)panel.Children[1]).Text;
            return Tuple.Create(checkBox, text);
        }
    }
}
