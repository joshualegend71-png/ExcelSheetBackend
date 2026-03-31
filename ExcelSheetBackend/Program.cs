using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

// Swagger (for testing)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Bank Statement API",
        Version = "v1"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
///{
app.UseDeveloperExceptionPage(); // 👈 ADD THIS
app.UseSwagger();
app.UseSwaggerUI();

//}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
