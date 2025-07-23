
namespace APIGerenciamento.Logging
{
    public class CustomerLogger : ILogger
    {
        private readonly string loggername;
        private readonly CustomLoggerProviderConfiguration loggerconfig;

        public CustomerLogger(string name, CustomLoggerProviderConfiguration config)
        {
            loggername = name;
            loggerconfig = config;
        }
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= loggerconfig.LogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception?
            exception, Func<TState, Exception?, string> formatter)
        {
            string message = $"{logLevel.ToString()} : {eventId.Id} - {formatter(state, exception)}";

            EscreverTextoNoArquivo(message);
        }

        private void EscreverTextoNoArquivo(string message)
        {
            string caminho = @"C:\Users\CSM\Desktop\CURSOS\ASP NET CORE\LOGGING\Eventos.txt";

            using (StreamWriter sw = new StreamWriter(caminho, true))
            {
                try
                {
                   sw.WriteLine(message);
                    sw.Close();
                }
                catch (Exception)
                {

                    throw;
                }
               
            }
        }
    }
}
