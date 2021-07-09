using BoxTI.Challenge.CovidTracking.API.Models;
using BoxTI.Challenge.CovidTracking.API.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using BoxTI.Challenge.CovidTracking.API.Service.Impl;
using Microsoft.AspNetCore.Authorization;

namespace BoxTI.Challenge.CovidTracking.API.Controllers
{
    [Authorize]
    public class CountryController : Controller
    {
        private readonly Db_Context _ctt;
        private readonly string BaseUrl = "https://covid-19-tracking.p.rapidapi.com/v1";
        private readonly IMapper _map;

        public CountryController(Db_Context context, IMapper mapper)
        {
            _ctt = context;
            _map = mapper;
        }

        [HttpGet("NumeroCovidPorPais")]
        public async Task<CountryDto> NumeroCovidPorPais([Required] string countryName)
        {
            CountryDto Country = new CountryDto();
            string Uri;

            using (var client = new HttpClient())
            {
                HttpRequestMessage request;
                ExecutarRequest(countryName, out Uri, client, out request);

                HttpResponseMessage responseMessage = await client.SendAsync(request);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var Content = responseMessage.Content.ReadAsStringAsync().Result;
                    Country = JsonConvert.DeserializeObject<CountryDto>(Content);
                }

                return Country;
            }
        }

