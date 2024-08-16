using AuthApi.Extensions;
using WebCore.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureDefault();
builder.Services.AddConfig(builder.Configuration);
// Add services to the container.
builder.Services.AddAuthentication()
    .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = "";
        options.ClientSecret = "";
    });
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddService();


var app = builder.Build();
await app.ConfigureDefault();
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();