namespace LeviMixer.Dsp.Metering;

public struct BlockLevel
{
    private float _peak;
    private float _sumOfSquares;
    private int _sampleCount;

    public void Add(float sample)
    {
        var magnitude = MathF.Abs(sample);
        _peak = MathF.Max(_peak, magnitude);
        _sumOfSquares += sample * sample;
        _sampleCount++;
    }

    public readonly void RecordInto(LevelMeter meter, int frameCount)
    {
        ArgumentNullException.ThrowIfNull(meter);
        var meanSquare = _sampleCount == 0 ? 0f : _sumOfSquares / _sampleCount;
        meter.Record(_peak, meanSquare, frameCount);
    }
}
