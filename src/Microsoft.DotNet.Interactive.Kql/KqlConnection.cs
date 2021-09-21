﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kusto.Data;
using Kusto.Data.Security;
using Microsoft.DotNet.Interactive.Connection;
using Microsoft.DotNet.Interactive.SqlServer;

namespace Microsoft.DotNet.Interactive.Kql
{
    public class KqlConnection : KernelConnection
    {
        public string Cluster { get; set; }

        public string Database { get; set; }

        public string PathToService { get; set; }

        public override async Task<Kernel> ConnectKernelAsync()
        {
            var connectionDetails = await BuildConnectionDetailsAsync();

            var sqlClient = new ToolsServiceClient(PathToService);

            var kernel = new MsKqlKernel(
                $"kql-{KernelName}",
                connectionDetails,
                sqlClient);

            await kernel.ConnectAsync();

            return kernel;
        }

        private async Task<KqlConnectionDetails> BuildConnectionDetailsAsync()
        {
            return new KqlConnectionDetails
            {
                Cluster = Cluster,
                Database = Database,
                Token = await GetKustoTokenAsync()
            };
        }

        private async Task<string> GetKustoTokenAsync()
        {
            var kcsb = new KustoConnectionStringBuilder(Cluster, Database)
                .WithAadUserPromptAuthentication();
            var authenticator = HttpClientAuthenticatorFactory.CreateAuthenticator(kcsb);

            var request = new HttpRequestMessage();
            await authenticator.AuthenticateAsync(request);

            // first value of authorization is the auth token
            // stored in <bearer> <token> format
            return request.Headers.GetValues("Authorization").First().Split(' ').Last();
        }

    }
}