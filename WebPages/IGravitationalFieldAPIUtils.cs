using NORCE.Drilling.GravitationalField.ModelShared;

namespace NORCE.Drilling.GravitationalField.WebPages;

public interface IGravitationalFieldAPIUtils
{
    string HostNameGravitationalField { get; }
    string HostBasePathGravitationalField { get; }
    HttpClient HttpClientGravitationalField { get; }
    Client ClientGravitationalField { get; }

    string HostNameUnitConversion { get; }
    string HostBasePathUnitConversion { get; }
}
