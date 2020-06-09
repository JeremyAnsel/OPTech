using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace OPTech
{
    static class ListBoxExtensions
    {
        public static void CopyItems(this ListBox list, ListBox right)
        {
            right.Items.Clear();

            foreach (string item in list.GetAllText())
            {
                right.AddText(item);
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

        public static void SetSelected(this ListBox list, int index, bool selected)
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

            if (selected)
            {
                list.ScrollIntoView(item);
            }
        }

        public static string GetText(this ListBox list, int index)
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

            if (text != null)
            {
                return text;
            }

            var textBlock = item as ListBoxItem;
            if (textBlock != null)
            {
                return (string)textBlock.Content;
            }

            var checkBox = item as CheckBox;
            if (checkBox != null)
            {
                return ((TextBlock)checkBox.Content).Text;
            }

            throw new NotSupportedException();
        }

        public static string[] GetAllText(this ListBox list)
        {
            var array = new string[list.Items.Count];

            for (int i = 0; i < list.Items.Count; i++)
            {
                array[i] = list.GetText(i);
            }

            return array;
        }

        public static Tuple<string, bool>[] GetAllTextWithSelected(this ListBox list)
        {
            var array = new Tuple<string, bool>[list.Items.Count];

            for (int i = 0; i < list.Items.Count; i++)
            {
                array[i] = Tuple.Create(list.GetText(i), list.IsSelected(i));
            }

            return array;
        }

        public static string GetSelectedText(this ListBox list)
        {
            if (list.SelectedIndex == -1)
            {
                return null;
            }

            return list.GetText(list.SelectedIndex);
        }

        public static void AddText(this ListBox list, string newItem, bool selected = false)
        {
            var textBlock = new ListBoxItem
            {
                Content = newItem,
                Foreground = System.Windows.Media.Brushes.Black,
                Background = System.Windows.Media.Brushes.White
            };

            list.Items.Add(textBlock);

            if (selected)
            {
                list.SelectedItems.Add(textBlock);
            }
        }

        public static void SetText(this ListBox list, int index, string newItem)
        {
            if (index == -1)
            {
                return;
            }

            var textBlock = new ListBoxItem
            {
                Content = newItem,
                Foreground = System.Windows.Media.Brushes.Black,
                Background = System.Windows.Media.Brushes.White
            };

            list.Items[index] = textBlock;
        }

        public static void AddCheck(this ListBox list, string newItem)
        {
            var checkBox = new CheckBox
            {
                Content = new TextBlock
                {
                    Text = newItem
                },
                IsEnabled = false,
                Foreground = System.Windows.Media.Brushes.Black,
                Background = System.Windows.Media.Brushes.White
            };

            list.Items.Add(checkBox);
        }

        public static void SetCheck(this ListBox list, int index, string newItem)
        {
            if (index == -1)
            {
                return;
            }

            var checkBox = new CheckBox
            {
                Content = new TextBlock
                {
                    Text = newItem
                },
                IsEnabled = false,
                Foreground = System.Windows.Media.Brushes.Black,
                Background = System.Windows.Media.Brushes.White
            };

            list.Items[index] = checkBox;
        }

        public static void SelectCheck(this ListBox list, int index, bool selected)
        {
            if (index == -1)
            {
                return;
            }

            var checkBox = list.Items[index] as CheckBox;

            if (checkBox == null)
            {
                return;
            }

            checkBox.IsChecked = selected;
        }
    }
}
