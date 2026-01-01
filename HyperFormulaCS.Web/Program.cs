using HyperFormulaCS.Calculation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Engine as Singleton (shared state for demo)
builder.Services.AddSingleton<Engine>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseStaticFiles(); // Enable serving index.html from wwwroot

app.MapControllers();
app.MapFallbackToFile("index.html"); // Fallback to SPA

app.Run();
