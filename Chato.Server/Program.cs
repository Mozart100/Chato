using Chato.Server.Hubs;
using Chato.Server.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<LoggerFilterOptions>(options =>
{
    options.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Trace);
    options.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Trace);
});

// Add services to the container.
builder.Services.NativeServiceRegistration();
builder.Services.CustomServiceRegistration(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(ServiceRegistrar.CorsPolicy);
app.UseHttpsRedirection();

app.UseAuthorization();


app.UseExceptionHandler("/error");
app.MapHub<ChatHub>("/chat");
app.MapControllers();


app.Run();
