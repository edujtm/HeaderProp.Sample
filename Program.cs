using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Not necessary for header propagation, it's here only to help debug
// the outgoing headers.
builder.Services.AddTransient<LoggingHandler>();

// Step 1 - Add Header Propagation Services
builder.Services.AddHeaderPropagation(options => {

  // Step 2 - Specify which headers should be propagated
  options.Headers.Add("Authorization");
});

// Step 3 - Add header propagation delegating handler
builder.Services
  .AddHttpClient("header-test")
  .AddHeaderPropagation()
  .AddHttpMessageHandler<LoggingHandler>(); // Not necessary, only for debugging outgoing headers

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Step 4 - Add Header Propagation Middleware
app.UseHeaderPropagation();

app.UseAuthorization();

app.MapControllers();

app.MapGet("propagate", async (IHttpClientFactory httpClientFactory) => {
  var client = httpClientFactory.CreateClient("header-test");

  var result = await client.GetAsync("http://www.google.com/");
  var content = await result.Content.ReadAsStringAsync();
  return Results.Ok(content);
});

app.Run();


// This class is not necessary, it only shows the outgoing headers 
// so that you can confirm that it's working correctly.
public class LoggingHandler : DelegatingHandler
{

  protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
  {
      Console.WriteLine("Outgoing Headers:");
      foreach (var header in request.Headers) {
        Console.WriteLine($"{header.Key} - {string.Join(", ", header.Value)}");
      }
      Console.WriteLine();

      HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
      return response;
  }
}