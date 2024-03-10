using System.Threading.Channels;

namespace QueueDemo;

public interface INameQueueService
{
    void Add(string name);
    string DeQueue();
    bool HasNext();
}

public class NameQueueService : INameQueueService
{
    private List<string> _queue;

    public NameQueueService()
    {
        _queue = new();
    }

    public void Add(string name)
    {
        _queue.Add(name);
    }

    public string DeQueue()
    {
        var result = _queue.First();
        _queue.RemoveAt(0);
        return result;
    }

    public bool HasNext()
    {
        return _queue.Count > 0;
    }
}

public class Queue1HostedService : BackgroundService
{
    private readonly INameQueueService _nameQueueService;

    public Queue1HostedService(INameQueueService nameQueueService)
    {
        _nameQueueService = nameQueueService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000);

            while (_nameQueueService.HasNext())
            {
                var name = _nameQueueService.DeQueue();
                await Task.Delay(500);
                Console.WriteLine($"ExecuteAsync worked for {name}");
            }

            Console.WriteLine($"ExecuteAsync worked for empty result");
        }
    }
}


// --------------------------------------


public interface IBackgroundTaskQueue<T>
{
    Task AddQueue(T item);

    Task<T> DeQueue(CancellationToken cancellationToken);
}

public class NameQueue : IBackgroundTaskQueue<string>
{
    private readonly Channel<string> _queue;

    public NameQueue()
    {
        var options = new BoundedChannelOptions(100);
        options.FullMode = BoundedChannelFullMode.Wait;
        _queue = Channel.CreateBounded<string>(options);
        //_queue = Channel.CreateBounded<string>(new BoundedChannelOptions(100)
        //{
        //    FullMode = BoundedChannelFullMode.Wait,
        //});
    }

    public async Task AddQueue(string item)
    {
        ArgumentException.ThrowIfNullOrEmpty(item);

        await _queue.Writer.WriteAsync(item);
    }

    public async Task<string> DeQueue(CancellationToken cancellationToken)
    {
        var item = await _queue.Reader.ReadAsync(cancellationToken);
        return item;
    }
}

public class Queue2HostedService : BackgroundService
{
    private readonly IBackgroundTaskQueue<string> _queue;

    public Queue2HostedService(IBackgroundTaskQueue<string> queue)
    {
        _queue = queue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var name = await _queue.DeQueue(stoppingToken);

            await Task.Delay(500);

            Console.WriteLine($"ExecuteAsync worked for {name}");
        }
    }
}