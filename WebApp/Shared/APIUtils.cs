public static class APIUtils
{
    // API parameters
    public static readonly string HostNameGravitationalField = NORCE.Drilling.GravitationalField.WebApp.Configuration.GravitationalFieldHostURL!;
    public static readonly string HostBasePathGravitationalField = "GravitationalField/api/";
    public static readonly HttpClient HttpClientGravitationalField = APIUtils.SetHttpClient(HostNameGravitationalField, HostBasePathGravitationalField);
    public static readonly NORCE.Drilling.GravitationalField.ModelShared.Client ClientGravitationalField = new NORCE.Drilling.GravitationalField.ModelShared.Client(APIUtils.HttpClientGravitationalField.BaseAddress!.ToString(), APIUtils.HttpClientGravitationalField);

    public static readonly string HostNameUnitConversion = NORCE.Drilling.GravitationalField.WebApp.Configuration.UnitConversionHostURL!;
    public static readonly string HostBasePathUnitConversion = "UnitConversion/api/";

    // API utility methods
    public static HttpClient SetHttpClient(string host, string microServiceUri)
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }; // temporary workaround for testing purposes: bypass certificate validation (not recommended for production environments due to security risks)
        HttpClient httpClient = new(handler)
        {
            BaseAddress = new Uri(host + microServiceUri)
        };
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        return httpClient;
    }
}