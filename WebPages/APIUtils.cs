using NORCE.Drilling.GravitationalField.ModelShared;

namespace NORCE.Drilling.GravitationalField.WebPages;

public class APIUtils : OSDC.DotnetLibraries.Drilling.WebAppUtils.APIUtils, IGravitationalFieldAPIUtils
{
    public APIUtils(IGravitationalFieldWebPagesConfiguration configuration)
    {
        HostNameGravitationalField = Require(configuration.GravitationalFieldHostURL, nameof(configuration.GravitationalFieldHostURL));
        HttpClientGravitationalField = SetHttpClient(HostNameGravitationalField, HostBasePathGravitationalField);
        ClientGravitationalField = new Client(HttpClientGravitationalField.BaseAddress!.ToString(), HttpClientGravitationalField);

        HostNameUnitConversion = Require(configuration.UnitConversionHostURL, nameof(configuration.UnitConversionHostURL));
    }

    private static string Require(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Configuration value '{propertyName}' must be assigned before WebPages is used.");
        }

        return value;
    }

    public string HostNameGravitationalField { get; }
    public string HostBasePathGravitationalField { get; } = "GravitationalField/api/";
    public HttpClient HttpClientGravitationalField { get; }
    public Client ClientGravitationalField { get; }

    public string HostNameUnitConversion { get; }
    public string HostBasePathUnitConversion { get; } = "UnitConversion/api/";
}
