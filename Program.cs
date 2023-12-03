using System.Net.WebSockets;
using System.Security.Principal;

namespace MultiThreadingResearch;

class Program
{
    private static readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
    private static CancellationToken _token;
    private static volatile bool _blockTrigger;
    private static System.Timers.Timer _timer;
    private static System.Threading.Timer _threadTimer;
    private static bool _result;
    private static object _lockObj = new object();

    /**
    План:   1. Запустили какую-то команду
            2. Меняем состояние тригеров для луча на false;
            2. Ожидаем завершения обработки либо по таймауту меняем состояние тригеров для луча

    Как будем ожидать таймаута:
        1. запустим поток, который завершится по таймауту.
        2. будем в этом потоке бесконечно проверять сменилось ли состояние обработки данных
        3.  
    */

    static void Main(string[] args)
    {
        ThreadBuilder();
    }

    private static void ThreadBuilder()
    {
        _result = true;
        _token = _tokenSource.Token;
        // MethodWithTimer();
        MethodWithThreadTimer();
        // MethodWithTasks();
        Thread.Sleep(TimeSpan.FromSeconds(1));
        WorkCompleted();
        Console.WriteLine($"Program has finished and result: {_result}");
    }

    private static void MethodWithThreadTimer()
    {
        Console.WriteLine("Thread.Timer method has started");
        System.Threading.TimerCallback tm = new System.Threading.TimerCallback(WorkToDo);
        _threadTimer = new System.Threading.Timer(tm, _token, 5000, Timeout.Infinite);
        Console.WriteLine($"Thread to terminate has completed, _result is equal to {_result}");
    }

    private static void MethodWithThreads()
    {
        Console.WriteLine("Thread method has started");
    }
    
    private static void MethodWithTasks()
    {
        Task TaskWithDelay = Task.Delay(5000).ContinueWith(DoWorkTask);
        Console.WriteLine("Task method has started");
    }

    private static void DoWorkTask(Task t)
    {
        if(_token.IsCancellationRequested)
            return;
        Console.WriteLine("Task in progress");
        _result = false;
        _tokenSource.Cancel();
    }

    private static void MethodWithTimer()
    {
        Console.WriteLine("Thread with timer has started");
        Thread thread = new Thread(ThreadToTerminaterWithClock);
        thread.Start();
        Console.WriteLine($"Program has finished and result: {_result}");
    }

    private static void ThreadToTerminateWithThreadingTimer()
    {
        TimeSpan timeout = TimeSpan.FromSeconds(5);
        while(!_token.IsCancellationRequested)
        {

        }
    }

    private static void ThreadToTerminaterWithClock()
    {
        lock(_lockObj)
        {
            TimeSpan timeout = TimeSpan.FromSeconds(5);
            _timer = new System.Timers.Timer(timeout);
            while(!_token.IsCancellationRequested)
            {
                _timer.Elapsed -= WorkToDo;
                _timer.Elapsed += WorkToDo;
                _timer.AutoReset = false;
                _timer.Enabled = true;
                _timer.Start();
                Thread.Sleep(100);
                Console.WriteLine($"thread to terminate has completed, _result is equal to {_result}");
            }
            _timer.Stop();
            _timer.Dispose();
            Console.WriteLine($"thread to terminate has completed, _result is equal to {_result}");
        }
    }

    private static void WorkToDo(object? sender, System.Timers.ElapsedEventArgs e)
    {
        Console.WriteLine("WorkToDo in progress");
        _tokenSource.Cancel();
        _result = false;
    }
    private static void WorkToDo(object state)
    {
        CancellationToken token = (CancellationToken)state;
        if(((CancellationToken)state).IsCancellationRequested)
        {
            TimerTermination();
            return;
        }
        Console.WriteLine("WorkToDo in progress");
        _result = false;
        _tokenSource.Cancel();
        
    }

    private static void TimerTermination()
    {
        _threadTimer.Change(Timeout.Infinite, Timeout.Infinite);
    } 

    private static void WorkCompleted()
    {
        _result = false;
        Console.WriteLine("Work completed");
        _tokenSource.Cancel();
    }

    private static void TaskBuilder()
    {
        
    }
}
