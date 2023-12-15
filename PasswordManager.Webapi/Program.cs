using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PasswordManager.Mongo;

var builder = WebApplication.CreateBuilder(args);

// Configure your services here
builder.Services.AddSingleton<PasswordManagerMongoContext>(sp =>
    PasswordManagerMongoContext.FromConnectionString("mongodb://localhost:27017", logging: true)
);


builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    );
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Resolve the PasswordManagerMongoContext from the service provider
var mongoContext = app.Services.GetRequiredService<PasswordManagerMongoContext>();

// Perform seeding if necessary
mongoContext.Seed(); // This will execute the seed method

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(); // Apply the default CORS policy
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
