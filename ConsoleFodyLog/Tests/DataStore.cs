namespace ConsoleFodyLog
{

    [Log]
    class DataStore<T>
    {
        private T[] _data = new T[10];

        public void AddOrUpdate(int index, T item)
        {
            if (index >= 0 && index < 10)
                _data[index] = item;
        }

        public string AddOrUpdateWithMsg(int index, T item)
        {
            if (index >= 0 && index < 10)
                _data[index] = item;

            return $"{item} is stored en position {index}";
        }

        public T GetData(int index)
        {
            if (index >= 0 && index < 10)
                return _data[index];
            else
                return default(T);
        }
    }

}
