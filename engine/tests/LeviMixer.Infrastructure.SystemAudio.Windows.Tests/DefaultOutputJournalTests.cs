namespace LeviMixer.Infrastructure.SystemAudio.Windows.Tests;

public sealed class DefaultOutputJournalTests : IDisposable
{
    private readonly TemporaryJournal _journal = new();

    [Fact]
    public void Nothing_is_pending_before_a_save()
    {
        Assert.Null(_journal.Journal.Pending());
    }

    [Fact]
    public void Saved_outputs_are_read_back()
    {
        _journal.Journal.Save(WindowsDevices.InterfaceAsDefault);

        Assert.Equal(WindowsDevices.InterfaceAsDefault, _journal.Journal.Pending());
    }

    [Fact]
    public void Unknown_roles_survive_the_round_trip()
    {
        var outputs = new DefaultOutputs("{0.0.0.00000000}.{interface}", null, null);

        _journal.Journal.Save(outputs);

        Assert.Equal(outputs, _journal.Journal.Pending());
    }

    [Fact]
    public void Clearing_removes_the_pending_outputs()
    {
        _journal.Journal.Save(WindowsDevices.InterfaceAsDefault);

        _journal.Journal.Clear();

        Assert.Null(_journal.Journal.Pending());
    }

    public void Dispose() => _journal.Dispose();
}
