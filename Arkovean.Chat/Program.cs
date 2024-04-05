using Arkovean.Chat.Hubs;
using Arkovean.Chat.Startup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.NativeServiceRegistration();
builder.Services.CustomServiceRegistration();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
