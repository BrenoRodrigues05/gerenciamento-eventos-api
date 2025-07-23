using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace APIGerenciamento.Filters
{
    public class APILoggingFilter : IActionFilter
    {
        private readonly ILogger<APILoggingFilter> _logger;

        public APILoggingFilter(ILogger<APILoggingFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller.GetType().Name;
            var action = context.ActionDescriptor.DisplayName;

            _logger.LogInformation("➡️ Executando ação: {Action} no controller: {Controller}", action, controller);
            _logger.LogInformation(DateTime.Now.ToLongTimeString());

            foreach (var param in context.ActionArguments)
            {
                _logger.LogInformation("📦 Parâmetro: {Key} = {@Value}", param.Key, param.Value);
            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var controller = context.Controller.GetType().Name;
            var action = context.ActionDescriptor.DisplayName;

            if (context.Exception == null)
            {
                _logger.LogInformation("✅ Ação executada com sucesso: {Action} no controller: " +
                    "{Controller}", action, controller);
                _logger.LogInformation(DateTime.Now.ToLongTimeString());
            }
            else
            {
                _logger.LogError(context.Exception, "❌ Erro ao executar ação: {Action} no " +
                    "controller: {Controller}", action, controller);
            }
        }

        
    }
}
