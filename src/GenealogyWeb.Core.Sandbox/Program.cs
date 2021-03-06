﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using GenealogyWeb.Core.Repositories;
using GenealogyWeb.Core.Business.DownwardTree;
using GenealogyWeb.Core.Business.UpwardTree;
using GenealogyWeb.Core.Models;

namespace GenealogyWeb.Core.Sandbox
{
    public class Program
    {
        /// <summary>
        /// Usage:
        /// var foo = ServiceProvider.GetService<INoFoo>();
        /// </summary>
        public static IServiceProvider ServiceProvider;
        public static IConfigurationRoot Configuration;

        public static void Main(string[] args)
        {
            Startup();
            ConfigureServices();

            //// TEST:
            //var dataTreeTests = ServiceProvider.GetService<DataTreeTests>();
            //dataTreeTests.Process();

            var personRepository = ServiceProvider.GetService<PersonRepository>();
            var person = new Person { name = "Test" };
            var id = personRepository.Add(person);
        }

        private static void Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        private static void ConfigureServices()
        {
            var services = new ServiceCollection();

            var connStr = Configuration.GetConnectionString("DefaultConnection");

            if (connStr == null)
                throw new ArgumentNullException("Connection string cannot be null");

            // Add interfaces/implementation to collection
            // External services:
            services.AddTransient(provider => new PersonRepository(connStr));
            services.AddTransient(provider => new MarriageRepository(connStr));
            services.AddTransient(provider => new SonRepository(connStr));
            services.AddTransient<DownwardTreeBuilder>();
            services.AddTransient<UpwardTreeBuilder>();

            // Internal services:
            services.AddTransient<DataTreeTests>();

            // Build our service provider from collection
            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
