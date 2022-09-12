using Application;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WebApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddApplication().AddInfrastructure(builder.Configuration);
builder.Services.AddSwagger();
var app = builder.Build();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.ConfigureSwag();
app.UseHttpsRedirection();
app.UseRouting();
app.HandleFluentValidationException();
app.HandleAuthorizationException();
app.UseEndpoints(x => x.MapControllers());
await app.RunMigrations();
await app.RunAsync();