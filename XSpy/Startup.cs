using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime.CredentialManagement;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using XSpy.Database;
using XSpy.Socket;
using XSpy.Socket.Auth;

namespace XSpy
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression(options => { options.Providers.Add<GzipCompressionProvider>(); });

            var dbConf = Configuration.GetConnectionString("Database");
            services.AddDatabaseServices(dbConf);

            var amazonSettings = Configuration.GetSection("AmazonSettings");

            var profile = new CredentialProfile("xspy_profile", new CredentialProfileOptions
            {
                AccessKey = amazonSettings.GetValue<string>("AccessKey"),
                SecretKey = amazonSettings.GetValue<string>("SecretKey")
            })
            {
                Region = RegionEndpoint.USEast1
            };

            var netSdkFile = new SharedCredentialsFile();
            netSdkFile.RegisterProfile(profile);
            
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSocketsServices();

            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("HubAuthorizationPolicy", policy =>
                {
                    policy.AddRequirements(new HubAuthorizationRequirement());
                });
            });

            
            services.AddSignalR(hubOptions =>
            {
                hubOptions.ClientTimeoutInterval = TimeSpan.FromSeconds(300);
                hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(10);
                hubOptions.HandshakeTimeout = TimeSpan.FromSeconds(300);
                hubOptions.MaximumParallelInvocationsPerClient = 20;
                hubOptions.EnableDetailedErrors = true;
                hubOptions.MaximumReceiveMessageSize = long.MaxValue;
            }).AddNewtonsoftJsonProtocol();

            services.AddAntiforgery();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                    options => options.LoginPath = "/");

            services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    CookieAuthenticationDefaults.AuthenticationScheme);
                defaultAuthorizationPolicyBuilder =
                    defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });
            
            services.AddControllersWithViews().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling
                    = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.MetadataPropertyHandling = MetadataPropertyHandling.Ignore;

                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                options.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;

                options.SerializerSettings.Converters.Add(new StringEnumConverter());

                options.SerializerSettings.Converters.Add(new IsoDateTimeConverter
                {
                    DateTimeStyles = DateTimeStyles.AssumeLocal
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            FileExtensionContentTypeProvider contentTypes = new FileExtensionContentTypeProvider();
            contentTypes.Mappings[".apk"] = "application/vnd.android.package-archive";
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = contentTypes
            });
            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();
            app.UseResponseCompression();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MainHub>("/telemetry",
                    dispatcherOptions =>
                    {
                        dispatcherOptions.WebSockets.CloseTimeout = TimeSpan.FromSeconds(15);
                    });
            });
        }
    }
}
