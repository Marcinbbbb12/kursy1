using System.Net;
using System.Text.Json;

namespace kursy1
{
    public partial class MainPage : ContentPage
    {
        public class Currency
        {
            public string? table { get; set; }
            public string? currency { get; set; }
            public string? code { get; set; }
            public IList<Rate> rates { get; set; }
        }

        public class Rate
        {
            public string? no { get; set; }
            public string? effectiveDate { get; set; }
            public double? bid { get; set; }
            public double? ask { get; set; }
        }

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnGetRatesClicked(object sender, EventArgs e)
        {
            string date = dpData.Date.ToString("yyyy-MM-dd");

            GetExchangeRate("USD", this.usdRateLabel);
            GetExchangeRate("EUR", this.eurRateLabel);
            GetExchangeRate("GBP", this.gbpRateLabel);
        }

        private void GetExchangeRate(string currencyCode, Label rateLabel)
        {
            string url = $"https://api.nbp.pl/api/exchangerates/rates/c/{currencyCode}/" + dpData.Date.ToString("yyyy-MM-dd") + "/?format=json";

            string json;
            using (var webClient = new WebClient())
            {
                json = webClient.DownloadString(url);
            }

            Currency currency = JsonSerializer.Deserialize<Currency>(json);

            if (currency != null && currency.rates.Count > 0)
            {
                rateLabel.Text = $"{currency.currency}: Bid: {currency.rates[0].bid}, Ask: {currency.rates[0].ask}";
            }
            else
            {
                rateLabel.Text = $"{currencyCode}: Kurs niedostępny";
            }
        }
    }
}