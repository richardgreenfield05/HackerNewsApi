using AspNetCoreRateLimit;
using HackerNewsApi.Clients.HackerNews;
using HackerNewsApi.Exceptions;
using Serilog;
Environment.SetEnvironmentVariable("CORECLR_GLOBAL_INVARIANT", "1");
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddHttpClient<HttpClient>();
builder.Services.AddHttpClient<HackerNewsClient>(client =>
{
    var baseUrl = builder.Configuration["HackerNewsApi:BaseUrl"];
    if (baseUrl != null)
    {
        client.BaseAddress = new Uri(baseUrl);
    }
    else
    {
        throw new ArgumentNullException("HackerNewsApi:BaseUrl", "Base URL must be provided in the configuration.");
    }
});

builder.Services.AddCustomRateLimiting(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseMiddleware<ExceptionHandlingMiddleware>();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.MapControllers();
app.UseIpRateLimiting();
app.Run();

Log.CloseAndFlush();
