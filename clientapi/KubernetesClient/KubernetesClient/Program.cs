﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ConsoleAppFramework;
using Microsoft.Extensions.Hosting;

namespace KubernetesClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Host.CreateDefaultBuilder().RunConsoleAppFrameworkAsync<ConsoleAppBase>(args);
        }
    }

    public class KubernetsApi : ConsoleAppBase
    {
        private IKubernetesClient _provider;

        public KubernetsApi()
        {
            _provider = GetDefaultProvider();
        }

        public async ValueTask GetOpenApiSpec()
        {
            using (var httpClient = _provider.CreateHttpClient())
            {
                var apiPath = "/openapi/v2";
                var res = await httpClient.GetStringAsync(_provider.KubernetesServiceEndPoint + apiPath);
                Console.WriteLine(res);
            }
        }

        private static IKubernetesClient GetDefaultProvider()
        {
            if (Environment.OSVersion.Platform != PlatformID.Unix)
                throw new NotImplementedException($"{Environment.OSVersion.Platform} is not supported.");
            return (IKubernetesClient)new UnixKubernetesClient();
        }
    }

    public interface IKubernetesClient
    {
        string AccessToken { get; }
        string HostName { get; }
        bool IsRunningOnKubernetes { get; }
        string KubernetesServiceEndPoint { get; }
        string Namespace { get; }
        bool SkipCertificationValidation { get; set; }

        HttpClient CreateHttpClient();
    }

    public class UnixKubernetesClient : KubernetesClientBase
    {
        private bool? _isRunningOnKubernetes;
        private string _namespace;
        private string _hostName;
        private string _accessToken;
        private string _kubernetesServiceEndPoint;

        public override bool IsRunningOnKubernetes
            => _isRunningOnKubernetes ?? (bool)(_isRunningOnKubernetes = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST")));

        public override string AccessToken
            => _accessToken ?? (_accessToken = File.ReadAllText("/var/run/secrets/kubernetes.io/serviceaccount/token"));

        public override string HostName
            => _hostName ?? (_hostName = Environment.GetEnvironmentVariable("HOSTNAME"));

        public override string KubernetesServiceEndPoint
            => _kubernetesServiceEndPoint ?? (_kubernetesServiceEndPoint = $"https://{Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST")}:{Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_PORT")}");

        public override string Namespace
            => _namespace ?? (_namespace = File.ReadAllText("/var/run/secrets/kubernetes.io/serviceaccount/namespace"));
    }

    public abstract class KubernetesClientBase : IKubernetesClient
    {
        public abstract string AccessToken { get; }
        public abstract string HostName { get; }
        public abstract string Namespace { get; }
        public abstract string KubernetesServiceEndPoint { get; }
        public abstract bool IsRunningOnKubernetes { get; }

        public bool SkipCertificationValidation { get; set; }

        public HttpClient CreateHttpClient()
        {
            var httpClientHandler = new HttpClientHandler();
            var httpClient = new HttpClient(httpClientHandler);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            if (SkipCertificationValidation)
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = /* HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; */ delegate { return true; };
            }

            return httpClient;
        }
    }
}
