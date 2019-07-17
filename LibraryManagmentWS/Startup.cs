﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LibraryManagmentWS
{
	public class Startup
	{
		
		public void ConfigureServices(IServiceCollection services)
		{
			DatabaseContext oDatabaseContext =
				new DatabaseContext();
			oDatabaseContext.Database.EnsureCreated();

			services.AddDbContext<DatabaseContext>(ServiceLifetime.Transient);

			services.AddGrpc();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{				
				endpoints.MapGrpcService<LibraryManagerService>();
			});
		}
	}
}
