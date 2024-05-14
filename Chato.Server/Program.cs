using Chato.Server.Errors;
using Chato.Server.Hubs;
using Chato.Server.Infrastracture;
using Chato.Server.Services;
using Chato.Server.Startup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<LoggerFilterOptions>(options =>
{
    options.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Trace);
    options.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Trace);
});

// Add services to the container.
builder.Services.NativeServiceRegistration();
builder.Services.CustomServiceRegistration(builder.Configuration);

builder.Services.AddConfig<AuthenticationConfig>(builder.Configuration);
//builder.Services.Configure<AuthenticationConfig>(builder.Configuration.GetSection(AuthenticationConfig.ApiName));



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var jsonVal = builder.Configuration.GetSection("AuthenticationConfig:Token").Value;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(AuthenticationService.GetBytes(jsonVal)),
                //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF32.GetBytes(jsonVal)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

builder.Services.AddTransient<IPreloadDataLoader, GenerateDefaultRoomAndUsersService>();


//var serviceProvider = builder.Services.BuildServiceProvider();
//var preload = serviceProvider.GetRequiredService<IPreloadDataLoader>();
//await preload.ExecuteAsync();



var app = builder.Build();
app.UseAuthentication();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();


}
app.UseCors(ServiceRegistrar.CorsPolicy);
app.UseHttpsRedirection();



app.UseExceptionHandler("/error");
app.MapHub<ChatHub>("/chat");


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();
