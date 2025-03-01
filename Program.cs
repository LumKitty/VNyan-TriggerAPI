// See https://aka.ms/new-console-template for more information
using System.Text;
using CommandLine;
using Newtonsoft.Json.Linq;

namespace VNyanAPI
{
    class Options
    {
        [Option('t', "trigger", Required = true,  HelpText = "VNyan trigger to call")]  public string  TriggerName { get; set; }
        [Option('a', "num1",    Required = false, HelpText = "VNyan trigger: Number1")] public int?    Num1 { get; set; }
        [Option('b', "num2",    Required = false, HelpText = "VNyan trigger: Number2")] public int?    Num2 { get; set; }
        [Option('c', "num3",    Required = false, HelpText = "VNyan trigger: Number3")] public int?    Num3 { get; set; }
        [Option('d', "text1",   Required = false, HelpText = "VNyan trigger: Text1")]   public string? Text1 { get; set; }
        [Option('e', "text2",   Required = false, HelpText = "VNyan trigger: Text2")]   public string? Text2 { get; set; }
        [Option('f', "text3",   Required = false, HelpText = "VNyan trigger: Text3")]   public string? Text3 { get; set; }
        [Option('v', "verbose", Required = false, HelpText = "Display JSON data and response")] public bool Verbose { get; set; }

    }

    class VNyanAPI
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                    {
                        string VNyanURL = "http://localhost:8069/";

                        dynamic Content = new JObject(
                            new JProperty("Action", "_lum_miu_vnyanapi"),
                            new JProperty("Payload",
                                new JObject(
                                    new JProperty("Trigger", o.TriggerName)
                                )
                            )
                        );
                        if (o.Text1 != null) { Content.Payload.Add(new JProperty("Text1", o.Text1)); }
                        if (o.Text2 != null) { Content.Payload.Add(new JProperty("Text2", o.Text2)); }
                        if (o.Text3 != null) { Content.Payload.Add(new JProperty("Text3", o.Text3)); }
                        if (o.Num1 != null) { Content.Payload.Add(new JProperty("Int1", o.Num1.ToString())); }
                        if (o.Num2 != null) { Content.Payload.Add(new JProperty("Int2", o.Num2.ToString())); }
                        if (o.Num3 != null) { Content.Payload.Add(new JProperty("Int3", o.Num3.ToString())); }

                        if (o.Verbose) { Console.WriteLine(Content.ToString()); }

                        HttpClient client = new HttpClient();
                        var jsonData = new StringContent(Content.ToString(), Encoding.ASCII);
                        jsonData.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                        var PostResult = Task.Run(() => client.PostAsync(VNyanURL, jsonData)).Result;
                        string Response = PostResult.Content.ReadAsStringAsync().Result;
                        int httpStatus = ((int)PostResult.StatusCode);
                        if (o.Verbose)
                        {
                            Console.WriteLine("HTTP Response: " + httpStatus.ToString());
                            Console.WriteLine(Response);
                        }
                    }
                )
            ;
        }
    }
}
