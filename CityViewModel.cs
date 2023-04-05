using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Platform;

namespace WeatherApp1
{
    internal class CityViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public City city;
        public List<City> data;
        public Command LoadDataCommand { protected set; get; }
        public Command LoadNewCityCommand { protected set; get; }
        public Command EntryCityChanged { protected set; get; }
        public CityViewModel()
        {
            city = new City();
            this.LoadDataCommand = new Command(LoadData);
            this.LoadNewCityCommand = new Command(LoadNewCity);
        }

        public string Name
        {
            get { return city.Name; }
            set
            {
                city.Name = value;
                OnPropertyChanged("Name");
            }
        }
        public double Latitude
        {
            get { return city.Latitude; }
            set
            {
                city.Latitude = value;
                OnPropertyChanged("Latitude");
            }
        }
        public double Longitude
        {
            get { return city.Longitude; }
            set
            {
                city.Longitude = value;
                OnPropertyChanged("Longitude");
            }
        }
        public string Time
        {
            get { return city.Time; }
            set
            {
                city.Time = value;
                OnPropertyChanged("Time");
            }
        }
        public double Temperature
        {
            get { return city.Temperature; }
            set
            {
                city.Temperature = value;
                OnPropertyChanged("Temperature");
            }
        }
        public double Windspeed
        {
            get { return city.Windspeed; }
            set
            {
                city.Windspeed = value;
                OnPropertyChanged("Windspeed");
            }
        }

        private async void LoadData()
        {
            string lat = this.Latitude.ToString().Replace(",", ".");
            string lon = this.Longitude.ToString().Replace(",", ".");
            string url = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current_weather=true";
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(url);
                var response = await client.GetAsync(client.BaseAddress);
                response.EnsureSuccessStatusCode();// выброc исключения, eсли произошла ошибка
                
                // десериализация ответа в формате json
                var content = await response.Content.ReadAsStringAsync();
                JObject o = JObject.Parse(content);
                var str = o.SelectToken(@"$.current_weather");
                var current_weatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(str.ToString());
                
                this.Time = current_weatherInfo.Time;
                this.Temperature = current_weatherInfo.Temperature;
                this.Windspeed = current_weatherInfo.Windspeed;
            }
            catch (Exception ex)
            {
                
            }
        }

        private async void LoadNewCity()
        {
            string name = this.Name;
            if (!string.IsNullOrEmpty(name))
            {
                string url = $" https://geocoding-api.open-meteo.com/v1/search?name={name}&language=ru";
                try
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(url);
                    var response = await client.GetAsync(client.BaseAddress);
                    response.EnsureSuccessStatusCode();// выброс исключения, если произошла ошибка
                                                       // десериализация ответа в формате json
                    var content = await response.Content.ReadAsStringAsync();
                    JObject o = JObject.Parse(content);
                    var str = o.SelectToken(@"$.results[0]");
                    var current_cityInfo = JsonConvert.DeserializeObject<City>(str.ToString());
                    // Добавим в хранилице найденый город (низвание, широту, долготу)
                    List<City> cities;
                    City city1 = new City();
                    city1.Name = current_cityInfo.Name;
                    city1.Latitude = current_cityInfo.Latitude;
                    city1.Longitude = current_cityInfo.Longitude;
                    //cities = new List<City> { city1, city2, city3 };
                    LoadCities();
                    data.Add(city1);
                    string s = JsonConvert.SerializeObject(data);
                    _ = SecureStorage.SetAsync("City", s); // key: City - под ним храниться список словарей
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public async void LoadCities()
        {
            // Читаем города из хранилища
            string json = await SecureStorage.GetAsync("City");
            if (!string.IsNullOrEmpty(json))
                data = JsonConvert.DeserializeObject<List<City>>(json);
        }


        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
