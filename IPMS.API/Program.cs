using IPMS.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
IConfiguration config = builder.Configuration;
// Add services to the container
builder.ConfigureSerilogLogger();
builder.Services.AddControllers();
builder.Services.AddApplicationServices(config);
builder.Services.AddAuthenticationServices(config);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

app.UseSwaggerDocumentation();
app.UseCors(x => x
               .AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();

});

await app.RunAsync();