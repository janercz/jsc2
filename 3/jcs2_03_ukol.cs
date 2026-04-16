using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public enum OrderPriority { Normal, Express }

public sealed class Order
{
    public long Id { get; }
    public OrderPriority Priority { get; }
    public long CreatedAtTicks { get; }

    public Order(long id, OrderPriority p, long createdAtTicks)
    {
        Id = id;
        Priority = p;
        CreatedAtTicks = createdAtTicks;
    }
}

public class OrderSimulator
{
    public int ProducerCount { get; }
    public int MaxOrders { get; }
    public int ExpressPercent { get; }
    public bool UseAsyncDelays { get; }

    // Fronty pro express a normal
    private readonly ConcurrentQueue<Order> _express = new();
    private readonly ConcurrentQueue<Order> _normal = new();

    // Počitadla přes Interlocked
    private long _generated = 0;
    private long _processed = 0;
    private long _totalLatencyTicks = 0;

    private readonly CancellationToken _ct;

    public OrderSimulator(int producerCount, int maxOrders, int expressPercent, bool asyncDelays, CancellationToken ct)
    {
        ProducerCount = producerCount;
        MaxOrders = maxOrders;
        ExpressPercent = expressPercent;
        UseAsyncDelays = asyncDelays;
        _ct = ct;
    }

    // ---------------------------
    // ÚKOL #1
    // Doplňte producenty
    // ---------------------------
    public IEnumerable<Task> StartProducers()
    {
        var tasks = new List<Task>();

        for (int i = 0; i < ProducerCount; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                while (!_ct.IsCancellationRequested)
                {
                    long current;
                    long next;
                    do
                    {
                        current = Interlocked.Read(ref _generated);
                        if (current >= MaxOrders) return;
                        next = current + 1;
                    } while (Interlocked.CompareExchange(ref _generated, next, current) != current);

                    bool express = Random.Shared.Next(0, 100) < ExpressPercent;
                    var order = new Order(
                        id: next,
                        p: express ? OrderPriority.Express : OrderPriority.Normal,
                        createdAtTicks: Stopwatch.GetTimestamp()
                    );

                    if (express)
                        _express.Enqueue(order);
                    else
                        _normal.Enqueue(order);

                    Thread.Sleep(1); // lehké zpomalení produkce
                }
            }, _ct));
        }

        return tasks;
    }

    // ---------------------------
    // ÚKOL #2
    // Doplňte konzument (jeden worker)
    // Pravidla:
    // - vždy nejdřív zkusit express frontu
    // - pokud prázdné, zkusit normal
    // ---------------------------
    public async Task StartConsumerAsync()
    {
        while (!_ct.IsCancellationRequested)
        {
            Order? order = null;

            if (_express.TryDequeue(out var expressOrder))
                order = expressOrder;
            else if (_normal.TryDequeue(out var normalOrder))
                order = normalOrder;

            if (order == null)
            {
                if (GeneratedCount >= MaxOrders && _express.IsEmpty && _normal.IsEmpty)
                    break;

                await Task.Delay(1);
                continue;
            }

            // ---------------------------
            // ÚKOL #3 – simulace tří kroků
            // ---------------------------
            await ProcessOrderAsync(order);

            var latencyTicks = Stopwatch.GetTimestamp() - order.CreatedAtTicks;
            Interlocked.Add(ref _totalLatencyTicks, latencyTicks);
            Interlocked.Increment(ref _processed);
        }
    }

    private async Task ProcessOrderAsync(Order order)
    {
        if (UseAsyncDelays)
        {
            await Task.Delay(Random.Shared.Next(1, 5)); // Platba 1–4 ms
            await Task.Delay(Random.Shared.Next(1, 4)); // Sklad 1–3 ms
            await Task.Delay(Random.Shared.Next(1, 5)); // Doručení 1–4 ms
        }
        else
        {
            Thread.Sleep(Random.Shared.Next(1, 5));
            Thread.Sleep(Random.Shared.Next(1, 4));
            Thread.Sleep(Random.Shared.Next(1, 5));
        }
    }

    public long ProcessedCount => Interlocked.Read(ref _processed);
    public long GeneratedCount => Interlocked.Read(ref _generated);
    public long TotalLatencyTicks => Interlocked.Read(ref _totalLatencyTicks);
}
public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("=== TEST ORDER SIMULATOR ===");

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(100));

        var sim = new OrderSimulator(
            producerCount: 8,
            maxOrders: 2000,
            expressPercent: 10,
            asyncDelays: true,
            ct: cts.Token
        );

        Console.WriteLine("E-shop běží. Stiskněte Enter pro ukončení.");

        var stopwatch = Stopwatch.StartNew();

        var producerTasks = new List<Task>(sim.StartProducers());

        var consumerTasks = new List<Task>();
        for (int i = 0; i < 3; i++)
            consumerTasks.Add(sim.StartConsumerAsync());

        var runningTasks = new List<Task>();
        runningTasks.AddRange(producerTasks);
        runningTasks.AddRange(consumerTasks);

        var enterListenerTask = Task.Run(() =>
        {
            Console.ReadLine();
            cts.Cancel();
        });

        var simulationDoneTask = Task.Run(async () =>
        {
            while (!cts.Token.IsCancellationRequested)
            {
                var generated = sim.GeneratedCount;
                var processed = sim.ProcessedCount;

                if (generated >= sim.MaxOrders && processed >= generated)
                    break;

                await Task.Delay(10);
            }
        });

        await Task.WhenAny(simulationDoneTask, enterListenerTask);
        cts.Cancel();

        try
        {
            await Task.WhenAll(runningTasks);
        }
        catch (OperationCanceledException)
        {
        }

        stopwatch.Stop();

        Console.WriteLine();
        Console.WriteLine("=== KONEC E-SHOPU ===");


        var averageLatencyMs = sim.TotalLatencyTicks * 1000.0 / Stopwatch.Frequency / sim.ProcessedCount;
        var throughput = sim.ProcessedCount / stopwatch.Elapsed.TotalSeconds;

        Console.WriteLine($"Vygenerováno:     {sim.GeneratedCount}");
        Console.WriteLine($"Zpracováno:       {sim.ProcessedCount}");
        Console.WriteLine($"Průměrná latence: {averageLatencyMs} ms");
        Console.WriteLine($"Propustnost:      {throughput} obj/s");
    }
}

/* Výstup:
- při doběhuntí programu bez zásahu uživatele (vše zpracováno):
=== TEST ORDER SIMULATOR ===
E-shop běží. Stiskněte Enter pro ukončení.

=== KONEC E-SHOPU ===
Vygenerováno: 1000
Zpracováno:   1000 

- při přerušení uživatelem (např. stisknutí Enter):
=== TEST ORDER SIMULATOR ===
E-shop běží. Stiskněte Enter pro ukončení.

=== KONEC E-SHOPU ===
Vygenerováno: 782
Zpracováno:   206
*/