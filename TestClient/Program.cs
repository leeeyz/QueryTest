using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Parallel.For(0, 1000, x =>
            {
                HttpHelper helper = new HttpHelper(string.Format("http://localhost:52507/home/buy/{0}", x));
                helper.OpenReadCompleted += (s, e) => {
                    using (var reader = new StreamReader(e.Result))
                    {
                        Console.WriteLine(reader.ReadToEnd());
                    }
                };
                helper.OpenReadAsync();
            });

            Console.ReadKey();
        }
    }
}
