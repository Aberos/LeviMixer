using LeviMixer.Application.Devices;
using LeviMixer.Application.Results;
using LeviMixer.Application.Session;
using LeviMixer.Domain.Mixing;

namespace LeviMixer.Application.UseCases;

public sealed class StartMixing(IAudioDeviceCatalog catalog, MixerSession session)
{
    public CommandResult Execute(StartMixingRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var devices = catalog.ListDevices();
        var input = request.Input is { } inputId ? devices.FindInput(inputId) : devices.DefaultInput;
        var output = request.Output is { } outputId ? devices.FindOutput(outputId) : devices.DefaultOutput;

        if (input is null)
        {
            return CommandResult.Failure(CommandError.DeviceNotFound(request.Input?.Value ?? "default input"));
        }

        if (output is null)
        {
            return CommandResult.Failure(CommandError.DeviceNotFound(request.Output?.Value ?? "default output"));
        }

        return OpenConnection(new AudioConnectionRequest(input, output, request.Format));
    }

    private CommandResult OpenConnection(AudioConnectionRequest connectionRequest)
    {
        try
        {
            session.Start(connectionRequest, DefaultChannelLayout.ForInterface(connectionRequest.Input.ChannelCount));
            return CommandResult.Success;
        }
        catch (AudioDeviceException exception)
        {
            return CommandResult.Failure(CommandError.DeviceUnavailable(exception.Message));
        }
    }
}
