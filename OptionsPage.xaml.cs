using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace WeatherApp1;

public partial class OptionsPage : ContentPage
{
    City MyCity;
    CityViewModel viewModel;
    public OptionsPage()
    {
        InitializeComponent();
    }

    private void OnCuttonClear(object sender, EventArgs e)
    {
        SecureStorage.RemoveAll();
        DisplayAlert("Уведомление", "Список городов очищен!", "ОK");
        List<City> cities;
        City city1 = new City();
        city1.Name = "Москва";
        city1.Latitude = 55.75;
        city1.Longitude = 37.62;
        City city2 = new City();
        city2.Name = "Санкт-Петербург";
        city2.Latitude = 59.94;
        city2.Longitude = 30.31;
        City city3 = new City();
        city3.Name = "Сочи";
        city3.Latitude = 43.60;
        city3.Longitude = 39.73;
        cities = new List<City> { city1, city2, city3 };
        string data = JsonConvert.SerializeObject(cities);
        SecureStorage.SetAsync("City", data); // key: City - под ним храниться список словарей
        DisplayAlert("Уведомление", "Добавлены 3 базовых города!", "ОK");
    }

    private void EntryCityChanged(object sender, TextChangedEventArgs e)
    {
        this.BindingContext = new CityViewModel
        {
            Name = EntryCity.Text.ToString(),
            Latitude = 0,
            Longitude = 0,
            Time = "",
            Temperature = 0,
            Windspeed = 0,
        };
    }
}
