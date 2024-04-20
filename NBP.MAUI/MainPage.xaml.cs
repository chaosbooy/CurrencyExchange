using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace NBP.MAUI
{
    public partial class MainPage : ContentPage
    {
        private List<string> currencies;
        private RootObject allRates;
        List<string> pickersList;

        public MainPage()
        {
            pickersList = new List<string>();
            InitializeComponent();
            InitializeClass();
        }

        private async void InitializeClass()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:64195/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var goldResponse = await client.GetAsync("http://api.nbp.pl/api/cenyzlota");
            var currencyResponse = await client.GetAsync("http://api.nbp.pl/api/exchangerates/tables/a");

            if (currencyResponse.IsSuccessStatusCode)
            {
                string response = await currencyResponse.Content.ReadAsStringAsync();

                allRates = JsonConvert.DeserializeObject<List<RootObject>>(response)[0];

                allRates.rates.Add(new Rate
                {
                    code = "PLN",
                    currency = "złoty",
                    mid = 1,
                });
            }
            else
                await DisplayAlert("Retrieval error", $"Couldn't retrieve exchange rate data", "OK");

            if (goldResponse.IsSuccessStatusCode)
            {
                string response = await goldResponse.Content.ReadAsStringAsync();
                CenaZlota goldRate = JsonConvert.DeserializeObject<List<CenaZlota>>(response)[0];

                allRates.rates.Add(new Rate
                {
                    code = "GOLD",
                    currency = "światowe",
                    mid = goldRate.cena,
                });
            }
            else
                await DisplayAlert("Retrieval error", $"Couldn't retrieve gold data", "OK");

            foreach (var rate in allRates.rates)
                pickersList.Add(rate.code);

            picker1.ItemsSource = pickersList;
            picker2.ItemsSource = pickersList;

            picker1.SelectedItem = pickersList[pickersList.Count - 2];
            picker2.SelectedItem = pickersList[pickersList.Count - 1];

        }

        private void InputChanged(object sender, TextChangedEventArgs e)
        {
            var s = (Entry)sender;
            if (!s.IsFocused) return;

            bool isFirstEntry = s == entry1;

            double convertedValue = 0;
            string convertionString = (e.NewTextValue.Length > 0 && e.NewTextValue[e.NewTextValue.Length - 1] == ',') ? e.NewTextValue.Substring(0, e.NewTextValue.Length - 1) : e.NewTextValue;
            if(convertionString == "")
            {
                if (isFirstEntry)
                    entry2.Text = "";
                else
                    entry1.Text = "";
            }
            else if (!double.TryParse(convertionString, out convertedValue))
            {
                string oldValue = new string(convertionString.Where(char.IsDigit).ToArray());
                if (isFirstEntry) entry1.Text = oldValue;
                else entry2.Text = oldValue;

                return;
            }
            else if (e.NewTextValue.Count(c => c == ',') > 1)
            {
                string oldValue = e.NewTextValue.Substring(0, e.NewTextValue.Length - 1);
                if (isFirstEntry) entry1.Text = oldValue;
                else entry2.Text = oldValue;
            }

            double firstMid = allRates.rates.Find(x => x.code == picker1.SelectedItem).mid;
            double secondMid = allRates.rates.Find(x => x.code == picker2.SelectedItem).mid;
            double exchangedValue = convertedValue / (!isFirstEntry ? firstMid : secondMid) * (!isFirstEntry ? secondMid : firstMid);
            if (isFirstEntry)
                entry2.Text = $"{exchangedValue}";
            else
                entry1.Text = $"{exchangedValue}";
        }

        private void PickerIndexChanged(object sender, EventArgs e)
        {
            var s = (Picker)sender;
            bool isFirstPicker = s == picker1;

            if ((isFirstPicker && entry1.Text.Length == 0) || (!isFirstPicker && entry2.Text.Length == 0)) return;

            double firstMid = allRates.rates.Find(x => x.code == picker1.SelectedItem).mid;
            double secondMid = allRates.rates.Find(x => x.code == picker2.SelectedItem).mid;
            double convertedValue = 0;
            if (isFirstPicker)
                convertedValue = double.Parse((entry1.Text.Length > 0 && entry1.Text[entry1.Text.Length - 1] == ',') ? entry1.Text.Substring(0, entry1.Text.Length - 1) : entry1.Text);
            else
                convertedValue = double.Parse((entry2.Text.Length > 0 && entry2.Text[entry1.Text.Length - 1] == ',') ? entry2.Text.Substring(0, entry2.Text.Length - 1) : entry2.Text);

            double exchangedValue = convertedValue / (!isFirstPicker ? firstMid : secondMid) * (!isFirstPicker ? secondMid : firstMid);
            if (isFirstPicker)
                entry2.Text = $"{exchangedValue}";
            else
                entry1.Text = $"{exchangedValue}";


        }
    }
    public class Rate
    {
        public string currency { get; set; }
        public string code { get; set; }
        public double mid { get; set; }

        public Rate()
        {
            currency = "";
            code = "";
            mid = 0;
        }
    }
    public class CenaZlota
    {
        public string data { get; set; }
        public float cena { get; set; }

        CenaZlota()
        {
            data = "";
            cena = 0;
        }
    }

    public class RootObject
    {
        public string table { get; set; }
        public string no { get; set; }
        public DateTime effectiveDate { get; set; }
        public List<Rate> rates { get; set; }

        public RootObject()
        {
            table = "";
            no = "";
            effectiveDate = new();
            rates = new();
        }
    }

}
