# NORCE.Drilling.GravitationalField.WebPages

Reusable Razor class library for the Gravitational Field web UI.

It contains the `Home`, `GravitationalFieldSingleCalculation`, `GravitationalFieldCalculationOrderMain`, `GravitationalFieldCalculationOrderEdit`, `GravitationalFieldView`, and `StatisticsGravitationalField` pages together with the supporting API and UI utility code they depend on.

The calculation pages edit and display points with `Latitude`, `Longitude`, and `Depth`. The UI stores and sends SI values to the service while using the configured unit components for display and input conversion. Latitude and longitude are radians, and depth is metres below the WGS84 ellipsoid, positive downward. Results use `GravityIntensityX/Y/Z` for east/north/up acceleration in metres per second squared.

## Package contents

- Home page
- Single-case earth gravitational field calculator
- Gravitational field calculation order list and edit pages with Save/Close behavior and dirty-close confirmation
- Gravitational field result view page
- Usage statistics page
- Host-configurable API access through injected configuration

## Dependencies

- `OSDC.DotnetLibraries.Drilling.WebAppUtils`
- `MudBlazor`
- `OSDC.UnitConversion.DrillingRazorMudComponents`
- `Plotly.Blazor`
- `ModelSharedOut`

## Host integration

The consuming app should:

1. Reference this package.
2. Provide an implementation of `IGravitationalFieldWebPagesConfiguration`.
3. Register that configuration and `IGravitationalFieldAPIUtils` in DI.
4. Add the `WebPages` assembly to the Blazor router `AdditionalAssemblies`.

## Required configuration

- `GravitationalFieldHostURL`
- `UnitConversionHostURL`

# Funding

The current work has been funded by the [Research Council of Norway](https://www.forskningsradet.no/) and [Industry partners](https://www.digiwells.no/about/board/) in the framework of the cent for research-based innovation [SFI Digiwells (2020-2028)](https://www.digiwells.no/) focused on Digitalization, Drilling Engineering and GeoSteering. 

# Contributors

**Lucas Volpi**, *NORCE Energy Modelling and Automation*

**Eric Cayeux**, *NORCE Energy Modelling and Automation*
