using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Application.DTOs.Auth;
using Tests.Common;
using Xunit;

namespace Tests.Auth;

[Collection("Integration Tests")]
public class RegisterationEmailTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private static string GetDecodedBody(string base64Body)
    {
        var bytes = Convert.FromBase64String(base64Body);
        return Encoding.UTF8.GetString(bytes);
    }

    [Fact]
    public async Task Register_SendsConfirmationEmail()
    {
        var request = new RegisterRequestDto
        {
            Username = "EmailTestUser",
            Email = "emailtest@example.com",
            Password = "TestPassword123",
            PhoneNumber = "+1234567890",
            Address = "123 Test Street"
        };
        var (response, _, _) = await RegisterationTestHelpers.PostRegisterAsync<object>(Client, request);

        await Task.Delay(500);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var messages = await Factory.MailhogClient!.GetMessagesAsync();
        Assert.NotNull(messages);
        Assert.True(messages.Total > 0, "No emails were sent");

        var emailMessages = await Factory.MailhogClient!.SearchMessagesByRecipientAsync(request.Email);
        Assert.Single(emailMessages.Items);

        var sentEmail = emailMessages.Items[0];
        Assert.Equal(request.Email, sentEmail.To[0].Email);
        Assert.Contains("Activate", sentEmail.Content.Headers["Subject"][0], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Register_EmailContainsCorrectSubject()
    {
        var request = new RegisterRequestDto
        {
            Username = "SubjectTestUser",
            Email = "subjecttest@example.com",
            Password = "TestPassword123"
        };

        await RegisterationTestHelpers.PostRegisterAsync<object>(Client, request);
        await Task.Delay(500);

        var messages = await Factory.MailhogClient!.SearchMessagesByRecipientAsync(request.Email);
        Assert.Single(messages.Items);

        var email = messages.Items[0];
        var subject = email.Content.Headers["Subject"][0];

        Assert.NotNull(subject);
        Assert.Contains("Verification", subject, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Code", subject, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Register_EmailContainsWelcomeMessage()
    {
        var request = new RegisterRequestDto
        {
            Username = "WelcomeTestUser",
            Email = "welcometest@example.com",
            Password = "TestPassword123"
        };

        await RegisterationTestHelpers.PostRegisterAsync<object>(Client, request);
        await Task.Delay(500);

        var messages = await Factory.MailhogClient!.SearchMessagesByRecipientAsync(request.Email);
        Assert.Single(messages.Items);

        var email = messages.Items[0];
        var body = GetDecodedBody(email.Content.Body);
        Assert.Contains("Greetings, traveler!", body);
    }

    [Fact]
    public async Task Register_EmailContainsConfirmationCode()
    {
        var request = new RegisterRequestDto
        {
            Username = "CodeTestUser",
            Email = "codetest@example.com",
            Password = "TestPassword123"
        };

        await RegisterationTestHelpers.PostRegisterAsync<object>(Client, request);
        await Task.Delay(500);

        var messages = await Factory.MailhogClient!.SearchMessagesByRecipientAsync(request.Email);
        Assert.Single(messages.Items);

        var email = messages.Items[0];
        var body = GetDecodedBody(email.Content.Body);

        // Match a 6-digit code
        var match = Regex.Match(body, @"\b\d{6}\b");
        Assert.True(match.Success, "6-digit verification code not found in email body");
    }
}