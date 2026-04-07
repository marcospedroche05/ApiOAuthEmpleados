// See https://aka.ms/new-console-template for more information
using ClienteApiOAuthEmpleados;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
Console.WriteLine("Hello, World!");

//PARA CREAR METODOS EN PROGRAM, DEBEMOS HACERLO STATIC
static async Task<string> GetTokenAsync(string user, string pass)
{
    string urlApi = "https://apioauthempleadosmarcos-c9fgbdh0bdbja8cj.francecentral-01.azurewebsites.net/";
    LoginModel model = new LoginModel
    {
        UserName = user,
        Password = pass
    };
    using (HttpClient client = new HttpClient())
    {
        string request = "api/auth/login";
        client.BaseAddress = new Uri(urlApi);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new
            MediaTypeWithQualityHeaderValue("application/json"));
        string json = JsonConvert.SerializeObject(model);
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync(request, content);
        if(response.IsSuccessStatusCode == true)
        {
            string data = await response.Content.ReadAsStringAsync();
            JObject objeto = JObject.Parse(data);
            string token = objeto.GetValue("response").ToString();
            return token;
        } else
        {
            return "Peticion incorrecta: " + response.StatusCode;
        }
    }
}
