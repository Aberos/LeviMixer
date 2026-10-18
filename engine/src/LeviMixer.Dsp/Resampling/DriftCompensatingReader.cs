using LeviMixer.Dsp.Buffers;

namespace LeviMixer.Dsp.Resampling;

public sealed class DriftCompensatingReader
{
    private const int Channels = 2;

    private readonly SpscRingBuffer _source;
    private readonly DriftControl _control;
    private readonly float[] _staging;
    private double _phase;
    private float _previousLeft;
    private float _previousRight;
    private double _smoothedFill;
    private bool _isPrimed;

    public DriftCompensatingReader(SpscRingBuffer source, DriftControl control)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(control);
        _source = source;
        _control = control;
        _staging = new float[(MaxSourceFramesPerRead(control) + 1) * Channels];
    }

    public double Ratio { get; private set; } = 1d;

    public bool IsPrimed => _isPrimed;

    public void ReadInto(Span<float> stereo)
    {
        var frames = Math.Min(stereo.Length / Channels, _control.MaxFramesPerRead);
        var fill = _source.AvailableToRead / Channels;
        if (!TryPrime(fill))
        {
            stereo.Clear();
            return;
        }

        DropOverflow(ref fill);
        UpdateRatio(fill);
        var lastPosition = _phase + ((frames - 1) * Ratio);
        var needed = (int)Math.Floor(lastPosition) + 1;
        if (fill < needed)
        {
            Underrun(stereo);
            return;
        }

        _source.Read(_staging.AsSpan(0, needed * Channels));
        Interpolate(stereo, frames);
        Advance(frames, needed);
    }

    private bool TryPrime(int fill)
    {
        if (!_isPrimed && fill >= _control.TargetFrames)
        {
            _isPrimed = true;
            _smoothedFill = fill;
        }

        return _isPrimed;
    }

    private void DropOverflow(ref int fill)
    {
        if (fill <= _control.OverflowFrames)
        {
            return;
        }

        _source.Skip((fill - _control.TargetFrames) * Channels);
        fill = _control.TargetFrames;
        _smoothedFill = fill;
    }

    private void UpdateRatio(int fill)
    {
        _smoothedFill += _control.FillSmoothing * (fill - _smoothedFill);
        var error = (_smoothedFill - _control.TargetFrames) / _control.TargetFrames;
        Ratio = 1d + Math.Clamp(error * _control.Responsiveness, -_control.MaxRatioDeviation, _control.MaxRatioDeviation);
    }

    private void Underrun(Span<float> stereo)
    {
        stereo.Clear();
        _isPrimed = false;
    }

    private void Interpolate(Span<float> stereo, int frames)
    {
        for (var frame = 0; frame < frames; frame++)
        {
            var position = _phase + (frame * Ratio);
            var index = (int)position;
            var fraction = (float)(position - index);
            stereo[frame * Channels] = Lerp(SourceSample(index, 0), SourceSample(index + 1, 0), fraction);
            stereo[(frame * Channels) + 1] = Lerp(SourceSample(index, 1), SourceSample(index + 1, 1), fraction);
        }
    }

    private float SourceSample(int index, int channel)
    {
        if (index == 0)
        {
            return channel == 0 ? _previousLeft : _previousRight;
        }

        return _staging[((index - 1) * Channels) + channel];
    }

    private void Advance(int frames, int consumed)
    {
        _previousLeft = _staging[(consumed - 1) * Channels];
        _previousRight = _staging[((consumed - 1) * Channels) + 1];
        _phase = Math.Max(0d, _phase + (frames * Ratio) - consumed);
    }

    private static float Lerp(float from, float to, float fraction) => from + ((to - from) * fraction);

    private static int MaxSourceFramesPerRead(DriftControl control) =>
        (int)Math.Ceiling((control.MaxFramesPerRead + 1) * (1d + control.MaxRatioDeviation)) + 1;
}
