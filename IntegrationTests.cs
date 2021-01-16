using SpendingsApi;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using SpendingsApi.Models;
using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;




namespace SpendingsTests
{
    public class IntegrationTests : IClassFixture<TestFixture<Startup>>
    {

        private HttpClient Client;
        private string requestUrl = "/api/Spendings";

        public IntegrationTests(TestFixture<Startup> fixture)
        {
            Client = fixture.Client;
        }

        [Fact]
        public async Task TestGetAllAsync()
        {
            // Act
            var response = await Client.GetAsync(requestUrl);
            string jsonString = response.Content.ReadAsStringAsync().Result;
            var act = JsonConvert.DeserializeObject<List<Spendings>>(jsonString);

            // Assert
            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(act.Count > 0);
        }
        [Fact]
        public async Task TestGetAsync()
        {
            // Act
            var response = await Client.GetAsync(requestUrl + "/1");

            string jsonString = response.Content.ReadAsStringAsync().Result;
            var act = JsonConvert.DeserializeObject<Spendings>(jsonString);

            // Assert
            Assert.Equal(1, act.idSpendings);
            Assert.Equal(4523, act.Price);
            Assert.Equal(4, act.CarID);
            Assert.Equal(5, act.CostID);
            Assert.Equal(new DateTime(2021,3,5), act.Date);
        }
        [Fact]
        public async Task TestPostItem()
        {
            // Dodanie nowego wiersza
            var requestBody = new
            {
                Url = requestUrl,
                Body = new Spendings
                {
                    idSpendings = 15,
                    Date = new DateTime(2021, 3, 5),
                    CarID = 4,
                    CostID = 5,
                    Price = 4523
                }           
            };

            // Act
            var response = await Client.PostAsync(requestBody.Url, ContentHelper.GetStringContent(requestBody.Body));

            var response2 = await Client.GetAsync(requestUrl + "/15");
            string jsonString = response2.Content.ReadAsStringAsync().Result;
            var act = JsonConvert.DeserializeObject<Spendings>(jsonString);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal(requestBody.Body.idSpendings, act.idSpendings);
            Assert.Equal(requestBody.Body.Price, act.Price);
            Assert.Equal(requestBody.Body.CarID, act.CarID);
            Assert.Equal(requestBody.Body.CostID, act.CostID);
            Assert.Equal(requestBody.Body.Date, act.Date);
        }
        [Fact]
        public async Task TestPutItemAsync()
        {
            var requestBody = new
            {
                Url = requestUrl + "/15",
                Body = new 
                {
                    idSpendings = 15,
                    Date = new DateTime(2015, 5, 5),
                    CarID = 4,
                    CostID = 5,
                    Price = 6655
                }
            };

            // Act
            var response = await Client.PutAsync(requestBody.Url, ContentHelper.GetStringContent(requestBody.Body));

            var response2 = await Client.GetAsync(requestUrl + "/15");
            string jsonString = response2.Content.ReadAsStringAsync().Result;
            var act = JsonConvert.DeserializeObject<Spendings>(jsonString);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal(requestBody.Body.idSpendings, act.idSpendings);
            Assert.Equal(requestBody.Body.Price, act.Price);
            Assert.Equal(requestBody.Body.CarID, act.CarID);
            Assert.Equal(requestBody.Body.CostID, act.CostID);
            Assert.Equal(requestBody.Body.Date, act.Date);
        }

        [Fact]
        public async Task TestDeleteItemAsync()
        {
            var response = await Client.DeleteAsync(requestUrl + "/15");

            // Assert
            response.EnsureSuccessStatusCode();
            var response2 = await Client.GetAsync(requestUrl + "/15");
            //USTAWIC SPRAWDZANIE
            //  Assert.False(singleResponse.Id);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response2.IsSuccessStatusCode);
        }
        [Fact]
        public async Task Return_404_Result()
        {
            // Act
            var response = await Client.GetAsync(String.Empty);

            // Assert
            //response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task Return_400_Result()
        {
            // Act
            var response = await Client.DeleteAsync(requestUrl + "/{id_url}");

            // Assert
            //response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Fact]
        public async Task Return_500_Result()
        {
            // Act
            var response = await Client.DeleteAsync(requestUrl + "/0");

            // Assert
            //response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }
    }
}
