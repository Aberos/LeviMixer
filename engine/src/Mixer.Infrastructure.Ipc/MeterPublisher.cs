using Mixer.Application.UseCases;
using Mixer.Infrastructure.Ipc.Mapping;
using Mixer.Infrastructure.Ipc.Transport;
using Mixer.Infrastructure.Ipc.Wire;

namespace Mixer.Infrastructure.Ipc;

internal sealed class MeterPublisher(ReadMeters readMeters, OutboundChannel outbound, TimeSpan interval) : IDisposable
{
    private readonly CancellationTokenSource _stop = new();
    private Task? _loop;

    public void Start() => _loop = Task.Run(PublishUntilStoppedAsync);

    public void Dispose()
    {
        _stop.Cancel();
        _loop?.Wait(TimeSpan.FromSeconds(1));
        _stop.Dispose();
    }

    private async Task PublishUntilStoppedAsync()
    {
        using var timer = new PeriodicTimer(interval);
        try
        {
            while (await timer.WaitForNextTickAsync(_stop.Token).ConfigureAwait(false))
            {
                PublishOnce();
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void PublishOnce()
    {
        if (readMeters.Execute() is { } report)
        {
            outbound.Send(OutboundMessage.Create("meters", WireMapper.ToWire(report), IpcJsonContext.Default.MetersPayload));
        }
    }
}
