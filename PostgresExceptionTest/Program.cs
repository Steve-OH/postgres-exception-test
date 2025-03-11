namespace PostgresExceptionTest;

using Eventuous;
using Eventuous.Postgresql;
using Npgsql;

public class Program
{
    private const string Host = "localhost";
    private const string Port = "5432";
    private const string User = "aUser";
    private const string Password = "aPassword";
    private const string DatabaseName = "aDatabase";
    private const string SchemaName = "aSchema";

    private static readonly string ConnectionString = $"Server={Host};Port={Port};User ID={User};Password={Password};Database={DatabaseName}";

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var dataSource = new NpgsqlDataSourceBuilder(ConnectionString).Build();
        builder.Services.AddSingleton(dataSource);
        builder.Services.AddEventuousPostgres(ConnectionString, SchemaName, initializeDatabase: true);
        TypeMap.RegisterKnownEventTypes();
        builder.Services.AddEventStore<PostgresStore>();
        builder.Services.AddCommandService<CommandService, FooBar>();
        var app = builder.Build();

        var commandService = app.Services.GetRequiredService<ICommandService<FooBar>>();
        var result = await commandService.Handle(new SetFooBar(), CancellationToken.None);
        if (result.Success)
        {
            Console.WriteLine(@"Success");
            return;
        }

        Console.WriteLine(@$"Failure: {result.Exception}");
    }

    internal record FooBar : State<FooBar>;

    internal record SetFooBar;

    [EventType("V1.FooBarCreated")]
    internal record FooBarCreated;

    internal class CommandService : CommandService<FooBar>
    {
        public CommandService(IEventStore store)
            : base(store)
        {
            this.On<SetFooBar>().InState(ExpectedState.Any).GetStream(_ => new StreamName("fooBar::dummy")).Act(HandleSetFooBar);
        }

        private static IEnumerable<object> HandleSetFooBar(FooBar fooBar, object[] originalEvents, SetFooBar cmd)
        {
            yield return new FooBarCreated();
        }
    }
}
