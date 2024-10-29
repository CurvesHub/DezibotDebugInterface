using DeziBotDebugInterface.Api.Models;

namespace DeziBotDebugInterface.Api.Repositories;

/// <inheritdoc />
public class SensorRepository : ISensorRepository // TODO: Implement missing methods
{
    /// <inheritdoc />
    public IAsyncEnumerable<Sensor> GetSensorsAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IAsyncEnumerable<Sensor> GetSensorByIdAsync(string serialNumber)
    {
        throw new NotImplementedException();
    }
}