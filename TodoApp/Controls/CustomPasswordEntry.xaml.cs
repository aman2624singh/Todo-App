namespace TodoApp.Controls;

public partial class CustomPasswordEntry : ContentView
{
    public static readonly BindableProperty TogglePasswordVisibilityCommandProperty =
            BindableProperty.Create(nameof(TogglePasswordVisibilityCommand), typeof(CommunityToolkit.Mvvm.Input.RelayCommand), typeof(CustomPasswordEntry));

    public static readonly BindableProperty PasswordProperty =
            BindableProperty.Create(nameof(Password), typeof(string), typeof(CustomPasswordEntry), null);

    public static readonly BindableProperty PasswordVisibilityIconProperty =
           BindableProperty.Create(nameof(PasswordVisibilityIcon), typeof(string), typeof(CustomPasswordEntry), null);

    public static readonly BindableProperty IsPasswordProperty =
            BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(CustomPasswordEntry), null);

    public CommunityToolkit.Mvvm.Input.RelayCommand TogglePasswordVisibilityCommand
    {
        get => (CommunityToolkit.Mvvm.Input.RelayCommand)GetValue(TogglePasswordVisibilityCommandProperty);
        set => SetValue(TogglePasswordVisibilityCommandProperty, value);
    }

    public string Password
    {
        get { return (string)GetValue(PasswordProperty); }
        set { SetValue(PasswordProperty, value); }
    }

    public string PasswordVisibilityIcon
    {
        get { return (string)GetValue(PasswordVisibilityIconProperty); }
        set { SetValue(PasswordVisibilityIconProperty, value); }
    }

    public bool IsPassword
    {
        get { return (bool)GetValue(IsPasswordProperty); }
        set { SetValue(IsPasswordProperty, value); }
    }   

    public CustomPasswordEntry()
    {
        InitializeComponent();
    }
}