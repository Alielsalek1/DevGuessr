using System.Diagnostics;
using System.Net.Http.Json;
using Application.DTOs.Auth;
using Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace Tests.StressTests;

[Collection("Integration Tests")]
public class ApiStressTests(CustomWebApplicationFactory factory, ITestOutputHelper output) : BaseIntegrationTest(factory)
{
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public async Task StressTest_GuestLogin_UnderLoad()
    {
        const int concurrentRequests = 50;
        const int totalRequests = 200;

        var stopwatch = Stopwatch.StartNew();
        var tasks = new List<Task<(bool Success, TimeSpan ResponseTime, string? Error)>>();

        // Create semaphore to limit concurrent requests
        using var semaphore = new SemaphoreSlim(concurrentRequests);

        for (int i = 0; i < totalRequests; i++)
        {
            tasks.Add(ExecuteGuestLoginWithSemaphore(semaphore));
        }

        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Analyze results
        var successfulRequests = results.Count(r => r.Success);
        var failedRequests = results.Count(r => !r.Success);
        var averageResponseTime = results.Where(r => r.Success).Average(r => r.ResponseTime.TotalMilliseconds);
        var maxResponseTime = results.Where(r => r.Success).Max(r => r.ResponseTime.TotalMilliseconds);
        var minResponseTime = results.Where(r => r.Success).Min(r => r.ResponseTime.TotalMilliseconds);
        var p95ResponseTime = results.Where(r => r.Success)
            .OrderBy(r => r.ResponseTime.TotalMilliseconds)
            .Skip((int)(results.Count(r => r.Success) * 0.95))
            .First().ResponseTime.TotalMilliseconds;

        _output.WriteLine($"=== Guest Login Stress Test Results ===");
        _output.WriteLine($"Total Requests: {totalRequests}");
        _output.WriteLine($"Concurrent Requests: {concurrentRequests}");
        _output.WriteLine($"Total Time: {stopwatch.Elapsed.TotalSeconds:F2} seconds");
        _output.WriteLine($"Requests/Second: {totalRequests / stopwatch.Elapsed.TotalSeconds:F2}");
        _output.WriteLine($"Successful Requests: {successfulRequests}");
        _output.WriteLine($"Failed Requests: {failedRequests}");
        _output.WriteLine($"Success Rate: {(successfulRequests / (double)totalRequests) * 100:F2}%");
        _output.WriteLine($"Average Response Time: {averageResponseTime:F2} ms");
        _output.WriteLine($"Min Response Time: {minResponseTime:F2} ms");
        _output.WriteLine($"Max Response Time: {maxResponseTime:F2} ms");
        _output.WriteLine($"95th Percentile: {p95ResponseTime:F2} ms");

        // Assertions
        Assert.True(successfulRequests > totalRequests * 0.95, $"Success rate too low: {(successfulRequests / (double)totalRequests) * 100:F2}%");
        Assert.True(averageResponseTime < 2000, $"Average response time too high: {averageResponseTime:F2} ms");
        Assert.True(p95ResponseTime < 5000, $"95th percentile response time too high: {p95ResponseTime:F2} ms");

        // Log failures
        var failures = results.Where(r => !r.Success).ToList();
        if (failures.Any())
        {
            _output.WriteLine("\n=== Failures ===");
            foreach (var failure in failures.Take(5)) // Show first 5 failures
            {
                _output.WriteLine($"Error: {failure.Error}");
            }
            if (failures.Count > 5)
            {
                _output.WriteLine($"... and {failures.Count - 5} more failures");
            }
        }
    }

