using Chato.Automation.Infrastructure;
using Chato.Automation.Responses;
using System.Net.Http.Json;
using System.Text.Json;
using Chato.Server.Infrastracture;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity.Data;
using System;

namespace Chato.Automation.Scenario;

public class TestPlanConfig : ChatoConfigBase<TestPlanConfig>
{
    public string BaseUrl { get; set; }
}

public class ScenarioConfig
{
    public int NumberEmptyLinesBetweenMethods { get; set; } = 2;
    public string BaseUrl { get; set; }
}


public abstract class ScenarioBase
{
    //private readonly DateOnlyJsonConverter _dateOnlyConverter;

    protected List<Func<Task>> SetupsLogicCallback;
    protected List<Func<Task>> BusinessLogicCallbacks;
    protected List<Func<Task>> SummaryLogicCallback;

    public ScenarioBase(ILogger logger, ScenarioConfig config)
    {

        Logger = logger;

        Config = config;
        BaseUrl = config.BaseUrl;

        SetupsLogicCallback = new List<Func<Task>>();
        BusinessLogicCallbacks = new List<Func<Task>>();
        SummaryLogicCallback = new List<Func<Task>>();

        //_dateOnlyConverter = new DateOnlyJsonConverter();
    }

    public ILogger Logger { get; }

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


    protected async Task CountDown(int max = 10)
    {
        for (var i =0; i< max; i++)
        {
            await Task.Delay(1000);
            Logger.LogInformation($"Delayed {i + 1}/{max} second.");
        }
    }

    protected async Task CountDown(Func<int ,Task> callback, int max=10)
    {
        for (var i = 0; i < max; i++)
        {
            await callback(i);

            await Task.Delay(1000);
            
            Logger.LogInformation($"Delayed {i + 1}/{max} second.");
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
                    Console.WriteLine($"{callback.Method.Name} started.");
                    await callback.Invoke();
                    Console.WriteLine($"{callback.Method.Name} finished successfully.");
                    //await callback.Invoke();kc
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

    protected async Task<TDto> Get<TDto>(string url, string? token = null) where TDto:class
    {
        using (HttpClient client = new HttpClient())
        {

            if (token is not null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var result = default(TDto);

                if(typeof(TDto) == typeof(byte[]))
                {
                    var buffer = await response.Content.ReadAsByteArrayAsync();
                    return buffer as TDto;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                if (responseContent.IsNullOrEmpty() == false)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    result = JsonSerializer.Deserialize<TDto>(responseContent,options);
                }

                return result;
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

    public async Task<TResponse> UploadFiles<TResponse>(string url, string token, params string[] filePaths)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var content = new MultipartFormDataContent();

            for (int i = 0; i < filePaths.Length; i++)
            {
                var fileStream = File.OpenRead(filePaths[i]);
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
                content.Add(fileContent, $"documents", Path.GetFileName(filePaths[i]));
            }

            var response = await client.PostAsync(url, content);
            return await EnsureSuccess<TResponse>(response) ?? throw new Exception($"Failed Populate in {url}");

        }
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

            return await EnsureSuccess<TResponse>(response) ?? throw new Exception($"Failed Populate in {url}");
        }
    }
    private async Task<TResponse> EnsureSuccess<TResponse>(HttpResponseMessage message)
    {
        var response = default(TResponse);
        if (message.IsSuccessStatusCode)
        {
            string responseContent = await message.Content.ReadAsStringAsync();
            var recieveOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            response = JsonSerializer.Deserialize<TResponse>(responseContent, recieveOptions);
        }

        return response;
    }

}
