# GravitationalField model

The model project defines the data structures and calculation logic used by the GravitationalField microservice.

## Main objects

- `GravitationalData`: one calculation point or result point.
- `GravitationalField`: a collection of gravitational data points, marked as `Raw` or `Completed`.
- `GravitationalFieldCalculationOrder`: an order containing one raw gravitational field and one completed gravitational field.
- `UsageStatisticsGravitationalField`: per-endpoint usage counters persisted by the service.

## Gravitational data

Each `GravitationalData` point uses:

- `Latitude`: geodetic latitude, in SI radians.
- `Longitude`: geodetic longitude, in SI radians.
- `Depth`: drilling depth, in SI metres.
- `GravityIntensityX`: calculated easterly gravity component, in metres per second squared.
- `GravityIntensityY`: calculated northerly gravity component, in metres per second squared.
- `GravityIntensityZ`: calculated upward gravity component, normally negative, in metres per second squared.

The public property is named `Latitude`. Older generated or serialized payloads that used the misspelled `Lattitude` property should be regenerated or migrated to `Latitude`.

## Calculation

`GravitationalFieldCalculationOrder.Calculate()` evaluates each raw data point with the EGM96 gravity model. The model receives latitude, longitude, and elevation; the order converts drilling depth to elevation by passing `-Depth`.

For every raw point, a completed point is produced with the same latitude, longitude, and depth, and with the three calculated gravity intensity components.

## Units

The model stores values in SI units. Unit and reference display conversions are handled in the web UI by the shared unit/reference components.
