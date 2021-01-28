using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tweetinvi;
using Tweetinvi.AspNet;
using Tweetinvi.Models;

namespace Thut.WebServiceTwitter
{
	public class Startup
	{
		public static IConfiguration Configuration { get; set; }
		public static IAccountActivityRequestHandler AccountActivityRequestHandler { get; set; }
		public static ITwitterClient WebhookClient { get; set; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			WebhookServerInitialization(app);

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

			var r = Assembly.GetExecutingAssembly()
				.GetCustomAttributes(typeof(System.Runtime.Versioning.TargetFrameworkAttribute), false)
				.SingleOrDefault();

			Console.WriteLine(r);

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapGet("/", async context =>
				{
					await context.Response.WriteAsync("Hello World!");
				});
			});
		}

		private static void WebhookServerInitialization(IApplicationBuilder app)
		{
			Plugins.Add<AspNetPlugin>();

			var consumerToken = Configuration["AppSettings:CONSUMER_TOKEN"];
			var consumerSecret = Configuration["AppSettings:CONSUMER_TOKEN"];
			var accessToken = Configuration["AppSettings:CONSUMER_TOKEN"];
			var accessTokenSecret = Configuration["AppSettings:CONSUMER_TOKEN"];
			var bearerToken = Configuration["AppSettings:BEARER_TOKEN"];

			var credentials = new TwitterCredentials(consumerToken,
				consumerSecret, accessToken, accessTokenSecret)
			{
				BearerToken = bearerToken
			};

			WebhookClient = new TwitterClient(credentials);
			AccountActivityRequestHandler = WebhookClient.AccountActivity.CreateRequestHandler();
			var config = new WebhookMiddlewareConfiguration(AccountActivityRequestHandler);
			app.UseTweetinviWebhooks(config);
		}
	}
}
