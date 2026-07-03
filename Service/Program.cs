using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol;
using System.IO;
using NORCE.Drilling.GravitationalField.Service;
using NORCE.Drilling.GravitationalField.Service.Managers;
using NORCE.Drilling.GravitationalField.Service.Mcp;
using NORCE.Drilling.GravitationalField.Service.Mcp.Tools;

var builder = WebApplication.CreateBuilder(args);

string externalConfigPath = builder.Configuration["GRAVITATIONALFIELD_EXTERNAL_CONFIG"]
    ?? Path.Combine(SqlConnectionManager.HOME_DIRECTORY, "GravitationalField.Service.json");
builder.Configuration.AddJsonFile(externalConfigPath, optional: true, reloadOnChange: true);

// registering the manager of SQLite connections through dependency injection
builder.Services.AddSingleton(sp =>
{
    var logger = sp.GetRequiredService<ILogger<SqlConnectionManager>>();

    var dbPath = Path.Combine(SqlConnectionManager.HOME_DIRECTORY,
                              SqlConnectionManager.DATABASE_FILENAME);

    // Ensure the directory exists before we build/use the connection string
    Directory.CreateDirectory(SqlConnectionManager.HOME_DIRECTORY);

    var csb = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder
    {
        DataSource = dbPath,
        // Optional but safe:
        Mode = Microsoft.Data.Sqlite.SqliteOpenMode.ReadWriteCreate,
        Cache = Microsoft.Data.Sqlite.SqliteCacheMode.Shared
    };

    return new SqlConnectionManager(csb.ToString(), logger, dbPath);
});

// registering the database cleaner service through dependency injection
builder.Services.AddHostedService(sp => new DatabaseCleanerService(
    sp.GetRequiredService<ILogger<DatabaseCleanerService>>(),
    sp.GetRequiredService<SqlConnectionManager>()));

// serialization settings (using System.Json)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        JsonSettings.ApplyTo(options.JsonSerializerOptions);
    });

// serialize using short name rather than full names
builder.Services.AddSwaggerGen(config =>
{
    config.CustomSchemaIds(type => type.FullName);
});

builder.Services.Configure<McpHubOptions>(builder.Configuration.GetSection(McpHubOptions.SectionName));
builder.Services.AddHttpClient(nameof(McpHubRegistrationService));
builder.Services.AddHostedService<McpHubRegistrationService>();

var serverVersion = typeof(SqlConnectionManager).Assembly.GetName().Version?.ToString() ?? "1.0.0";

builder.Services.AddMcpServer(options =>
{
    options.ServerInfo = new Implementation
    {
        Name = "GravitationalFieldService",
        Version = serverVersion
    };
    options.Capabilities = new ServerCapabilities
    {
        Tools = new ToolsCapability()
    };
}).WithHttpTransport();

builder.Services.AddLegacyMcpTool<PingMcpTool>();
builder.Services.AddLegacyMcpTool<GetAllGravitationalFieldIdsMcpTool>();
builder.Services.AddLegacyMcpTool<GetAllGravitationalFieldMetaInfoMcpTool>();
builder.Services.AddLegacyMcpTool<GetGravitationalFieldByIdMcpTool>();
builder.Services.AddLegacyMcpTool<GetAllGravitationalFieldMcpTool>();
builder.Services.AddLegacyMcpTool<GetAllCompletedGravitationalFieldMcpTool>();
builder.Services.AddLegacyMcpTool<PostGravitationalFieldMcpTool>();
builder.Services.AddLegacyMcpTool<PutGravitationalFieldByIdMcpTool>();
builder.Services.AddLegacyMcpTool<DeleteGravitationalFieldByIdMcpTool>();
builder.Services.AddLegacyMcpTool<GetAllGravitationalFieldCalculationOrderIdsMcpTool>();
builder.Services.AddLegacyMcpTool<GetAllGravitationalFieldCalculationOrderMetaInfoMcpTool>();
builder.Services.AddLegacyMcpTool<GetGravitationalFieldCalculationOrderByIdMcpTool>();
builder.Services.AddLegacyMcpTool<GetAllGravitationalFieldCalculationOrderLightMcpTool>();
builder.Services.AddLegacyMcpTool<GetAllGravitationalFieldCalculationOrderMcpTool>();
builder.Services.AddLegacyMcpTool<PostGravitationalFieldCalculationOrderMcpTool>();
builder.Services.AddLegacyMcpTool<PutGravitationalFieldCalculationOrderByIdMcpTool>();
builder.Services.AddLegacyMcpTool<DeleteGravitationalFieldCalculationOrderByIdMcpTool>();
builder.Services.AddLegacyMcpTool<GetGravitationalFieldUsageStatisticsMcpTool>();

var app = builder.Build();

var basePath = "/GravitationalField/api";

app.UsePathBase(basePath);

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto
});

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

string relativeSwaggerPath = "/swagger/merged/swagger.json";
string fullSwaggerPath = $"{basePath}{relativeSwaggerPath}";
string customVersion = "Merged API Version 1";
string exposedModel = "wwwroot/json-schema/GravitationalFieldMergedModel.json";
if (File.Exists(exposedModel))
{
    var mergedDoc = SwaggerMiddlewareExtensions.ReadOpenApiDocument(exposedModel);
    app.UseCustomSwagger(mergedDoc, relativeSwaggerPath);
    app.UseSwaggerUI(c =>
    {
        //c.SwaggerEndpoint("v1/swagger.json", "API Version 1");
        c.SwaggerEndpoint(fullSwaggerPath, customVersion);
    });
}

app.UseCors(cors => cors
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowed(origin => true)
                        .AllowCredentials()
           );

app.MapMcp("/mcp");
app.MapMcpWebSocket("/mcp/ws");
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
