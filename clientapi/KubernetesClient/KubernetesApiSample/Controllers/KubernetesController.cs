using System.Linq;
using System.Threading.Tasks;
using KubernetesClient;
using KubernetesClient.Models;
using KubernetesClient.Requests;
using KubernetesClient.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KubernetesApiSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class KubernetesController : ControllerBase
    {
        private readonly ILogger<KubernetesController> _logger;
        private readonly Kubernetes _operations;

        public KubernetesController(Kubernetes operations, ILogger<KubernetesController> logger)
        {
            _operations = operations;
            _logger = logger;
        }

        // curl localhost:5000/kubernetes/status
        // response format: always json response.
        [HttpGet("status")]
        public KubernetesClientStatusResponse GetStatus()
        {
            _logger.LogInformation("Get status.");
            var res = _operations.GetStatus();
            return res;
        }

        // curl localhost:5000/kubernetes/configure_client?skipcertificatevalidate=true
        // response format: always json response.
        [HttpGet("configure_client")]
        [HttpPost("configure_client")]
        public KubernetesClientStatusResponse ConfigureClient(bool skipCertificateValidate = true)
        {
            _logger.LogInformation("Configure status.");
            _operations.ConfigureClient(skipCertificateValidate);
            var res = _operations.GetStatus();
            return res;
        }

        // curl localhost:5000/kubernetes/configure_response?isJson=true
        // response format: always json response.
        [HttpGet("configure_response")]
        [HttpPost("configure_response")]
        public KubernetesClientStatusResponse ConfigureResponse(bool isJson = false)
        {
            _logger.LogInformation("Configure response.");
            _operations.ConfigureResponse(isJson);
            var res = _operations.GetStatus();
            return res;
        }

        // curl localhost:5000/kubernetes/spec
        // response format: always json response.
        [HttpGet("spec")]
        public async Task<string> GetSpec()
        {
            _logger.LogInformation("Get spec api.");
            var res = await _operations.GetOpenApiSpecAsync();
            return res;
        }

        #region deployments

        // curl localhost:5000/kubernetes/deployments
        // curl localhost:5000/kubernetes/deployments?ns=default
        // response format: depends on accept type
        [HttpGet("deployments")]
        public async Task<string[]> GetDeployments(string ns = "")
        {
            _logger.LogInformation("Get deployments api.");
            var deployments = await _operations.GetDeploymentsAsync(ns);
            return deployments.items
                .Select(item => $"{item.metadata.@namespace}/{item.metadata.name}")
                .ToArray();
        }

        // curl "localhost:5000/kubernetes/deployment?ns=default&name=kubernetesapisample"
        // response format: depends on accept type
        [HttpGet("deployment")]
        public async Task<V1Deployment> GetDeployment(string ns, string name)
        {
            _logger.LogInformation("Get deployment api.");
            var deployment = await _operations.GetDeploymentAsync(ns, name);
            return deployment;
        }

        // curl -X POST -H "Content-Type: application/json" localhost:5000/kubernetes/deployment -d '{"namespace": "default", "body": "YXBpVmVyc2lvbjogYXBwcy92MSAjICBmb3IgazhzIHZlcnNpb25zIGJlZm9yZSAxLjkuMCB1c2UgYXBwcy92MWJldGEyICBhbmQgYmVmb3JlIDEuOC4wIHVzZSBleHRlbnNpb25zL3YxYmV0YTENCmtpbmQ6IERlcGxveW1lbnQNCm1ldGFkYXRhOg0KICBuYW1lOiBmcm9udGVuZA0Kc3BlYzoNCiAgc2VsZWN0b3I6DQogICAgbWF0Y2hMYWJlbHM6DQogICAgICBhcHA6IGd1ZXN0Ym9vaw0KICB0ZW1wbGF0ZToNCiAgICBtZXRhZGF0YToNCiAgICAgIGxhYmVsczoNCiAgICAgICAgYXBwOiBndWVzdGJvb2sNCiAgICBzcGVjOg0KICAgICAgY29udGFpbmVyczoNCiAgICAgICAgLSBuYW1lOiBwaHAtcmVkaXMNCiAgICAgICAgICBpbWFnZTogazhzLmdjci5pby9ndWVzdGJvb2s6djMNCiAgICAgICAgICByZXNvdXJjZXM6DQogICAgICAgICAgICByZXF1ZXN0czoNCiAgICAgICAgICAgICAgY3B1OiAxMDBtDQogICAgICAgICAgICAgIG1lbW9yeTogMTAwTWkNCiAgICAgICAgICAgIGxpbWl0czoNCiAgICAgICAgICAgICAgY3B1OiAyMDAwbQ0KICAgICAgICAgICAgICBtZW1vcnk6IDEwMDBNaQ0KICAgICAgICAgIGVudjoNCiAgICAgICAgICAgIC0gbmFtZTogR0VUX0hPU1RTX0ZST00NCiAgICAgICAgICAgICAgdmFsdWU6IGRucw0KICAgICAgICAgICAgICAjIElmIHlvdXIgY2x1c3RlciBjb25maWcgZG9lcyBub3QgaW5jbHVkZSBhIGRucyBzZXJ2aWNlLCB0aGVuIHRvDQogICAgICAgICAgICAgICMgaW5zdGVhZCBhY2Nlc3MgZW52aXJvbm1lbnQgdmFyaWFibGVzIHRvIGZpbmQgc2VydmljZSBob3N0DQogICAgICAgICAgICAgICMgaW5mbywgY29tbWVudCBvdXQgdGhlICd2YWx1ZTogZG5zJyBsaW5lIGFib3ZlLCBhbmQgdW5jb21tZW50IHRoZQ0KICAgICAgICAgICAgICAjIGxpbmUgYmVsb3c6DQogICAgICAgICAgICAgICMgdmFsdWU6IGVudg0KICAgICAgICAgIHBvcnRzOg0KICAgICAgICAgICAgLSBuYW1lOiBodHRwLXNlcnZlcg0KICAgICAgICAgICAgICBjb250YWluZXJQb3J0OiAzMDAwDQo="}'
        // response format: depends on accept type
        [HttpPost("deployment")]
        public async Task<V1Deployment> CreateOrUpdateDeployment(KubernetesCreateOrUpdateRequest request)
        {
            _logger.LogInformation($"Create or Replace deployment api. namespace {request.NameSpace}, bodyContentType {request.BodyContentType}, body {request.Body}");
            var decodedBody = Kubernetes.Base64ToString(request.Body);
            var deployment = await _operations.CreateOrReplaceDeploymentAsync(request.NameSpace, decodedBody, request.BodyContentType);
            return deployment;
        }

        // curl -X DELETE -H "Content-Type: application/json" localhost:5000/kubernetes/deployment -d '{"namespace": "default", "name": "frontend"}'
        // response format: depends on accept type
        [HttpDelete("deployment")]
        public async Task<V1Status> DeleteDeployment(KubernetesDeleteRequest request)            
        {
            _logger.LogInformation($"Delete deployment api. namespace {request.NameSpace}, name {request.Name}");
            var options = request.GraceperiodSecond.HasValue
                ? new V1DeleteOptions { gracePeriodSeconds = request.GraceperiodSecond.Value }
                : null;
            var status = await _operations.DeleteDeploymentAsync(request.NameSpace, request.Name, options);
            return status;
        }
        #endregion

        #region jobs

        // curl localhost:5000/kubernetes/jobs
        // curl localhost:5000/kubernetes/jobs?ns=default
        // response format: JSON
        [HttpGet("jobs")]
        public async Task<string[]> GetJobs(string ns = "")
        {
            _logger.LogInformation("Get jobs api.");
            var jobs = await _operations.GetJobsAsync(ns);
            return jobs.items
                .Select(item => $"{item.metadata.@namespace}/{item.metadata.name}")
                .ToArray();
        }

        // curl "localhost:5000/kubernetes/job?ns=default&name=hoge"
        // response format: JSON
        [HttpGet("job")]
        public async Task<V1Job> GetJob(string ns, string name)
        {
            _logger.LogInformation("Get job api.");
            var job = await _operations.GetJobAsync(ns, name);
            return job;
        }

        // curl -X POST -H "Content-Type: application/json" localhost:5000/kubernetes/job -d '{"namespace": "default", "body": "YXBpVmVyc2lvbjogYmF0Y2gvdjEKa2luZDogSm9iCm1ldGFkYXRhOgogIGxhYmVsczoKICAgIGFwcDogaG9nZQogIG5hbWU6IGhvZ2UKc3BlYzoKICBiYWNrb2ZmTGltaXQ6IDYKICBjb21wbGV0aW9uczogMQogIHBhcmFsbGVsaXNtOiAxCiAgdGVtcGxhdGU6CiAgICBtZXRhZGF0YToKICAgICAgbGFiZWxzOgogICAgICAgIGFwcDogaG9nZQogICAgc3BlYzoKICAgICAgY29udGFpbmVyczoKICAgICAgICAtIGltYWdlOiBob2dlCiAgICAgICAgICBpbWFnZVB1bGxQb2xpY3k6IEFsd2F5cwogICAgICAgICAgbmFtZTogaG9nZQogICAgICByZXN0YXJ0UG9saWN5OiBOZXZlcgo="}'
        // response format: YAML
        [HttpPost("job")]
        public async Task<V1Job> CreateOrUpdateJob(KubernetesCreateOrUpdateRequest request)
        {
            _logger.LogInformation($"Create or Replace job api. namespace {request.NameSpace}, bodyContentType {request.BodyContentType}, body {request.Body}");
            var decodedBody = Kubernetes.Base64ToString(request.Body);
            var job = await _operations.CreateOrReplaceJobAsync(request.NameSpace, decodedBody, request.BodyContentType);
            return job;
        }

        // curl -X DELETE -H "Content-Type: application/json" localhost:5000/kubernetes/job -d '{"namespace": "default", "name": "hoge"}'
        // curl -X DELETE -H "Content-Type: application/json" localhost:5000/kubernetes/job -d '{"namespace": "default", "name": "hoge", "gracePeriodSeconds": 0}'
        // response format: depends on accept type
        [HttpDelete("job")]
        public async Task<V1Job> DeleteJob(KubernetesDeleteRequest request)
        {
            _logger.LogInformation($"Delete job api. namespace {request.NameSpace}, name {request.Name}");
            var options = new V1DeleteOptions
            {
                propagationPolicy = "Foreground",
                gracePeriodSeconds = request.GraceperiodSecond,
            };
            var res = await _operations.DeleteJobAsync(request.NameSpace, request.Name, options);
            return res;
        }

        #endregion
    }
}