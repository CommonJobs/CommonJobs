using CommandLine;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace Admin
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            // Set "_default_" value in VS (Debug -> Relay.Admin.Cli Properties -> Command line arguments)
            // NOTE: This code does not recognize doble quoted strings as command line does, for example spaces inside quoted string
            args = args.Length != 1 || args[0] != "_default_" ? args
                : @"list-zoho-candidates --pause"
                .Split(' ').Select(x => x.Replace("_!_", " ")).ToArray();
#endif

            var consoleOutput = new ConsoleOutputHelper();

            Func<ExportToZoho.ZohoConfiguration, ExportToZoho.IZohoClient> zohoClientFactory = cfg => new ExportToZoho.ZohoClient(cfg, new ExportToZoho.ZohoApi.ZohoRequestSerializer());

            // Register here all options types
            var result = Parser.Default.ParseArguments<
                ExportToZoho.ExportToZohoOptions,
                ExportToZoho.ListZohoCandidatesOptions
                >(args);

            RegisterWorker<ExportToZoho.ExportToZohoOptions>(result, options => new ExportToZoho.ExportToZohoWorker(
                consoleOutput,
                zohoClientFactory));

            RegisterWorker<ExportToZoho.ListZohoCandidatesOptions>(result, options => new ExportToZoho.ListZohoCandidatesWorker(
                consoleOutput,
                zohoClientFactory));

            result.WithParsed<ICliOptions>(options =>
            {
                if (options.Pause)
                {
                    Pause();
                }
            });

            result.WithNotParsed(errors =>
            {
                // Ugly patch to pause even when parsing fails
                if (args?.Contains("--pause", StringComparer.OrdinalIgnoreCase) ?? false)
                {
                    Pause();
                }
            });

        }

        private static void Pause()
        {
            Console.WriteLine("Press any key to continue . . .");
            Console.ReadKey();
        }

        static void RegisterWorker<TOptions>(ParserResult<object> parserResult, Func<ICliWorker<TOptions>> workerFactory)
        {
            parserResult.WithParsed<TOptions>(options => Run(options, o => workerFactory()));
        }

        static void RegisterWorker<TOptions>(ParserResult<object> parserResult, Func<TOptions, ICliWorker<TOptions>> workerFactory)
        {
            parserResult.WithParsed<TOptions>(options => Run(options, workerFactory));
        }

        static void Run<TOptions>(TOptions options, Func<TOptions, ICliWorker<TOptions>> workerFactory)
        {
            if (!TryValidate(options, out var results))
            {
                Console.WriteLine("Validation Error:\r\n");
                foreach (var error in results)
                {
                    var errorOptions = (error.MemberNames?.Count() ?? 0) == 0 ? string.Empty
                        : $"({(error.MemberNames.Count() == 1 ? "option" : "options")}: { string.Join(", ", error.MemberNames)})";
                    Console.WriteLine($"    * {error.ErrorMessage} {errorOptions}");
                }
                return;
            }

            try
            {
                var worker = workerFactory(options);

                try
                {
                    worker.Run(options).GetAwaiter().GetResult();
                }
                catch (AggregateException aggex)
                {
                    // Sometimes GetAwaiter().GetResult() throws AggregateExceptions
                    Exception real = aggex;
                    while (real is AggregateException)
                    {
                        real = real.InnerException;
                    }
                    // It does not loose original stack trace
                    ExceptionDispatchInfo.Capture(real).Throw();
                }
            }
            catch (ApplicationException e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            catch (Exception e)
            {

                Console.WriteLine($"Unexpected error: {e.Message}");
                Console.WriteLine(e);
            }
        }

        public static bool TryValidate(object @object, out ICollection<ValidationResult> results)
        {
            var context = new ValidationContext(@object, serviceProvider: null, items: null);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(
                @object, context, results,
                validateAllProperties: true
            );
        }
    }
}
