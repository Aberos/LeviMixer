namespace LeviMixer.Infrastructure.SystemAudio.Windows.Tests;

internal sealed class TemporaryJournal : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), $"levimixer-tests-{Guid.NewGuid():N}");

    public TemporaryJournal()
    {
        Journal = new DefaultOutputJournal(Path.Combine(_directory, "default-output.txt"));
    }

    public DefaultOutputJournal Journal { get; }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }
}