    [Fact]
    public async Task StressTest_UserRegistration_UnderLoad()
    {
        const int concurrentRequests = 20;
        const int totalRequests = 100;

        var stopwatch = Stopwatch.StartNew();
        var tasks = new List<Task<(bool Success, TimeSpan ResponseTime, string? Error)>>();

        using var semaphore = new SemaphoreSlim(concurrentRequests);

        for (int i = 0; i < totalRequests; i++)
        {
            var uniqueEmail = $"stress_test_user_{i}_{Guid.NewGuid()}@example.com";
            tasks.Add(ExecuteUserRegistrationWithSemaphore(semaphore, uniqueEmail));
        }

        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();

        var successfulRequests = results.Count(r => r.Success);
        var failedRequests = results.Count(r => !r.Success);
        var successfulResults = results.Where(r => r.Success).ToList();
        var averageResponseTime = successfulResults.Any() ? successfulResults.Average(r => r.ResponseTime.TotalMilliseconds) : 0;
        var maxResponseTime = successfulResults.Any() ? successfulResults.Max(r => r.ResponseTime.TotalMilliseconds) : 0;
        var minResponseTime = successfulResults.Any() ? successfulResults.Min(r => r.ResponseTime.TotalMilliseconds) : 0;

        _output.WriteLine($"=== User Registration Stress Test Results ===");
        _output.WriteLine($"Total Requests: {totalRequests}");
        _output.WriteLine($"Concurrent Requests: {concurrentRequests}");
        _output.WriteLine($"Total Time: {stopwatch.Elapsed.TotalSeconds:F2} seconds");
        _output.WriteLine($"Requests/Second: {totalRequests / stopwatch.Elapsed.TotalSeconds:F2}");
        _output.WriteLine($"Successful Requests: {successfulRequests}");
        _output.WriteLine($"Failed Requests: {failedRequests}");
        _output.WriteLine($"Success Rate: {(successfulRequests / (double)totalRequests) * 100:F2}%");
        _output.WriteLine($"Average Response Time: {averageResponseTime:F2} ms");
        _output.WriteLine($"Min Response Time: {minResponseTime:F2} ms");
        _output.WriteLine($"Max Response Time: {maxResponseTime:F2} ms");

        Assert.True(successfulRequests > totalRequests * 0.90, $"Success rate too low: {(successfulRequests / (double)totalRequests) * 100:F2}%");
        Assert.True(averageResponseTime < 3000, $"Average response time too high: {averageResponseTime:F2} ms");
    }

    [Fact]
    public async Task StressTest_HealthCheck_UnderHighLoad()
    {
        const int concurrentRequests = 5;
        const int totalRequests = 20;

        var stopwatch = Stopwatch.StartNew();
        var tasks = new List<Task<(bool Success, TimeSpan ResponseTime, string? Error)>>();

        using var semaphore = new SemaphoreSlim(concurrentRequests);

        for (int i = 0; i < totalRequests; i++)
        {
            tasks.Add(ExecuteHealthCheckWithSemaphore(semaphore));
        }

        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();

        var successfulRequests = results.Count(r => r.Success);
        var failedRequests = results.Count(r => !r.Success);

        if (successfulRequests == 0)
        {
            var firstErrors = results.Where(r => !r.Success).Take(5).Select(r => r.Error).ToList();
            Assert.Fail($"All stress test requests failed. First 5 errors: {string.Join(" | ", firstErrors)}");
        }

        var averageResponseTime = results.Where(r => r.Success).Average(r => r.ResponseTime.TotalMilliseconds);
        var maxResponseTime = results.Where(r => r.Success).Max(r => r.ResponseTime.TotalMilliseconds);
        var minResponseTime = results.Where(r => r.Success).Min(r => r.ResponseTime.TotalMilliseconds);
        var p99ResponseTime = results.Where(r => r.Success)
            .OrderBy(r => r.ResponseTime.TotalMilliseconds)
            .Skip((int)(results.Count(r => r.Success) * 0.99))
            .First().ResponseTime.TotalMilliseconds;

        _output.WriteLine($"=== Health Check Stress Test Results ===");
        _output.WriteLine($"Total Requests: {totalRequests}");
        _output.WriteLine($"Concurrent Requests: {concurrentRequests}");
        _output.WriteLine($"Total Time: {stopwatch.Elapsed.TotalSeconds:F2} seconds");
        _output.WriteLine($"Requests/Second: {totalRequests / stopwatch.Elapsed.TotalSeconds:F2}");
        _output.WriteLine($"Successful Requests: {successfulRequests}");
        _output.WriteLine($"Failed Requests: {failedRequests}");
        _output.WriteLine($"Success Rate: {(successfulRequests / (double)totalRequests) * 100:F2}%");
        _output.WriteLine($"Average Response Time: {averageResponseTime:F2} ms");
        _output.WriteLine($"Min Response Time: {minResponseTime:F2} ms");
        _output.WriteLine($"Max Response Time: {maxResponseTime:F2} ms");
        _output.WriteLine($"99th Percentile: {p99ResponseTime:F2} ms");

        Assert.True(successfulRequests == totalRequests, "All health check requests should succeed");
        Assert.True(averageResponseTime < 500, $"Average response time too high: {averageResponseTime:F2} ms");
        Assert.True(p99ResponseTime < 1000, $"99th percentile response time too high: {p99ResponseTime:F2} ms");
    }

