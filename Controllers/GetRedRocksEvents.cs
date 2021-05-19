using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace red_rocks_web_scaper_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetRedRocksEvents : Controller
    {
        private readonly string _redRocksEventsUrl = "https://www.redrocksonline.com/events/#";
        [HttpGet]
        public async Task GetEvents()
        {
            var htmlDoc = await CallUrl(_redRocksEventsUrl);

            var eventCards = htmlDoc.DocumentNode.SelectNodes("//div")
                .Where(node => node.GetAttributeValue("class", "").Contains("card card-event event-month-active event-filter-active"));

            var events = new List<Event>();
            var year = 2021;
            for(var i = 1; i < 200; i++)
            {
                var nameNode = htmlDoc.DocumentNode
                    .SelectSingleNode($"//*[@id=\"event-grid\"]/div[2]/div[{i}]/div[2]/h3");
                var dateNode = htmlDoc.DocumentNode
                    .SelectSingleNode($"//*[@id=\"event-grid\"]/div[2]/div[{i}]/div[2]/div");
                var otherArtists = htmlDoc.DocumentNode
                    .SelectSingleNode($"//*[@id=\"event-grid\"]/div[2]/div[{i}]/div[2]/p");

                if (nameNode == null || dateNode == null)
                    break;

                var name = nameNode.InnerText.Replace("\n", "").Trim();
                if (name.Contains("Graduation") || name.Contains("Film On The Rocks") || name.Contains("Yoga"))
                    continue;

                if (otherArtists != null)
                {
                    name = name + " :: " + otherArtists.InnerText.Replace("\n", "").Trim();
                }
                DateTime date;
                try
                {
                    date = DateTime.ParseExact(dateNode.InnerText.Replace("\n", "").Trim() + $" {year}",
                        new string[] { "ddd, MMM d, h:mm tt yyyy", "ddd, MMM dd, h:mm tt yyyy" },
                        null);
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("was not recognized as a valid DateTime"))
                    {
                        year++;
                        date = DateTime.ParseExact(dateNode.InnerText.Replace("\n", "").Trim() + $" {year}",
                            new string[] { "ddd, MMM d, h:mm tt yyyy", "ddd, MMM dd, h:mm tt yyyy" },
                            null);
                    }
                    else
                    {
                        break;
                    }
                }
                var show = new Event()
                {
                    // Thu, May 20, 7:00 pm
                    // Fri, Apr 22, 5:00 pm
                    Date = date,
                    EventName = name
                };
                events.Add(show);

            }

            var json = JsonConvert.SerializeObject(events);
            System.IO.File.WriteAllText("events.json", json);
        }

        private async Task<HtmlDocument> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            var response = await client.GetStringAsync(fullUrl);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            return htmlDoc;
        }
    }
}
