using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BoxTI.Challenge.CovidTracking.API.Models;
using BoxTI.Challenge.CovidTracking.API.Models.Dto;

namespace BoxTI.Challenge.CovidTracking.API.Service.Impl
{
    public class CountryService
    {
        public void SalvarListaDeDados(Country country)
        {
            try
            {
                using (var db = new Db_Context())
                {
                    
                    db.Countries.Add(country);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void ExportarCSV(List<CountryDto> country, String path)
        {
            var csv = new StringBuilder();

            try
            {
                foreach (CountryDto item in country)
                {
                    csv.AppendLine(item.ActiveCases + ";" +
                                   item.CountryName + ";" +
                                   item.LastUpdate + ";" +
                                   item.NewCases + ";" +
                                   item.NewDeaths + ";" +
                                   item.TotalCases + ";" +
                                   item.TotalDeaths + ";" +
                                   item.TotalRecovered + ";");
                }

                File.WriteAllText(path + @"\dados_pais.csv", csv.ToString());
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