    [Fact]
    public async Task StressTest_MixedEndpoints_UnderLoad()
    {
        const int concurrentRequests = 5;
        const int totalRequests = 20;

        var stopwatch = Stopwatch.StartNew();
        var tasks = new List<Task<(bool Success, TimeSpan ResponseTime, string? Error)>>();

        using var semaphore = new SemaphoreSlim(concurrentRequests);

        // Mix of different endpoint types (excluding registration due to email infrastructure issues in tests)
        for (int i = 0; i < totalRequests; i++)
        {
            if (i % 2 == 0)
            {
                tasks.Add(ExecuteHealthCheckWithSemaphore(semaphore));
            }
            else
            {
                tasks.Add(ExecuteGuestLoginWithSemaphore(semaphore));
            }
        }

        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();

        var successfulRequests = results.Count(r => r.Success);
        var failedRequests = results.Count(r => !r.Success);
        var averageResponseTime = results.Where(r => r.Success).Average(r => r.ResponseTime.TotalMilliseconds);

        _output.WriteLine($"=== Mixed Endpoints Stress Test Results ===");
        _output.WriteLine($"Total Requests: {totalRequests}");
        _output.WriteLine($"Concurrent Requests: {concurrentRequests}");
        _output.WriteLine($"Total Time: {stopwatch.Elapsed.TotalSeconds:F2} seconds");
        _output.WriteLine($"Requests/Second: {totalRequests / stopwatch.Elapsed.TotalSeconds:F2}");
        _output.WriteLine($"Successful Requests: {successfulRequests}");
        _output.WriteLine($"Failed Requests: {failedRequests}");
        _output.WriteLine($"Success Rate: {(successfulRequests / (double)totalRequests) * 100:F2}%");
        _output.WriteLine($"Average Response Time: {averageResponseTime:F2} ms");

        Assert.True(successfulRequests > totalRequests * 0.85, $"Success rate too low: {(successfulRequests / (double)totalRequests) * 100:F2}%");
    }

    private async Task<(bool Success, TimeSpan ResponseTime, string? Error)> ExecuteGuestLoginWithSemaphore(SemaphoreSlim semaphore)
    {
        await semaphore.WaitAsync();
        try
        {
            // Guest login only requires Idempotency-Key header, no request body
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/internal-auth/login/guest");
            requestMessage.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());

            var stopwatch = Stopwatch.StartNew();
            var response = await Client.SendAsync(requestMessage);
            stopwatch.Stop();

            if (response.IsSuccessStatusCode)
            {
                return (true, stopwatch.Elapsed, null);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, stopwatch.Elapsed, $"Status: {response.StatusCode}, Content: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            return (false, TimeSpan.Zero, ex.Message);
        }
        finally
        {
            semaphore.Release();
        }
    }

    private async Task<(bool Success, TimeSpan ResponseTime, string? Error)> ExecuteUserRegistrationWithSemaphore(SemaphoreSlim semaphore, string email)
    {
        await semaphore.WaitAsync();
        try
        {
            var request = new RegisterRequestDto
            {
                Email = email,
                Username = $"user_{Guid.NewGuid().ToString("N").Substring(0, 8)}",
                Password = "TestPassword123!"
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/internal-auth/register");
            requestMessage.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());
            requestMessage.Content = JsonContent.Create(request);

            var stopwatch = Stopwatch.StartNew();
            var response = await Client.SendAsync(requestMessage);
            stopwatch.Stop();

            if (response.IsSuccessStatusCode)
            {
                return (true, stopwatch.Elapsed, null);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, stopwatch.Elapsed, $"Status: {response.StatusCode}, Content: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            return (false, TimeSpan.Zero, ex.Message);
        }
        finally
        {
            semaphore.Release();
        }
    }

    private async Task<(bool Success, TimeSpan ResponseTime, string? Error)> ExecuteHealthCheckWithSemaphore(SemaphoreSlim semaphore)
    {
        await semaphore.WaitAsync();
        try
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await Client.GetAsync("/health");
            stopwatch.Stop();

            if (response.IsSuccessStatusCode)
            {
                return (true, stopwatch.Elapsed, null);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, stopwatch.Elapsed, $"Status: {response.StatusCode}, Content: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            return (false, TimeSpan.Zero, ex.Message);
        }
        finally
        {
            semaphore.Release();
        }
    }
}