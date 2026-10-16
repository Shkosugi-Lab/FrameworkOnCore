// perf-baseline: the environment's own cost, without System.Web (load.ps1). Kestrel answers a page of the size of
// PipelineProbe's Events.aspx; then System.Web's habits one at a time: the hop to a dedicated thread (WorkerThreads), ViewState's
// AES + HMAC, culture-aware string comparisons.
using System.Collections.Concurrent;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
var app = builder.Build();
var page = System.Text.Encoding.UTF8.GetBytes(new string('x', 6000));
var state = new byte[2000];
RandomNumberGenerator.Fill(state);
var key = RandomNumberGenerator.GetBytes(32);
var aes = new ThreadLocal<Aes>(() => { var a = Aes.Create(); a.Key = key; return a; });
var hmac = new ThreadLocal<HMACSHA256>(() => new HMACSHA256(key));
var words = Enumerable.Range(0, 200).Select(i => "Control" + i + "_Name").ToArray();

Task Write(HttpContext context) { context.Response.ContentType = "text/html"; return context.Response.Body.WriteAsync(page).AsTask(); }

app.MapGet("/plain", Write);
app.MapGet("/thread", async context => { await Hop.Run(() => { }); await Write(context); });
app.MapGet("/crypto", async context => {
    // as many protects as a page with view state and event validation: 2
    for (int i = 0; i < 2; i++) {
        var iv = RandomNumberGenerator.GetBytes(16);
        var enc = aes.Value!.EncryptCbc(state, iv);
        hmac.Value!.ComputeHash(enc);
    }
    await Write(context);
});
app.MapGet("/culture", async context => {
    int n = 0;
    foreach (var w in words) if (string.Compare(w, "control99_name", StringComparison.CurrentCultureIgnoreCase) == 0) n++;
    foreach (var w in words) if (w.StartsWith("Control1", StringComparison.CurrentCulture)) n++;
    await Write(context);
});
app.Run();

// WorkerThreads' way: idle dedicated threads wait on one semaphore; a request is queued and one is released.
static class Hop
{
    static readonly ConcurrentQueue<(Action, TaskCompletionSource)> queue = new();
    static readonly SemaphoreSlim signal = new(0);
    static int idle;
    public static Task Run(Action work)
    {
        var done = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        while (true) {
            var c = Volatile.Read(ref idle);
            if (c <= 0) { new Thread(() => Loop((work, done))) { IsBackground = true }.Start(); break; }
            if (Interlocked.CompareExchange(ref idle, c - 1, c) == c) { queue.Enqueue((work, done)); signal.Release(); break; }
        }
        return done.Task;
    }
    static void Loop((Action, TaskCompletionSource) item)
    {
        while (true) {
            item.Item1(); item.Item2.SetResult();
            Interlocked.Increment(ref idle);
            signal.Wait();
            while (!queue.TryDequeue(out item)) Thread.Yield();
        }
    }
}
