using Chato.Automation.Infrastructure;
using Chato.Automation.Responses;
using System.Net.Http.Json;
using System.Text.Json;
using Chato.Server.Infrastracture;

namespace Chato.Automation.Scenario;

public class ScenarioConfig
{
    public int NumberEmptyLinesBetweenMethods { get; set; } = 2;
}
public abstract class ScenarioBase
{
    //private readonly DateOnlyJsonConverter _dateOnlyConverter;

    protected List<Func<Task>> SetupsLogicCallback;
    protected List<Func<Task>> BusinessLogicCallbacks;
    protected List<Func<Task>> SummaryLogicCallback;

    public ScenarioBase(string baseUrl)
    {

        Logger = AutomationLoggerFactory.CreateLogger();

        BaseUrl = baseUrl;
        Config = new ScenarioConfig();

        SetupsLogicCallback = new List<Func<Task>>();
        BusinessLogicCallbacks = new List<Func<Task>>();
        SummaryLogicCallback = new List<Func<Task>>();

        //_dateOnlyConverter = new DateOnlyJsonConverter();
    }

    public IAutomationLogger Logger { get; }

    public string BaseUrl { get; }

    protected ScenarioConfig Config { get; }

    public abstract string ScenarioName { get; }
    public abstract string Description { get; }

    protected virtual async Task PostRun()
    {
        Console.WriteLine($"No {nameof(PostRun)} operation was performed!!!");
    }

    protected void DisplayEmptyLines()
    {
        var loop = Config.NumberEmptyLinesBetweenMethods;
        while (loop-- > 0)
        {
            Console.WriteLine("");
        }
    }

    protected void DisplayDividerLines(int amount = 2)
    {
        var loop = amount;
        while (loop-- > 0)
        {
            Console.WriteLine("---------------------------------------------------------------------");
        }
    }



    public async Task StartRunScenario()
    {
        Console.WriteLine($" ------------------------{ScenarioName}----------------------------");

        Console.WriteLine();
        Console.WriteLine($"This scenario main purpose: {Description}");
        Console.WriteLine();


        if (SetupsLogicCallback.SafeAny())
        {
            Console.WriteLine($"Setup run started.");
            foreach (var callback in SetupsLogicCallback)
            {
                await callback.Invoke();
                DisplayDividerLines();
            }
            Console.WriteLine($"Setup finished succeffully.");
        }
        else
        {
            Console.WriteLine($"No Setups.");
        }

        Console.WriteLine();

        Console.WriteLine($"Business Logic started with base url = {BaseUrl}.");
        Console.WriteLine();

        try
        {

            var steps = 0;
            while (steps < BusinessLogicCallbacks.Count)
            {
                var callback = BusinessLogicCallbacks.ElementAt(steps);
                if (SetupsLogicCallback.SafeAny())
                {
                    DisplayDividerLines(4);
                }
                Console.WriteLine($"{callback.Method.Name} started.");
                await callback.Invoke();
                Console.WriteLine($"{callback.Method.Name} finished successfully.");

                if (steps + 1 != BusinessLogicCallbacks.Count)
                {
                    DisplayDividerLines();
                    //Console.WriteLine();
                }

                steps++;
            }
            Console.WriteLine();
            Console.WriteLine($"Business Logic finished successfully.");

        }
        finally
        {
            Console.WriteLine();
            if (SummaryLogicCallback.SafeAny())
            {
                DisplayDividerLines(4);
                Console.WriteLine($"Post run started.");
                foreach (var callback in SummaryLogicCallback)
                {
                    await callback.Invoke();
                    DisplayDividerLines();
                }
                Console.WriteLine($"Post run ended succeffully.");
            }
            else
            {
                Console.WriteLine($"No post runs.");
            }
        }
    }

    protected async Task<TDto> Get<TDto>(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(url);

            // Check the response status
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadFromJsonAsync<TDto>();

                return res;

            }

            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new ErrorResponseException { ErrorResponse = errorResponse };
        }
    }
    protected async Task<TResponse> DeleteCommand<TResponse>(string url) where TResponse : class
    {
        using (HttpClient client = new HttpClient())
        {
            var response = await client.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var responseData = JsonSerializer.Deserialize<TResponse>(responseContent, options);
                return responseData;
            }

            throw new Exception($"Failed to perform DELETE request to {url}");
        }

    }

    protected async Task<TResponse> RunPostCommand<TRequest, TResponse>(string url, TRequest request) where TRequest : class
    {
        return await RunPutOrPostCommand<TRequest, TResponse>(url, request, true);

    }

    protected async Task<TResponse> RunPutCommand<TRequest, TResponse>(string url, TRequest request) where TRequest : class
    {
        return await RunPutOrPostCommand<TRequest, TResponse>(url, request, false);
    }


    private async Task<TResponse> RunPutOrPostCommand<TRequest, TResponse>(string url, TRequest request, bool isPostRequest = true)
    {
        using (HttpClient client = new HttpClient())
        {
            var sendOptions = new JsonSerializerOptions();
            //sendOptions.Converters.Add(_dateOnlyConverter);

            var content = new StringContent(JsonSerializer.Serialize(request, sendOptions), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;

            if (isPostRequest)
            {
                response = await client.PostAsync(url, content);
            }
            else
            {
                response = await client.PutAsync(url, content);
            }

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var recieveOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                //recieveOptions.Converters.Add(_dateOnlyConverter);

                var responseData = JsonSerializer.Deserialize<TResponse>(responseContent, recieveOptions);
                return responseData;
            }

            throw new Exception($"Failed Populate in {url}");
        }
    }
}
