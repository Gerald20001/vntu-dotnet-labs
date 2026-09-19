using System;

namespace GenericsDelegatesEvents.Events;

/// <summary>
/// Аргументи події переповнення стека.
/// Передає інформацію про елемент, який не вдалося додати через вичерпання місткості.
/// </summary>
/// <typeparam name="T">Тип даних елемента</typeparam>
public class OverflowEventArgs<T> : EventArgs
{
    public T RejectedItem { get; }
    public int Capacity { get; }
    public DateTime AttemptedAt { get; }

    public OverflowEventArgs(T rejectedItem, int capacity)
    {
        RejectedItem = rejectedItem;
        Capacity = capacity;
        AttemptedAt = DateTime.Now;
    }
}
