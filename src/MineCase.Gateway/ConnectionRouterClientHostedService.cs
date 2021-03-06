﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MineCase.Gateway.Network;
using Orleans;

namespace MineCase.Gateway
{
    public class ConnectionRouterClientHostedService : IHostedService
    {
        private TcpListener _tcpListener = null;
        private readonly IClusterClient _client;

        public ConnectionRouterClientHostedService(IClusterClient client, IHostApplicationLifetime lifetime)
        {
            _client = client;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                Int32 port = 25565;
                IPAddress localAddr = IPAddress.Parse("0.0.0.0");

                _tcpListener = new TcpListener(localAddr, port);

                // Start listening for client requests.
                _tcpListener.Start();

                // Enter the listening loop.
                while (true)
                {
                    // Console.Write("Waiting for a connection... ");
                    // Waiting for a connection.
                    DispatchIncomingClient(await _tcpListener.AcceptTcpClientAsync());
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                _tcpListener.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async void DispatchIncomingClient(TcpClient tcpClient)
        {
            try
            {
                using (ClientSession session = new ClientSession(tcpClient, _client))
                {
                    await session.Startup();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}