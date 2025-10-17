using ClinicApp.Services;
using ClinicApp.ViewModels;
using System.Windows;

namespace ClinicApp.Views
{
    public partial class StaffWindow : Window
    {
        private readonly StaffViewModel _viewModel;

        public StaffWindow()
        {
            InitializeComponent();

            // Inicijalizacija servisa
            var staffService = new StaffService(App.DbContext!);

            // Inicijalizacija ViewModel-a
            _viewModel = new StaffViewModel(staffService);
            DataContext = _viewModel;

            // Učitavanje podataka
            Loaded += async (s, e) => await _viewModel.LoadDataAsync();
        }
    }
}