using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;

double[][] ReadMatrix(string path) => File.ReadAllLines(path)
    .Select(l => l.Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(double.Parse).ToArray())
    .ToArray();

Console.WriteLine("Введите путь к matrix1.csv:");
var path1 = Console.ReadLine()?.Trim('"') ?? "";
Console.WriteLine("Введите путь к matrix2.csv:");
var path2 = Console.ReadLine()?.Trim('"') ?? "";

var m1 = ReadMatrix(path1);
var m2 = ReadMatrix(path2);

// Подключение к SignalR
var connection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5044/matrixHub")
    .WithAutomaticReconnect()
    .Build();

await connection.StartAsync();
Console.WriteLine("Подключено к серверу SignalR!");

// Отправка матриц на сервер
var resultJson = await connection.InvokeAsync<string>(
    "MultiplyMatrices",
    JsonSerializer.Serialize(m1),
    JsonSerializer.Serialize(m2)
);

var result = JsonSerializer.Deserialize<double[][]>(resultJson);

// Сохранение результата
File.WriteAllLines("result.csv", result.Select(r => string.Join(",", r)));
Console.WriteLine("Результат сохранён в result.csv");
