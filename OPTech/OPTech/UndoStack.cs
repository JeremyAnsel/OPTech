using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace OPTech
{
    static class UndoStack
    {
        public const int MaxStackLength = 30;

        public static ObservableCollection<Tuple<string, UndoStackItem>> Stack { get; } = new ObservableCollection<Tuple<string, UndoStackItem>>();

        public static void Push(string label)
        {
            Stack.Insert(0, Tuple.Create(label, UndoStackItem.Capture()));

            while (Stack.Count > MaxStackLength)
            {
                Stack.RemoveAt(Stack.Count - 1);
            }
        }

        public static void Restore(int index)
        {
            if (index == -1)
            {
                return;
            }

            var stack = Stack[index];

            stack.Item2.Restore();

            Stack.RemoveAt(index);
            Stack.Insert(0, Tuple.Create(stack.Item1, stack.Item2.Clone()));
        }
    }
}
