﻿using Autofac;
using Autofac.Builder;
using Autofac.Extensions.DependencyInjection;
using Spacetime.Core;
using Spacetime.Core.gRPC;
using Spacetime.Core.Services;
using Spacetime.Core.Infrastructure;
using Spacetime.Helpers;
using MudBlazor.Services;
using Serilog;
using Fluxor;
using Spacetime.Themes;
using Spacetime.Core.Formatters;
using Spacetime.Container;
using Spacetime.Core.gRPC.Dynamic;

namespace Spacetime;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // set up logging with Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
          .Enrich.FromLogContext()
          .WriteTo.Debug()
          .CreateLogger();

        builder.Services.AddLogging(options =>
        {
            options.AddSerilog(dispose: true);
        });

        // add maui/hybrid dependencies
        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Services.AddBlazorWebView();
        
        // add third party libraries
        var assembly = typeof(MauiProgram).Assembly;
        builder.Services.AddFluxor(options => options.ScanAssemblies(assembly));
        builder.Services.AddMudServices();

        // register http clients
        builder.Services.AddHttpClient<ISpacetimeService, SpacetimeRestService>();

        // use Autofac integration
        builder.ConfigureContainer(new AutofacServiceProviderFactory(ConfigureContainer));

        return builder.Build();
    }

    private static void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<RequestService>();
        builder.RegisterType<SettingsService>();
        builder.RegisterType<SpacetimeRestService>().As<ISpacetimeService>();
        builder.RegisterType<UrlBuilder>();
        builder.RegisterType<DefaultTheme>();
        builder.RegisterType<ScriptUtils>();
        builder.RegisterType<GrpcExplorer>().As<IGrpcExplorer>();
        builder.RegisterType<DynamicGrpcFactory>().As<IDynamicGrpcFactory>();
        builder.RegisterType<LiteDbProtobufService>().As<IProtobufService>();

        builder.RegisterType<FormatterFactory>().As<IFormatterFactory>();
        builder.RegisterType<EmptyFormatter>().Keyed<IFormatter>(FormatterType.Default);
        builder.RegisterType<JsonFormatter>().Keyed<IFormatter>(FormatterType.Json);
    }
}