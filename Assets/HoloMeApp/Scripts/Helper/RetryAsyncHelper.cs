using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

public class RetryAsyncHelpe {

    public static async Task RetryOnExceptionAsync(Func<Task> operation, int times = 3) {
        await RetryOnExceptionAsync<Exception>(operation, times);
    }

    public static async Task RetryOnExceptionAsync<TException>(
        Func<Task> operation, int times = 3) where TException : Exception {

        var attempts = -1;
        do {
            try {
                attempts++;
                await operation();
                break;
            } catch (TException ex) {
                if (attempts == (times - 1))
                    throw ex;

                await CreateDelayForException(attempts);
            }
        } while (true);
    }

    private static Task CreateDelayForException(int attempts) {
        var delay = IncreasingDelayInSeconds(attempts);
        return Task.Delay(delay);
    }

    internal static int[] DelayPerAttemptInSeconds = {
        2000,
        5000,
        10000,
        30000,
        60000,
    };

    static int IncreasingDelayInSeconds(int failedAttempts) {
        return failedAttempts > DelayPerAttemptInSeconds.Length ? DelayPerAttemptInSeconds.Last() : DelayPerAttemptInSeconds[failedAttempts];
    }
}
