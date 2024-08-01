using System;
using System.Collections.Generic;

namespace Aequus.Common.Utilities.Helpers;
internal class LinkedListTools {
    public static void ForEach<T>(LinkedList<T> linkedList, Action<T> action) {
        LinkedListNode<T> node = linkedList.First!;

        do {
            action(node.Value);
        }
        while (Step(ref node));
    }

    public static void Where<T>(LinkedList<T> linkedList, Predicate<T> predicate) {
        LinkedListNode<T> node = linkedList.First!;

        do {
            if (predicate(node.Value)) {
                linkedList.Remove(node);
            }
        }
        while (Step(ref node));
    }

    public static bool Step<T>(ref LinkedListNode<T> node) {
        LinkedListNode<T>? next = node.Next;
        node = next ?? node;
        return next != null;
    }
}
