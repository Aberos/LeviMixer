using LeviMixer.Infrastructure.SystemAudio.Linux.PipeWire;

namespace LeviMixer.Infrastructure.SystemAudio.Linux.Tests;

public sealed class DefaultSinkSettingTests
{
    private const string ConfiguredOutput = """
        Found "default" metadata 41
        update: id:0 key:'default.configured.audio.sink' value:'{"name":"alsa_output.usb-umc22"}' type:'Spa:String:JSON'
        """;

    [Fact]
    public void Reads_the_configured_default_sink()
    {
        var setting = new DefaultSinkSetting(new RecordingCommandRunner(ConfiguredOutput));

        Assert.Equal("""{"name":"alsa_output.usb-umc22"}""", setting.Read());
    }

    [Fact]
    public void Missing_configuration_reads_as_null()
    {
        var setting = new DefaultSinkSetting(new RecordingCommandRunner("""Found "default" metadata 41"""));

        Assert.Null(setting.Read());
    }

    [Fact]
    public void Leftover_virtual_sink_from_a_crash_reads_as_null()
    {
        var leftover = ConfiguredOutput.Replace("alsa_output.usb-umc22", "levimixer_pc", StringComparison.Ordinal);

        Assert.Null(new DefaultSinkSetting(new RecordingCommandRunner(leftover)).Read());
    }

    [Fact]
    public void Points_default_to_a_node_by_name()
    {
        var runner = new RecordingCommandRunner();

        new DefaultSinkSetting(runner).PointTo("levimixer_pc");

        Assert.Equal("""pw-metadata 0 default.configured.audio.sink {"name":"levimixer_pc"} Spa:String:JSON""", runner.Commands.Single());
    }

    [Fact]
    public void Restoring_nothing_deletes_the_configuration()
    {
        var runner = new RecordingCommandRunner();

        new DefaultSinkSetting(runner).Restore(null);

        Assert.Equal("pw-metadata -d 0 default.configured.audio.sink", runner.Commands.Single());
    }

    [Fact]
    public void Restoring_a_previous_value_writes_it_back()
    {
        var runner = new RecordingCommandRunner();

        new DefaultSinkSetting(runner).Restore("""{"name":"alsa_output.usb-umc22"}""");

        Assert.Equal("""pw-metadata 0 default.configured.audio.sink {"name":"alsa_output.usb-umc22"} Spa:String:JSON""", runner.Commands.Single());
    }
}
