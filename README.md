# GravitationalField

This repository is replaced by https://github.com/Open-Source-Drilling-Community/EarthGravity. The repository is therefore turned be in archive mode.

The GravitationalField repository hosts a microservice and client webapp for GravitationalField.

Its public calculation contract uses WGS84 latitude and longitude in SI radians and true vertical depth below the WGS84 ellipsoid in metres, positive downward. The service converts these values to GeographicLib's degree/ellipsoidal-height convention internally and returns EGM96 east/north/up acceleration as `GravityIntensityX/Y/Z` in metres per second squared.

# Solution architecture

The solution is composed of:
- **ModelSharedIn**
  - contains C# auto-generated classes of Model dependencies
  - these dependencies are stored as json files (following the OpenApi standard) and C# classes are generated on execution of the program
  - *dependencies* = some external microservices (OpenApi schemas in json format)
- **Model**
  - defines the main classes and methods to run the microservice
  - represents calculation points with `Latitude`, `Longitude`, and `Depth`, and stores calculated gravity vector components
  - tracks per-endpoint usage statistics in `UsageStatisticsGravitationalField`
  - *dependencies* = BaseModels
- **Service**
  - defines the proper microservice API
  - exposes an embedded MCP server for AI tool calls against the GravitationalField API and its usage statistics
  - can optionally register its MCP endpoint on an MCP hub using external configuration
  - *dependencies* = Model
- **ModelSharedOut**
  - contains C# auto-generated classes for microservice clients dependencies
  - these dependencies are stored as json files (following the OpenAPI standard) and C# classes are generated on execution of the program
  - these dependencies include the OpenApi schema of the microservice itself as well as other dependencies that may be useful to run the microservice
  - *dependencies* = GravitationalField.json + some external microservices (OpenApi schemas in json format)
- **ModelTest**
  - performs unit tests on the Model (in particular for base computations)
  - *dependencies* = Model
- **ServiceTest**
  - microservice client that performs unit tests on the microservice (by default, an instance of the microservice must be running on http port 8080 to run tests)
  - *dependencies* = ModelShared
- **WebApp**
  - microservice web app client that manages data associated with GravitationalField and allows single-case and batch interaction with the microservice
  - provides a harmonized batch calculation editor, a single-case calculator, and usage statistics pages
  - *dependencies* = ModelShared
- **home** (auto-generated)
  - data are persisted in the microservice container using the Sqlite database located at *home/GravitationalField.db*

# Security/Confidentiality

Data are persisted as clear text in a unique Sqlite database hosted in the docker container.
Neither authentication nor authorization have been implemented.
Would you like or need to protect your data, docker containers of the microservice and webapp are available on dockerhub, under the digiwells organization, at:

https://hub.docker.com/?namespace=digiwells

More info on how to run the container and map its database to a folder on your computer, at:

https://github.com/NORCE-DrillingAndWells/DrillingAndWells/wiki

# Deployment

Microservice is available at:

https://dev.digiwells.no/GravitationalField/api/GravitationalFieldCalculationOrder

https://app.digiwells.no/GravitationalField/api/GravitationalFieldCalculationOrder

Web app management page is available at:

https://dev.digiwells.no/GravitationalField/webapp/GravitationalField

https://app.digiwells.no/GravitationalField/webapp/GravitationalField

The single-case gravitational field calculator is available at:

https://dev.digiwells.no/GravitationalField/webapp/GravitationalFieldSingleCalculation

https://app.digiwells.no/GravitationalField/webapp/GravitationalFieldSingleCalculation

Usage statistics are available at:

https://dev.digiwells.no/GravitationalField/webapp/StatisticsGravitationalField

https://app.digiwells.no/GravitationalField/webapp/StatisticsGravitationalField

The embedded MCP server is available at:

https://dev.digiwells.no/GravitationalField/api/mcp

https://app.digiwells.no/GravitationalField/api/mcp

The MCP WebSocket endpoint is available at:

https://dev.digiwells.no/GravitationalField/api/mcp/ws

https://app.digiwells.no/GravitationalField/api/mcp/ws

The OpenApi schema of the microservice is available and testable at:

https://dev.digiwells.no/GravitationalField/swagger (development server) 

https://app.digiwells.no/GravitationalField/swagger (production server)

The microservice and webapp are deployed as Docker containers using Kubernetes and Helm. More info at:

https://github.com/NORCE-DrillingAndWells/DrillingAndWells/wiki

# Funding

The current work has been funded by the [Research Council of Norway](https://www.forskningsradet.no/) and [Industry partners](https://www.digiwells.no/about/board/) in the framework of the cent for research-based innovation [SFI Digiwells (2020-2028)](https://www.digiwells.no/) focused on Digitalization, Drilling Engineering and GeoSteering. 

# Contributors

**Lucas Volpi**, *NORCE Energy Modelling and Automation*

**Eric Cayeux**, *NORCE Energy Modelling and Automation*
