
function getWeatherIconUrl(iconCode) {
    return `https://openweathermap.org/img/wn/${iconCode}@2x.png`;
}

document.addEventListener('DOMContentLoaded', function () {
    const apiKey = '5be53b358afc85bae0bf938bb7715b96'; // API anahtarınız
    getWeatherData(40.7128, -74.0060, 'weather-new-york', 'New York');
    getWeatherData(41.0082, 28.9784, 'weather-istanbul', 'İstanbul');

    function translateWeatherCondition(condition) {
        const translationMap = {
            'Clear': 'Açık',
            'Clouds': 'Bulutlu',
            'Rain': 'Yağmurlu',
            'Snow': 'Karlı',
            'Mist': 'Sisli',
            'Smoke': 'Sisli',
            'Haze': 'Puslu',
            'Dust': 'Sisli',
            'Fog': 'Sisli',
            'Sand': 'Kum Fırtınası',
            'Ash': 'Sisli',
            'Squall': 'Fırtına',
            'Tornado': 'Kasırga',
            'Drizzle': 'Yağmurlu',
            'Thunderstorm': 'Fırtına',
            'Scattered clouds': 'Bulutlu',
            'Broken clouds': 'Bulutlu',
            'Shower rain': 'Yağmurlu',
            'Heavy intensity shower rain': 'Yağmurlu',
            'Light rain': 'Yağmurlu',
            'Moderate rain': 'Yağmurlu',
            'Heavy intensity rain': 'Yağmurlu',
            'Very heavy rain': 'Yağmurlu',
            'Extreme rain': 'Yağmurlu',
            'Freezing rain': 'Yağmurlu',
            'Light intensity drizzle': 'Yağmurlu',
            'Heavy intensity drizzle': 'Yağmurlu',
            'Light intensity drizzle rain': 'Yağmurlu',
            'Heavy intensity drizzle rain': 'Yağmurlu',
            'Shower drizzle': 'Yağmurlu'
            // Diğer olası hava durumu terimleri eklenebilir
        };
        return translationMap[condition] || condition;
    }

    function getWeatherData(lat, lon, elementId, city) {
        const url = `https://api.openweathermap.org/data/2.5/weather?lat=${lat}&lon=${lon}&appid=${apiKey}&units=metric&lang=tr`;

        fetch(url)

            .then(response => response.json())
            .then(data => {
                const translatedCondition = translateWeatherCondition(data.weather[0].main);
                const iconUrl = getWeatherIconUrl(data.weather[0].icon);
                const weatherInfo = `

    <ul class="text-center">
        <li class="text-center">
            <img width=25 src="${iconUrl}" alt="${data.weather[0].description}">
            <strong> ${city} </strong>
        </li>
        <li class="text-center">
            ${data.main.temp.toFixed(1)}°C
            ${translatedCondition}
        </li>
    </ul>


    `;
                document.getElementById(elementId).innerHTML = weatherInfo;
            })
            .catch(error => {
                console.error(`ulaşılamadı`, error);
                document.getElementById(elementId).innerHTML = 'yüklenemedi';
            });
    }
});


