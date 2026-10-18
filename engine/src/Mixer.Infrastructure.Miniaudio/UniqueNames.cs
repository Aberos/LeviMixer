namespace Mixer.Infrastructure.Miniaudio;

internal sealed class UniqueNames
{
    private readonly Dictionary<string, int> _occurrences = new(StringComparer.Ordinal);

    public string Reserve(string name)
    {
        var occurrence = _occurrences.GetValueOrDefault(name) + 1;
        _occurrences[name] = occurrence;
        return occurrence == 1 ? name : $"{name} #{occurrence}";
    }
}
