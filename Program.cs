using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace desafio
{
    class Desafio
    {
        static async Task Main(string[] args)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    string url = "https://sms.comtele.com.br/api/v2/send";
                    httpClient.DefaultRequestHeaders.Add("auth-key", "7888ffef-72f1-45ce-bd7f-82faf12d7fd6");
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string fileName = "lista.txt";
                    string aux = "auxiliar.txt";
                    File.Copy(fileName, aux, true);

                    using(StreamReader sr = new StreamReader(fileName))
                    using(StreamWriter sw = new StreamWriter(aux,append: true))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] splited = line.Split(";");
                            if (splited.Length >= 2)
                            {
                                string phone_number = splited[0];
                                string message = splited[1];
                                string payload = $"{{\"Sender\":\"3\",\"Receivers\":\"{phone_number}\",\"Content\":\"{message}\"}}";
                                HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");

                                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url))
                                {
                                    request.Content = content;
                                    HttpResponseMessage response = await httpClient.SendAsync(request);
                                    string responseContent = await response.Content.ReadAsStringAsync();

                                    Console.WriteLine($"Response: {responseContent}");

                                    string[] feedback = responseContent.Split(",");
                                    string status = feedback.Length >= 3 ? feedback[2] : "Unknown";
                                    sw.WriteLine("-Status-" + status);
                                }
                            }
                        }

                    }
                    File.Copy(aux, fileName, true);
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }
    }
}