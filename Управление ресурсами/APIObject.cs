// Вставьте сюда финальное содержимое файла APIObject.cs
using System;

namespace Memory.API
{
    public class APIObject : IDisposable
    {
        #region IDisposable Support
        private readonly int pupaAndLupa;
        private bool disposedValue = false; // Для определения избыточных вызовов

        public APIObject(int number)
        {
            this.pupaAndLupa = number;
            MagicAPI.Allocate(number);
        }

        public void Dispose()
        {
            if (!disposedValue)
            {
                MagicAPI.Free(pupaAndLupa);
                disposedValue = true;
            }
        }

        ~APIObject() { if (!disposedValue) MagicAPI.Free(pupaAndLupa); }
        #endregion
    }
}


