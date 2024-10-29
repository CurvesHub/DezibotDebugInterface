using DeziBotDebugInterface.Api.Models;

namespace DeziBotDebugInterface.Api.Repositories;

/// <summary>
/// Provides data access for sensors.
/// </summary>
public interface ISensorRepository
{
    /// <summary>
    /// Gets all sensors.
    /// </summary>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Sensor"/>.</returns>
    IAsyncEnumerable<Sensor> GetSensorsAsync();
    
    /// <summary>
    /// Gets a sensor by its serial number.
    /// </summary>
    /// <param name="serialNumber">The serial number of the sensor to get.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="Sensor"/>.</returns>
    IAsyncEnumerable<Sensor> GetSensorByIdAsync(string serialNumber);
}