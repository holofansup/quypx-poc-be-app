namespace cirrus.Models
{
    public class WeatherData
    {
        public Coord Coord { get; set; }
        public Weather[] Weather { get; set; }
        public string Base { get; set; }
        public Main Main { get; set; }
        public int Visibility { get; set; }
        public Wind Wind { get; set; }
        public Clouds Clouds { get; set; }
        public int Dt { get; set; }
        public Sys Sys { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Cod { get; set; }
    }
    public class Coord
    {
        public double Lon { get; set; }
        public double Lat { get; set; }
    }

    public class Weather
    {
        public int Id { get; set; }
        public string Main { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }

    public class Main
    {
        public double Temp { get; set; }
        public double Feels_like { get; set; }
        public double Temp_min { get; set; }
        public double Temp_max { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
        public int Sea_level { get; set; }
        public int Grnd_level { get; set; }
    }

    public class Wind
    {
        public double Speed { get; set; }
        public int Deg { get; set; }
    }

    public class Clouds
    {
        public int All { get; set; }
    }

    public class Sys
    {
        public int Type { get; set; }
        public int Id { get; set; }
        public string Country { get; set; }
        public int Sunrise { get; set; }
        public int Sunset { get; set; }
    }
}


//{ "coord":{ "lon":-0.1257,"lat":51.5085},"weather":[{ "id":803,"main":"Clouds","description":"broken clouds","icon":"04n"}],"base":"stations","main":{ "temp":57.61,"feels_like":57.34,"temp_min":56.05,"temp_max":59,"pressure":1016,"humidity":91,"sea_level":1016,"grnd_level":1011},"visibility":10000,"wind":{ "speed":2.3,"deg":0},"clouds":{ "all":75},"dt":1729877977,"sys":{ "type":2,"id":2075535,"country":"GB","sunrise":1729838526,"sunset":1729874803},"timezone":3600,"id":2643743,"name":"London","cod":200}