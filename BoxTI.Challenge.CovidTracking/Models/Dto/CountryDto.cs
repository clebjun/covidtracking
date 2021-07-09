using Newtonsoft.Json;
using System;

namespace BoxTI.Challenge.CovidTracking.API.Models.Dto
{
    public class CountryDto
    {
        [JsonProperty(PropertyName = "Active Cases_text")]
        public string ActiveCases { get; set; }

        [JsonProperty(PropertyName = "Country_text")]
        public string CountryName { get; set; }

        [JsonProperty(PropertyName = "Last Update")]
        public string LastUpdate { get; set; }

        [JsonProperty(PropertyName = "New Cases_text")]
        public string NewCases { get; set; }

        [JsonProperty(PropertyName = "New Deaths_text")]
        public string NewDeaths { get; set; }

        [JsonProperty(PropertyName = "Total Cases_text")]
        public string TotalCases { get; set; }

        [JsonProperty(PropertyName = "Total Deaths_text")]
        public string TotalDeaths { get; set; }

        [JsonProperty("Total Recovered_text")]
        public string TotalRecovered { get; set; }
    }
}

