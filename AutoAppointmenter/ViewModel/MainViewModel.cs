using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows.Threading;
using AutoAppointmenter.Model;
using Reactive.Bindings;

namespace AutoAppointmenter.ViewModel; 

internal class MainViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;

    public ReactiveProperty<string> Username { get; } = new("");
    public ReactiveProperty<string> Password { get; } = new("");

    public MainViewModel() {
        Init();
        
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
        timer.Start();
        timer.Tick += (s, args) =>
        {
            timer.Stop();
            AppointmenterScheduler.Add();
        };
    }

    private async void Init() {
        var settings = await Settings.LoadAsync();
        Username.Value = settings.Username;
        Password.Value = settings.Password;
        
        Observable.Merge(Username, Password)
            .Subscribe(async _ => await Settings.SaveAsync( new(Username.Value, Password.Value)));
    }
}