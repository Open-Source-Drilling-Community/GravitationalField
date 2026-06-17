# NORCE.Drilling.GravitationalField.WebPages

Reusable Razor class library for the Gravitational Field web UI.

It contains the `Home`, `GravitationalFieldCalculationOrderMain`, `GravitationalFieldCalculationOrderEdit`, and `GravitationalFieldView` pages together with the supporting API and UI utility code they depend on.

## Package contents

- Home page
- Gravitational field calculation order list and edit pages
- Gravitational field result view page
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
