using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace kursy1
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnConvertClicked(object sender, EventArgs e)
        {
            var sourceCurrency = sourceCurrencyPicker.SelectedItem?.ToString();
            var targetCurrency = targetCurrencyPicker.SelectedItem?.ToString();

            if (double.TryParse(amountEntry.Text, out var amount))
            {
                if (sourceCurrency != null && targetCurrency != null && sourceCurrency != targetCurrency)
                {
                    var rate = await GetExchangeRate(sourceCurrency, targetCurrency);
                    if (rate != null)
                    {
                        var convertedAmount = amount * (double)rate;
                        resultLabel.Text = $"Wynik: {convertedAmount:F2} {targetCurrency}";
                    }
                    else
                    {
                        resultLabel.Text = "Nie udało się pobrać kursu.";
                    }
                }
                else
                {
                    resultLabel.Text = "Proszę wybrać różne waluty.";
                }
            }
            else
            {
                resultLabel.Text = "Proszę wprowadzić prawidłową kwotę.";
            }
        }

        private async Task<double?> GetExchangeRate(string sourceCurrency, string targetCurrency)
        {
            string urlSource = $"https://api.nbp.pl/api/exchangerates/rates/c/{sourceCurrency}/?format=json";
            string urlTarget = $"https://api.nbp.pl/api/exchangerates/rates/c/{targetCurrency}/?format=json";

            using (var client = new HttpClient())
            {
                try
                {
                    var responseSource = await client.GetStringAsync(urlSource);
                    var responseTarget = await client.GetStringAsync(urlTarget);

                    
                    Debug.WriteLine($"Odpowiedź z źródłowego API: {responseSource}");
                    Debug.WriteLine($"Odpowiedź z docelowego API: {responseTarget}");

                    var dataSource = JsonSerializer.Deserialize<ExchangeRateResponse>(responseSource);
                    var dataTarget = JsonSerializer.Deserialize<ExchangeRateResponse>(responseTarget);

                    if (dataSource?.Rates != null && dataSource.Rates.Length > 0 &&
                        dataTarget?.Rates != null && dataTarget.Rates.Length > 0)
                    {
                        return dataTarget.Rates[0].Ask / dataSource.Rates[0].Bid;
                    }
                    else
                    {
                        return null; 
                    }
                }
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine($"Błąd podczas przesyłania żądania: {ex.Message}");
                    return null;
                }
                catch (JsonException ex)
                {
                    Debug.WriteLine($"Błąd deserializacji JSON: {ex.Message}");
                    return null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Inny błąd: {ex.Message}");
                    return null;
                }
            }
        }

        public class ExchangeRateResponse
        {
            public string Currency { get; set; }
            public Rate[] Rates { get; set; }
        }

        public class Rate
        {
            public double Bid { get; set; }
            public double Ask { get; set; }
        }
    }
}