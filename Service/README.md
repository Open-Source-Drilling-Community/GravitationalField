# GravitationalField microservice

The GravitationalField microservice is packaged as a docker container named:

``norcedrillinggravitationalfieldservice``

It is available on dockerhub, under the digiwells organization, at:

https://hub.docker.com/?namespace=digiwells

The API (OpenApi schema) of the microservice is available and testable at:

https://dev.digiwells.no/GravitationalField/api/swagger (development server) 

https://app.digiwells.no/GravitationalField/api/swagger (production server)

The microservice itself is available at:

https://dev.digiwells.no/GravitationalField/api/GravitationalFieldCalculationOrder

https://app.digiwells.no/GravitationalField/api/GravitationalFieldCalculationOrder

# Calculation data

Calculation orders contain raw and completed gravitational fields. Input points use:

- `Latitude`: geodetic latitude in SI radians.
- `Longitude`: geodetic longitude in SI radians.
- `Depth`: drilling depth in SI metres.

Completed points preserve the input position and add the calculated gravity vector components `GravitatyIntensityX`, `GravitatyIntensityY`, and `GravitatyIntensityZ`.

The latitude property is named `Latitude`. Payloads using the former misspelled `Lattitude` property should be regenerated against the current OpenAPI schema.

# Usage statistics

The service exposes aggregate per-endpoint usage counters at:

- `GET /GravitationalField/api/GravitationalFieldUsageStatistics`

The counters cover the `GravitationalField` and `GravitationalFieldCalculationOrder` CRUD endpoints, including ID, metadata, heavy/light data queries, completed-field queries, and write/delete operations. The history is persisted periodically to `../home/history.json` when the runtime `home` directory is available.

# MCP support

The microservice exposes an embedded MCP server so MCP-aware clients can discover and call the GravitationalField API as tools.

The HTTP MCP endpoint is:

- `POST /GravitationalField/api/mcp`

The WebSocket MCP endpoint is:

- `GET /GravitationalField/api/mcp/ws`

The exposed tool groups are:

- `ping`
- `gravitational_field.*` for GravitationalField CRUD, metadata, ID, and completed-state queries
- `gravitational_field_calculation_order.*` for calculation-order CRUD, metadata, ID, light, and full-data queries
- `gravitational_field_usage_statistics.get` for aggregate usage statistics

The current tool names are:

- `gravitational_field.get_all_ids`
- `gravitational_field.get_all_meta_info`
- `gravitational_field.get_by_id`
- `gravitational_field.get_all`
- `gravitational_field.get_all_completed`
- `gravitational_field.create`
- `gravitational_field.update_by_id`
- `gravitational_field.delete_by_id`
- `gravitational_field_calculation_order.get_all_ids`
- `gravitational_field_calculation_order.get_all_meta_info`
- `gravitational_field_calculation_order.get_by_id`
- `gravitational_field_calculation_order.get_all_light`
- `gravitational_field_calculation_order.get_all`
- `gravitational_field_calculation_order.create`
- `gravitational_field_calculation_order.update_by_id`
- `gravitational_field_calculation_order.delete_by_id`
- `gravitational_field_usage_statistics.get`

The service can optionally register itself on an MCP hub. The registration is configured with the `McpHub` section in `appsettings.json` or in an external JSON configuration file. By default the Docker image looks for that external file at:

- `/home/GravitationalField.Service.json`

The path can be overridden with the environment variable:

- `GRAVITATIONALFIELD_EXTERNAL_CONFIG`

The MCP hub configuration uses:

- `Enabled`: enables or disables hub registration.
- `HubBaseUrl`: base URL of the MCP hub.
- `RegistrationEndpoint`: CRUD endpoint on the hub, defaulting to `McpMicroservice`.
- `RetryIntervalSeconds`: delay between registration retries, defaulting to 60 seconds.
- `PublicBaseUrl`: externally reachable base URL of this microservice.
- `ServiceName`: display name registered on the hub.
- `InstanceId`: optional stable instance GUID. If omitted, the service creates and persists one in the runtime home directory.
- `UnregisterOnShutdown`: removes the registration from the hub when the service stops.

# Source code

The present microservice and webapp solution has been generated from a NORCE Drilling and Well Modelling team dotnet template.
Creation date: 22.04.2026
Version: 4.0.25
Source code of the dotnet template can be found here: https://github.com/NORCE-DrillingAndWells/Templates
Documentation relative to the template can be found here: https://github.com/NORCE-DrillingAndWells/DrillingAndWells/wiki/.NET-Templates

# Funding

The current work has been funded by the [Research Council of Norway](https://www.forskningsradet.no/) and [Industry partners](https://www.digiwells.no/about/board/) in the framework of the cent for research-based innovation [SFI Digiwells (2020-2028)](https://www.digiwells.no/) focused on Digitalization, Drilling Engineering and GeoSteering. 

# Contributors

**Lucas Volpi**, *NORCE Energy Modelling and Automation*

**Eric Cayeux**, *NORCE Energy Modelling and Automation*
