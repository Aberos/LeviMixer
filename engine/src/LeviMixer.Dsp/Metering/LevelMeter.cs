namespace LeviMixer.Dsp.Metering;

public sealed class LevelMeter
{
    private const float RmsTimeConstantSeconds = 0.3f;

    private readonly float _sampleRate;
    private float _peak;
    private float _meanSquare;

    public LevelMeter(int sampleRate)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sampleRate);
        _sampleRate = sampleRate;
    }

    public void Record(float blockPeak, float blockMeanSquare, int frameCount)
    {
        RaisePeakTo(blockPeak);
        var smoothing = 1f - MathF.Exp(-frameCount / (RmsTimeConstantSeconds * _sampleRate));
        var meanSquare = Volatile.Read(ref _meanSquare);
        Volatile.Write(ref _meanSquare, meanSquare + (smoothing * (blockMeanSquare - meanSquare)));
    }

    public MeterReading TakeReading() =>
        new(Interlocked.Exchange(ref _peak, 0f), MathF.Sqrt(Volatile.Read(ref _meanSquare)));

    private void RaisePeakTo(float candidate)
    {
        var current = Volatile.Read(ref _peak);
        while (candidate > current)
        {
            var observed = Interlocked.CompareExchange(ref _peak, candidate, current);
            if (observed == current)
            {
                return;
            }

            current = observed;
        }
    }
}
