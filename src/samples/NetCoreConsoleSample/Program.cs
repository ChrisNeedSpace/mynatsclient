﻿using System;
using MyNatsClient;
using MyNatsClient.Extensions;

namespace NetCoreConsoleSample
{
    public class Program
    {
        private static INatsClient _client;

        public static void Main(string[] args)
        {
            var cnInfo = new ConnectionInfo("localhost")
            {
                Credentials = new Credentials("test", "p@ssword123")
            };
            _client = new NatsClient(cnInfo);
            _client.Connect();

            _client.Sub("getTemp", stream => stream.Subscribe(msg =>
            {
                var parts = msg.GetPayloadAsString().Split('@');
                _client.Pub(msg.ReplyTo, $"Temp is {TempService.Get(parts[0], parts[1])}C");
            }));

            var c = 0;

            while (true)
            {
                Console.WriteLine("Query? (y=yes;n=no)");
                if (Console.ReadKey().KeyChar == 'n')
                    break;

                Console.WriteLine();

                c++;
                Console.WriteLine($"Got reply: {_client.RequestAsync("getTemp", "STOCKHOLM@SWEDEN").Result.GetPayloadAsString()}");
                if (c % 5 == 0)
                {
                    _client.Disconnect();
                    _client.Connect();
                }
            }

            _client.Disconnect();
        }
    }

    internal static class TempService
    {
        private static readonly Random Rnd = new Random();

        internal static decimal Get(string city, string countryCode)
        {
            return Rnd.Next(-3000, 4200) / 100M;
        }
    }
}
