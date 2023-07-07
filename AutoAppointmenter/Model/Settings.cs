using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace AutoAppointmenter.Model; 

internal static class Settings {
    private static readonly string Path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
    public static Data Data { get; private set; } = new();
    
    public static async Task SaveAsync(Data data) {
        var json = JsonSerializer.Serialize(data);
        await File.WriteAllTextAsync("settings.json", json);
    }
    
    public static async Task<Data> LoadAsync() {
        if (!File.Exists(Path)) {
            await SaveAsync(new Data());
            return new Data();
        }
        
        var json = await File.ReadAllTextAsync(Path);
        Data = JsonSerializer.Deserialize<Data>(json) ?? new();
        return Data;
    }
}

public record Data(string Username = "", string Password = "");
