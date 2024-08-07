﻿using Chato.Server.Infrastracture;

namespace Chato.Automation.Infrastructure.Instruction;

public class CounterSignal
{
    public class CounterItemSignal
    {
        private readonly SemaphoreSlim _ensureSingleTask;

        private int _thrasholdTasks;
        private int _current;

        public CounterItemSignal(int count)
        {
            _thrasholdTasks = _current = count;
            _ensureSingleTask = new SemaphoreSlim(1);
        }

        public async Task<int> Increment()
        {
            return await Executer(() => ++_current);
        }

        public async Task<int> Decrement()
        {
            return await Executer(() => --_current);
        }

        public async Task Reset()
        {
            await Executer(() => _current = _thrasholdTasks);
        }

        public async Task SetthrasholdAsync(int thrashold)
        {
            await Executer(() => _current = _thrasholdTasks = thrashold);
        }

        public async Task<bool> IsReleased()
        {
            return await Executer(() => _current <= 0);
        }

        public async Task<bool> IsPollingReleased(int timeoutInSecond, int period = 500)
        {
            var isReleased = await Task.Run(async () =>
            {
                var result = false;
                var timeStemp = TimeStamp.Reset();
                do
                {
                    if (await IsReleased())
                    {
                        result = true;
                        break;
                    }

                    await Task.Delay(period);

                } while (timeStemp.IsSecondOver(timeoutInSecond) == false);

                return result;
            });

            return isReleased;
        }

        private async Task<TResult> Executer<TResult>(Func<TResult> callback)
        {
            try
            {
                await _ensureSingleTask.WaitAsync();
                return callback();
            }
            finally
            {
                _ensureSingleTask.Release();
            }
        }
    }

    private CounterItemSignal _counterSignal;

    public CounterSignal(int thrashold)
    {
        _counterSignal = new CounterItemSignal(thrashold);
    }

    public async Task ReleaseAsync()
    {
        await _counterSignal.Decrement();
    }

    public async Task<bool> WaitAsync(int timeoutInSecond)
    {
        var isPassed = await _counterSignal.IsPollingReleased(timeoutInSecond);
        return isPassed;
    }

    public async Task ResetAsync()
    {
        await _counterSignal.Reset();
    }

    public async Task SetThrasholdAsync(int thrashold)
    {
        await _counterSignal.SetthrasholdAsync(thrashold);
    }
}

