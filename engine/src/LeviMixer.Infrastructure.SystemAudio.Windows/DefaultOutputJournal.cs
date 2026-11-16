namespace LeviMixer.Infrastructure.SystemAudio.Windows;

public sealed class DefaultOutputJournal(string path)
{
    private const int LineCount = 3;

    public static string DefaultPath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LeviMixer", "default-output.txt");

    public void Save(DefaultOutputs outputs)
    {
        ArgumentNullException.ThrowIfNull(outputs);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllLines(path, [outputs.Console ?? string.Empty, outputs.Multimedia ?? string.Empty, outputs.Communications ?? string.Empty]);
    }

    public DefaultOutputs? Pending()
    {
        if (!File.Exists(path))
        {
            return null;
        }

        var lines = File.ReadAllLines(path);
        return lines.Length == LineCount ? new DefaultOutputs(ValueOf(lines[0]), ValueOf(lines[1]), ValueOf(lines[2])) : null;
    }

    public void Clear() => File.Delete(path);

    private static string? ValueOf(string line) => string.IsNullOrWhiteSpace(line) ? null : line.Trim();
}
