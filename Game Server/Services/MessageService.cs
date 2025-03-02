using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using TicTacToeGameServer.Interfaces;
using TicTacToeGameServer.Models;

namespace TicTacToeGameServer.Services
{
    public class MessageService : IMessageService
    {
        private readonly IDictionary<string, IServiceHandler> _services;

        public MessageService(IDictionary<string, IServiceHandler> services) 
        {
            _services = services;
        }

        public async Task<object> HandleMessageAsync(User curUser, string data)
        {
            return Logic(curUser,data);
        }

        private object Logic(User curUser, string data)
        {
            Dictionary<string, object> response = new Dictionary<string, object>();
            try
            {
                if (curUser == null)
                {
                    Console.WriteLine("curUser is null");
                    return JsonConvert.SerializeObject(response);
                }

                Dictionary<string, object> msgData = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                if(!msgData.ContainsKey("Service"))
                {
                    Console.WriteLine("no Service is sent");
                    return JsonConvert.SerializeObject(response);
                }

                string serviceName = msgData["Service"].ToString();

                if(!_services.TryGetValue(serviceName,out var service))
                {
                    Console.WriteLine("Uknown Service: " + serviceName);
                    return JsonConvert.SerializeObject(response);
                }

                response.Add("Service", serviceName);
                object broadcast = service.Handle(curUser, msgData);

                if(broadcast is Dictionary<string,object> broadcastDict && 
                    broadcastDict.Count > 0)
                {
                    //string retData = JsonConvert.SerializeObject(broadcastDict);
                    var settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };

                    // להוסיף את ה־Converter שהכנת:
                    settings.Converters.Add(new IPAddressConverter());

                    // עכשיו לעשות SerializeObject באמצעות אותן הגדרות
                    string retData = JsonConvert.SerializeObject(broadcastDict, settings);


                    curUser.SendMessage(retData);
                    response.Add("Ack", true);
                }

            }
            catch
            (Exception e)
            {
                Console.WriteLine("Uknown exception: " + e.Message);
            }
            return JsonConvert.SerializeObject(response);
        }
    }
    public class IPAddressConverter : JsonConverter<IPAddress>
    {
        public override void WriteJson(JsonWriter writer, IPAddress value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override IPAddress ReadJson(JsonReader reader, Type objectType, IPAddress existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var ipString = (string)reader.Value;
            return IPAddress.Parse(ipString);
        }
    }
}
