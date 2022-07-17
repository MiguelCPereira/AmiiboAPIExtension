using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AmiiboAPIProject.Model
{
    public class Amiibo
    {
        [JsonProperty(PropertyName = "amiiboSeries")]
        public string AmiiboSeries { get; set; }
        [JsonProperty(PropertyName = "character")]
        public string Character { get; set; } // Some characters have multiple amiibos (for ex. Golden Mario and Mario) 
        [JsonProperty(PropertyName = "gameSeries")]
        public string Franchise { get; set; }
        [JsonProperty(PropertyName = "image")]
        public string ImageUrl { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "release")]
        public JObject ReleaseDatesJObj { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; } // If it's a figurine, or any other type of merchandise (yarn plushie, bands, etc)

        [JsonIgnore]
        public BitmapImage Image
        {
            get
            {
                // Check if the user has internet connection by pinging Google
                try
                {
                    if (new Ping().Send("www.google.com.mx").Status == IPStatus.Success)
                        return new BitmapImage(new Uri(ImageUrl));
                    else
                        return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/noAmiibo.png"));
                }
                catch
                {
                    return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/noAmiibo.png"));
                }
            }
            set { Image = value; }
        }

        [JsonIgnore]
        private List<string> _releaseDates;
        [JsonIgnore]
        public List<string> ReleaseDates
        {
            get
            {
                if (_releaseDates == null)
                {
                    _releaseDates = new List<string>();

                    foreach(var jToken in ReleaseDatesJObj)
                    {
                        if(jToken.Value.ToString() != "")
                            _releaseDates.Add(jToken.Key + ": " + jToken.Value);
                    }
                }

                return _releaseDates;
            }
            set
            {
                _releaseDates = value;
            }
        }
    }
}
