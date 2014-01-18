using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace MobileServicesClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            Func<MobileServiceClient> client = () => new MobileServiceClient("https://codecampmtl-simon.azure-mobile.net/",
                                                 "yboAhyAhMBRqxLwTfwvHpdemfAyveF81");

            var prog = new Doer("Programmer", client);
            var analyst = new Doer("Analyser", client);

            try
            {
                prog.Start();
                analyst.Start();

                Console.Read();
            }
            finally
            {
                prog.Stop();
                analyst.Stop();
            }
        }
    }

    public class Doer
    {
        private readonly string taskName;
        private readonly MobileServiceClient client;
        private bool stopped;

        public Doer(string taskName, Func<MobileServiceClient> client)
        {
            this.taskName = taskName;
            this.client = client();
        }

        public void Stop()
        {
            stopped = true;
        }

        public void Start()
        {
            var t = new Thread(() =>
                {
                    var mobileServiceTable = client.GetTable<TodoItem>();
                    int i=0;
                    while (! stopped)
                    {
                        Task task = mobileServiceTable.InsertAsync(new TodoItem { Text = taskName +" " + i++});
                        task.Wait();

                        Thread.Sleep(1);

                    }
                });

            t.Start();
        }
    }

    
    public class TodoItem
    {
        public string Id { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("complete")]
        public bool Complete { get; set; }
    }
}