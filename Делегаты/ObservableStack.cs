// Вставьте сюда финальное содержимое файла ObservableStack.cs
using System;
using System.Collections.Generic;
using System.Text;

namespace Delegates.Observers
{
    public class StackOperationsLogger
    {
        private readonly Observer observer = new Observer();
        public void SubscribeOn<T>(ObservableStack<T> stack) { stack.Add(observer); }
        public string GetLog() => observer.Log.ToString();
    }

    public class Observer
    {
        public StringBuilder Log = new StringBuilder();
        public void HandleEvent(object eventData) { Log.Append(eventData); }
    }

    public delegate void MyEvent(object obj);
    public class ObservableStack<T>
    {
        public event MyEvent IamObserver;
        public void Add(Observer observer) { IamObserver += observer.HandleEvent; }
        public void Notify(object eventData) { IamObserver?.Invoke(eventData); }
        public void Remove(Observer observer) { IamObserver -= observer.HandleEvent; }
        readonly List<T> data = new List<T>();
        public void Push(T obj)
        {
            data.Add(obj);
            Notify(new StackEventData<T> { IsPushed = true, Value = obj });
        }

        public T Pop()
        {
            if (data.Count == 0) throw new InvalidOperationException();
            var result = data[data.Count - 1];
            Notify(new StackEventData<T> { IsPushed = false, Value = result });
            return result;
        }
    }
}
