using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeatherShow
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private async void buttonLoadWeather_Click(object sender, EventArgs e)
        {
            string selectedCity = listBoxCities.SelectedItem?.ToString();
            await GetWeatherAsync(selectedCity);
        }

        private async Task GetWeatherAsync(string city)
        {
            try
            {
                string[] parts = city.Split('\t');
                string cityName = parts[0];


                string[] coordinates = parts[1].Split(new string[] { ", " }, StringSplitOptions.None);

                string latitude = coordinates[0];
                string longitude = coordinates[1];

                string apiKey = "f85e73db0cafe86ef973b9b1c0749644";
                string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}";
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);


                    string responseBody = await response.Content.ReadAsStringAsync();

                    Weather weather = JsonConvert.DeserializeObject<Weather>(responseBody);
                    JObject jsonObject = JObject.Parse(responseBody);
                    weather.Country = (string)jsonObject["sys"]["country"];
                    weather.Name = (string)jsonObject["name"];
                    weather.Temp = (double)jsonObject["main"]["temp"];
                    weather.Description = (string)jsonObject["weather"][0]["description"];
                    textBoxName.Text = weather.Name;
                    textBoxCountry.Text = weather.Country;
                    textBoxTemperature.Text = weather.Temp.ToString();
                    textBoxDescription.Text = weather.Description;
                }

            }


            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            LoadCitiesFromFile();
        }


        private async void listBoxCities_SelectedIndexChanged(object sender, EventArgs e)
        {
            //doesn't need, the event starts only after pressing the button
        }




        private void LoadCitiesFromFile()
        {
            try
            {
                var cities = File.ReadAllLines(@"C:\Users\gkras\OneDrive\Рабочий стол\city.txt");
                foreach (var city in cities)
                {
                    listBoxCities.Items.Add(city);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке городов: {ex.Message}");
            }
        }

       
    }
}
