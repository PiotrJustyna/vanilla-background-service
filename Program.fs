module VanillaBackgorundService.App

open System
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Serilog
open Serilog.Exceptions
open Elastic.CommonSchema.Serilog

let configureLogging () =
  let environment = Environment.GetEnvironmentVariable("ENVIRONMENT");

  let configurationBuilder = new ConfigurationBuilder()

  let configuration =
    configurationBuilder
      .AddJsonFile("appsettings.json", false, true)
      .AddJsonFile($"appsettings.{environment}.json", true, true)
      .Build()

  let loggerEnrichmentConfiguration = new LoggerConfiguration()

  Log.Logger <-
    loggerEnrichmentConfiguration
      .Enrich.FromLogContext()
      .Enrich.WithExceptionDetails()
      .Enrich.WithProperty("Environment", environment)
      .WriteTo.Console()
      .WriteTo.File(
        new EcsTextFormatter(),
        "./logs/log.json",
        ?rollingInterval = Some RollingInterval.Minute,
        ?retainedFileCountLimit = Some 3)
      .ReadFrom.Configuration(configuration)
      .CreateLogger()

  ()

[<EntryPoint>]
let main argv =
  let hostBuilder = Host.CreateDefaultBuilder(argv)

  configureLogging ()

  let host =
    hostBuilder
      .ConfigureServices(fun hostContext services -> 
        services.AddHostedService<SampleFSharpWorker.Workers.Worker>() |> ignore
      )
      .UseSerilog()
      .UseWindowsService()
      .Build()
      .Run()
  
  0