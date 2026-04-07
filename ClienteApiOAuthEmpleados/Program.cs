// See https://aka.ms/new-console-template for more information
using ClienteApiOAuthEmpleados;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

Console.WriteLine("Cliente Api OAuth");
Console.WriteLine("Introduzca apellido");
string apellido = Console.ReadLine();
Console.WriteLine("Id del empleado (Password)");
string password = Console.ReadLine();
string response = await GetTokenAsync(apellido, password);
Console.WriteLine(response);
Console.WriteLine("------------------------------");
Console.WriteLine("Introduzca ID del empleado a buscar");
string idEmpleado = Console.ReadLine();
string datos = await FindEmpleadoAsync(int.Parse(idEmpleado), response);
Console.WriteLine(datos);


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

static async Task<string> FindEmpleadoAsync(int idEmpleado, string token)
{
    string urlApi = "https://apioauthempleadosmarcos-c9fgbdh0bdbja8cj.francecentral-01.azurewebsites.net/";
    using (HttpClient client = new HttpClient())
    {
        string request = "api/empleados/" + idEmpleado;
        client.BaseAddress = new Uri(urlApi);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //LO UNICO DE NOVEDAD ES INCLUIR authorization con bearer TOKEN DENTRO DEL HEADER
        client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
        HttpResponseMessage response = await client.GetAsync(request);
        if(response.IsSuccessStatusCode == true)
        {
            string data = await response.Content.ReadAsStringAsync();
            return data;
        } else
        {
            return "Error de algo: " + response.StatusCode;
        }

    }
}
