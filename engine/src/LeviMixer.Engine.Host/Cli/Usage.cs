namespace LeviMixer.Engine.Host.Cli;

internal static class Usage
{
    public static int Print()
    {
        System.Console.Error.WriteLine("""
            levimixer-engine ipc
            levimixer-engine devices
            levimixer-engine run [--input <id>] [--output <id>] [--buffer <frames>] [--master <dB|-inf>] [--seconds <n>]
            """);
        return 1;
    }
}
