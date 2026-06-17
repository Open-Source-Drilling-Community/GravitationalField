using MudBlazor;
using MudBlazor.Services;
using NORCE.Drilling.GravitationalField.WebApp;
using NORCE.Drilling.GravitationalField.WebPages;

var builder = WebApplication.CreateBuilder(args);

WebPagesHostConfiguration webPagesConfiguration = new()
{
    GravitationalFieldHostURL = builder.Configuration["GravitationalFieldHostURL"] ?? string.Empty,
    UnitConversionHostURL = builder.Configuration["UnitConversionHostURL"] ?? string.Empty,
};

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});
builder.Services.AddSingleton<IGravitationalFieldWebPagesConfiguration>(webPagesConfiguration);
builder.Services.AddSingleton<IGravitationalFieldAPIUtils, APIUtils>();

var app = builder.Build();

app.UseForwardedHeaders();
// This needs to match with what is defined in "charts/<helm-chart-name>/templates/values.yaml ingress.Path
app.UsePathBase("/GravitationalField/webapp");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
