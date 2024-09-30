using ImageAPI.Handlers;

namespace UserRegistration.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register services
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register WebSocketHandler and its dependencies
            builder.Services.AddSingleton<BroadcastService>();
            builder.Services.AddSingleton<SvgService>();
            builder.Services.AddSingleton<MessageProcessor>();
            builder.Services.AddSingleton<WebSocketHandler>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseCors(builder =>
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());

            app.MapControllers();
            app.UseWebSockets();

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();

                        var webSocketHandler = context.RequestServices.GetRequiredService<WebSocketHandler>();
                        await webSocketHandler.HandleWebSocketAsync(webSocket, context.RequestAborted);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
            });

            app.Run();
        }
    }
}
