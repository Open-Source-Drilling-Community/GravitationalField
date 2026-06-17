using NORCE.Drilling.GravitationalField.WebPages;

namespace NORCE.Drilling.GravitationalField.WebApp;

public class WebPagesHostConfiguration : IGravitationalFieldWebPagesConfiguration
{
    public string GravitationalFieldHostURL { get; set; } = string.Empty;
    public string? UnitConversionHostURL { get; set; } = string.Empty;
}
