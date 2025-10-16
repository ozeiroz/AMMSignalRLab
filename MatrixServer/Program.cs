using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Настройка CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(_ => true);
    });
});

// SignalR
builder.Services.AddSignalR();

var app = builder.Build();

app.UseRouting();
app.UseCors();
app.UseWebSockets();

// Тестовый эндпоинт
app.MapGet("/test", () => "Server is working!");

// Hub
app.MapHub<MatrixHub>("/matrixHub");

Console.WriteLine("Server started on http://localhost:5044");
await app.RunAsync("http://localhost:5044");

// Hub для умножения матриц
public class MatrixHub : Hub
{
    public Task<string> MultiplyMatrices(string jsonA, string jsonB)
    {
        var A = JsonSerializer.Deserialize<double[][]>(jsonA);
        var B = JsonSerializer.Deserialize<double[][]>(jsonB);

        if (A[0].Length != B.Length)
            throw new HubException("Матрицы несовместимы");

        int n = A.Length, p = A[0].Length, m = B[0].Length;
        double[][] res = new double[n][];
        for (int i = 0; i < n; i++)
        {
            res[i] = new double[m];
            for (int j = 0; j < m; j++)
            {
                double sum = 0;
                for (int k = 0; k < p; k++)
                    sum += A[i][k] * B[k][j];
                res[i][j] = sum;
            }
        }

        return Task.FromResult(JsonSerializer.Serialize(res));
    }
}
