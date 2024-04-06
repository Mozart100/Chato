namespace Chato.Server.Infrastracture
{
    public class SemephoreAutoSlim<T>
    {
        private readonly T _item;
        private readonly SemaphoreSlim _semaphore;

        public SemephoreAutoSlim(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            _item = item;
            _semaphore = new SemaphoreSlim(1);
        }



        public async Task DoAsync(Func<T, Task> callback)
        {
            await _semaphore.WaitAsync();

            try
            {
                await callback(_item);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
