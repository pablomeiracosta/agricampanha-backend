using AutoMapper;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Auria.Structure.Configuration;

namespace Auria.Structure;

public class AuriaContext
{
    private static AuriaContext? _instance;
    private static readonly object _lock = new object();

    public IConfiguration Configuration { get; private set; }
    public AppSettings Settings { get; private set; }
    public ILogger Log { get; private set; }
    public IMapper? Mapper { get; set; }

    private AuriaContext(IConfiguration configuration)
    {
        Configuration = configuration;
        Settings = new AppSettings();
        Configuration.Bind(Settings);

        Log = ConfigureSerilog();
    }

    public static AuriaContext Initialize(IConfiguration configuration)
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new AuriaContext(configuration);
                }
            }
        }
        return _instance;
    }

    public static AuriaContext Instance
    {
        get
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("AuriaContext não foi inicializado. Chame Initialize() primeiro.");
            }
            return _instance;
        }
    }

    private ILogger ConfigureSerilog()
    {
        var logLevel = Enum.TryParse<LogEventLevel>(Settings.Serilog.MinimumLevel, out var level)
            ? level
            : LogEventLevel.Information;

        return new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .WriteTo.Console()
            .WriteTo.File(
                Settings.Serilog.LogPath,
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }

    public void ConfigureMapper(IMapper mapper)
    {
        Mapper = mapper;
    }
}
