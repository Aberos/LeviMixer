namespace Mixer.Engine.Host.Cli;

internal static class Usage
{
    public static int Print()
    {
        System.Console.Error.WriteLine("""
            mixer-engine devices
            mixer-engine run [--input <id>] [--output <id>] [--buffer <frames>] [--master <dB|-inf>] [--seconds <n>]
            """);
        return 1;
    }
}
