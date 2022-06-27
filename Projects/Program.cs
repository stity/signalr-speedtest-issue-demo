using SpeedTest.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddRazorPages();
builder.Services.AddSignalR().AddMessagePackProtocol();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.MapHub<SpeedTestHub>("/speedTest", options =>
{
    options.TransportMaxBufferSize = 0;
    options.ApplicationMaxBufferSize = 0;

});
app.UseWebSockets();

app.Run();
