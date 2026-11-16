namespace LeviMixer.Infrastructure.SystemAudio.Windows;

public sealed record DefaultOutputs(string? Console, string? Multimedia, string? Communications)
{
    public bool IsEmpty => Console is null && Multimedia is null && Communications is null;
}
