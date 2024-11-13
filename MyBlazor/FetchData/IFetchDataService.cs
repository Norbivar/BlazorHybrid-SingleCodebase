using MyBlazor;
public interface IFetchDataService
{
    Task<WeatherForecast[]?> GetWeatherForecastsAsync();
}