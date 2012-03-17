using System;
using System.Linq;
using System.Threading;
using IdeoneClient;

namespace ExecuteFSharp
{
    class Program
    {
        private const string _code = @"
let fib n =
  let rec fib' a b n =
    match n with
    | 0 -> a
    | 1 -> b
    | _ -> fib' b (a + b) (n - 1)
  fib' 0 1 n
 
fib 20 |> printf ""%d""";

        static void Main(string[] args)
        {
            var service = new IdeoneService("your_user", "your_pass"); // use your account

            try
            {
                Console.Write("Getting F#: ");
                var languages = service.GetLanguages();
                var fsharp = languages.First(p => p.Value.StartsWith("F#"));
                Console.WriteLine(fsharp);

                Console.Write("Creating submission: ");
                var link = service.CreateSubmission(_code, fsharp.Key, "", true, true);
                Console.WriteLine(link);

                SubmissionStatus status;

                while (true)
                {
                    status = service.GetSubmissionStatus(link);
                    Console.WriteLine(status.State);

                    if (status.State == SubmissionState.Done)
                    {
                        break;
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }

                if (status.Result != SubmissionResult.Success)
                {
                    Console.WriteLine(status.Result + "!!!");
                    return;
                }

                var detail = service.GetSubmissionDetail(link, false, false, true, false, false);
                Console.WriteLine("CreateAt: " + detail.CreateAt);
                Console.WriteLine("Time: " + detail.Time + "s");
                Console.WriteLine("Memory: " + detail.Memory + "kb");
                Console.WriteLine("Output: " + detail.Output);
            }
            catch (IdeoneException ex)
            {
                Console.WriteLine(ex.Message);
            }


            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }
    }
}
