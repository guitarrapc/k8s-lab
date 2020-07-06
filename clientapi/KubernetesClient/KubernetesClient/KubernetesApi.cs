﻿using System;
using System.Threading.Tasks;
using KubernetesClient.Responses;

namespace KubernetesClient
{
    public class KubernetesApiConfig
    {
        public bool SkipCertificateValidation { get; set; }
        public ResponseType ResponseType { get; set; } = ResponseType.Json;

        public void Configure(IKubernetesClient _provider)
        {
            _provider.SkipCertificationValidation = SkipCertificateValidation;
        }
    }

    public class KubernetesApi
    {
        public bool IsRunningOnKubernetes { get; }
        private IKubernetesClient _provider;
        private KubernetesApiConfig _config = new KubernetesApiConfig();

        public KubernetesApi()
        {
            _provider = GetDefaultProvider();
            _provider.SkipCertificationValidation = _config.SkipCertificateValidation;
            _provider.ResponseType = _config.ResponseType;
            IsRunningOnKubernetes = _provider.IsRunningOnKubernetes;
        }

        public KubernetesApi(KubernetesApiConfig config)
        {
            _config = config;
            _provider = GetDefaultProvider();
            _provider.SkipCertificationValidation = _config.SkipCertificateValidation;
            _provider.ResponseType = _config.ResponseType;

            IsRunningOnKubernetes = _provider.IsRunningOnKubernetes;
        }

        public KubernetesClientStatusResponse GetStatusAsync()
        {
            var status = new KubernetesClientStatusResponse
            {
                AccessToken = _provider.AccessToken,
                HostName = _provider.HostName,
                IsRunningOnKubernetes = _provider.IsRunningOnKubernetes,
                KubernetesServiceEndPoint = _provider.KubernetesServiceEndPoint,
                Namespace = _provider.Namespace,
                SkipCertificationValidation = _provider.SkipCertificationValidation,
            };
            return status;
        }

        public void ConfigureClient(bool skipCertficateValidate)
        {
            _provider.SkipCertificationValidation = skipCertficateValidate;
        }

        public async ValueTask<string> GetApiAsync(string apiPath)
        {
            using (var httpClient = _provider.CreateHttpClient())
            {
                var res = await httpClient.GetStringAsync(_provider.KubernetesServiceEndPoint + apiPath);
                return res;
            }
        }

        /// <summary>
        /// OpenAPI Swagger Definition. https://kubernetes.io/ja/docs/concepts/overview/kubernetes-api/
        /// </summary>
        /// <returns></returns>
        public async ValueTask<string> GetOpenApiSpecAsync()
        {
            using (var httpClient = _provider.CreateHttpClient())
            {
                var apiPath = "/openapi/v2";
                var res = await httpClient.GetStringAsync(_provider.KubernetesServiceEndPoint + apiPath);
                return res;
            }
        }

        private static IKubernetesClient GetDefaultProvider()
        {
            if (Environment.OSVersion.Platform != PlatformID.Unix)
                throw new NotImplementedException($"{Environment.OSVersion.Platform} is not supported.");
            return (IKubernetesClient)new UnixKubernetesClient();
        }
    }
}
