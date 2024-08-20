using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Spoofer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()


        {
            InitializeComponent();
        }

        private async void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            string baseUrl = urlTextBox.Text;
            resultTextBlock.Text = "Поиск...";
            progressBar.Visibility = Visibility.Visible;

            await Task.Run(() => StartBruteForce(baseUrl));
            progressBar.Visibility = Visibility.Hidden;
        }

        private async Task StartBruteForce(string baseUrl)
        {
            var characters = "abcdefghiklmnopqrstuvwxyzABCDEFGHIKLMNOPQRSTUVWXYZ0123456789-"; 
            for (int length = 1; length <= 5; length++)  // Максимальная длина
            {
                foreach (var combination in GetCombinations(characters, length))
                {
                    string fullUrl = $"{baseUrl}/{combination}";

                    // Обновляем текстовое поле с текущей комбинацией
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        resultTextBlock.Text = $"Пробуем: {fullUrl}";
                    });

                    if (await CheckUrl(fullUrl))
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            resultTextBlock.Text = $"Сайт существует: {fullUrl}";
                        });
                        return;
                    }
                }
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                resultTextBlock.Text = "Сайт не найден.";
            });
        }

        private async Task<bool> CheckUrl(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(url);
                    return response.IsSuccessStatusCode;
                }
                catch
                {
                    return false;
                }
            }
        }

        private static IEnumerable<string> GetCombinations(string characters, int length)
        {
            var result = new List<string>();
            GenerateCombinations(characters, length, string.Empty, result);
            return result;
        }

        private static void GenerateCombinations(string characters, int length, string current, List<string> result)
        {
            if (current.Length == length)
            {
                result.Add(current);
                return;
            }

            foreach (char character in characters)
            {
                GenerateCombinations(characters, length, current + character, result);
            }
        }
    }
}
