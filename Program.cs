using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AnalyseSentiment.Services;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<SentimentService>();  // Register the SentimentService
builder.Services.AddControllers();  // Register MVC controllers

var app = builder.Build();

// Serve static files (HTML, CSS, JS)
app.UseStaticFiles();  // Enable static files in wwwroot folder

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.MapControllers();  // Map the controllers for the API

app.Run();
