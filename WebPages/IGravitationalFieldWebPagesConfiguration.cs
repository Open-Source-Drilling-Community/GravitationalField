using OSDC.DotnetLibraries.Drilling.WebAppUtils;

namespace NORCE.Drilling.GravitationalField.WebPages;

public interface IGravitationalFieldWebPagesConfiguration :
    IUnitConversionHostURL
{
    string GravitationalFieldHostURL { get; }
}