        [HttpGet("NumeroCovidTodosPaises")]
        public async Task<List<CountryDto>> NumeroCovidTodosPaises()
        {
            List<CountryDto> CountryList = new List<CountryDto>();
            string Uri;

            using (var client = new HttpClient())
            {
                HttpRequestMessage request;
                ExecutarRequest("", out Uri, client, out request);

                HttpResponseMessage responseMessage = await client.SendAsync(request);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var Content = responseMessage.Content.ReadAsStringAsync().Result;
                    CountryList = JsonConvert.DeserializeObject<List<CountryDto>>(Content);
                }

                return CountryList;
            }
        }
        private void ExecutarRequest(string countryName, out string Uri, HttpClient client, out HttpRequestMessage request)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request = new HttpRequestMessage();

            if (countryName == "")
                Uri = BaseUrl;
            else
                Uri = BaseUrl + "/" + countryName;

            request.RequestUri = new Uri(Uri);
            request.Method = HttpMethod.Get;
            request.Headers.Add("x-rapidapi-key", "27a8a51e73mshb56ce7441cf679cp1cc3b1jsn492bbef14582");
            request.Headers.Add("x-rapidapi-host", "covid-19-tracking.p.rapidapi.com");
        }

        [HttpGet("DadosCovidPaisesEspecificos")]
        public List<CountryDto> DadosCovidPaisesEspecificos()
        {
            List<CountryDto> CountryList = new List<CountryDto>();
            
            List<string> lista = new List<string>()
            { 
                new string ("Brazil"),
                new string ("Japan"),
                new string ("Netherlands"),
                new string ("Nigeria),"),
                new string ("Australia")
            };
            
            foreach (string item in lista)
            {
                CountryList.Add(NumeroCovidPorPais(item).Result);
            }

            return CountryList;
        }

        [HttpPost("SalvarDados")]
        public async Task<IActionResult> Create()
        {
            Country country;
            List<CountryDto> listCountryDto = await NumeroCovidTodosPaises();
            
            if (listCountryDto.Count == 0)
                return BadRequest("Nenhum registro encontrado.");

            try
            {
                foreach (CountryDto item in listCountryDto)
                {
                    if (item.ActiveCases == "N/A") item.ActiveCases = "0";
                    if (item.NewCases == "N/A") item.NewCases = "0";
                    if (item.NewDeaths == "N/A") item.NewDeaths = "0";
                    if (item.TotalCases == "N/A") item.TotalCases = "0";
                    if (item.TotalDeaths == "N/A") item.TotalDeaths = "0";
                    if (item.TotalRecovered == "N/A") item.TotalRecovered = "0";

                    item.ActiveCases = string.IsNullOrWhiteSpace(item.ActiveCases) ? "0" : item.ActiveCases.Replace(",", "");
                    item.CountryName = string.IsNullOrWhiteSpace(item.CountryName) ? "" : item.CountryName;
                    item.NewCases = string.IsNullOrWhiteSpace(item.NewCases) ? "0" : item.NewCases.Replace(",", "");
                    item.NewDeaths = string.IsNullOrWhiteSpace(item.NewDeaths) ? "0" : item.NewDeaths.Replace(",", "");
                    item.TotalCases = string.IsNullOrWhiteSpace(item.TotalCases) ? "0" : item.TotalCases.Replace(",", "");
                    item.TotalDeaths = string.IsNullOrWhiteSpace(item.TotalDeaths) ? "0" : item.TotalDeaths.Replace(",", "");
                    item.TotalRecovered = string.IsNullOrWhiteSpace(item.TotalRecovered) ? "0" : item.TotalRecovered.Replace(",", "");

                    country = _map.Map<Country>(item);
                    _ctt.Countries.Add(country);
                    _ctt.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException);
            }

            return Ok();
        }

        [HttpGet("ListarDadosTodos")]
        public List<CountryDto> ListarDadosTodos()
        {
            var result = _ctt.Countries.Select(x => _map.Map<CountryDto>(x)).ToList();
            return result;
        }

        [HttpGet("ListarDadosPaisEspecifico")]
        public List<CountryDto> ListarDadosPaisEspecifico([Required] string countryName)
        {
            var result =  _ctt.Countries
                            .Where(x => x.CountryName == countryName)
                            .Select(x => _map.Map<CountryDto>(x))
                            .ToList();

            return result;
        }

        [HttpPut("AtualizarDadosPais")]
        public async Task<IActionResult> AtualizarDadosPais(string activeCases,
                                                         [Required] string countryName,
                                                         DateTime lastUpdate,
                                                         string newCases,
                                                         string newDeaths,
                                                         string totalCases,
                                                         string totalDeaths,
                                                         string totalRecovered)
        {
            Country country = _ctt.Countries.FirstOrDefault(x => x.CountryName == countryName);
            
            if (country == null)
                return BadRequest("Nenhum registro encontrado para atualização dos dados do país.");

            try
            {
                CountryDto countryDto = new CountryDto()
                {
                    ActiveCases = activeCases,
                    CountryName = countryName,
                    LastUpdate = lastUpdate.ToString(),
                    NewCases = newCases,
                    NewDeaths = newDeaths,
                    TotalCases = totalCases,
                    TotalDeaths = totalDeaths,
                    TotalRecovered = totalRecovered
                };

                country = _map.Map(countryDto, country);
                await _ctt.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(countryName))
                    return NotFound();
                else
                    return StatusCode(500);
            }

            return Ok();
        }

        private bool CountryExists(string countryName)
        {
            return _ctt.Countries.Any(e => e.CountryName == countryName);
        }

        [HttpPut("ExcluirPais")]
        public async Task<IActionResult> ExcluirPais ([Required] string countryName)
        {
            Country country = _ctt.Countries.FirstOrDefault(x => x.CountryName == countryName);

            if (country == null)
                return BadRequest("Nenhum registro encontrado para exclusão lógica do país.");

            try
            {
                country.IsDeleted = true;
                await _ctt.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(countryName))
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode(500);
                }
            }

            return Ok();
        }

        [HttpGet("OrdenarPaises")]
        public object GetPaisesOrdenados()
        {
            var countries = _ctt.Countries.OrderByDescending(x => x.ActiveCases).ToList();
            return countries;
        }

        [HttpPost("GerarCSV")]
        public IActionResult GerarCSV([Required] string countryName,
                                      [Required] string path)
        {
            List<CountryDto> dados = ListarDadosPaisEspecifico(countryName);

            CountryService countryService = new CountryService();
            countryService.ExportarCSV(dados, path);

            return Ok();
        }
    }
}
