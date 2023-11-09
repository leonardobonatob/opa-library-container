using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Web;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace opa_library{
    public class OpaMiddleware
    {
        private readonly RequestDelegate _next;

        public OpaMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task<Task> Invoke(HttpContext context)
        {
            Stopwatch middlewarewatch = new Stopwatch();
            middlewarewatch.Start();

            var request = context.Request;
            request.EnableBuffering(); // Old EnableRewind()

            int bufferSize = 1024; // Need find a way to determine automatic -> causes fail: Microsoft.AspNetCore.Server.Kestrel[13] if too low
            Encoding encoding = Encoding.UTF8;

            string requestContent = "";
            using (StreamReader bodyReader = new StreamReader(request.Body, encoding, true, bufferSize, true))
                requestContent = bodyReader.ReadToEndAsync().Result;
            
            bool opaResult = false;
            if (requestContent != null && !requestContent.Equals("")){

                // Measuring Opa excecution time
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                opaResult = await Opa.Eval(requestContent);
                stopwatch.Stop();
                Console.WriteLine($"\nOpa execution time: {stopwatch.ElapsedMilliseconds} ms");
            }
            
            // Check Opa's result
            if (opaResult){
                middlewarewatch.Stop();
                Console.WriteLine(value: $"\nMiddleware execution time: {middlewarewatch.ElapsedMilliseconds} ms\n");

                // Call the next delegate/middleware in the pipeline
                return _next(context);
            }
            else{
                middlewarewatch.Stop();
                Console.WriteLine(value: $"\nMiddleware execution time: {middlewarewatch.ElapsedMilliseconds} ms\n");
                
                // Http response 403
                context.Response.StatusCode = 403;

                // Break the pipeline
                return Task.CompletedTask;
            }
        }
    }

    public static class OpaMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestOpa(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<OpaMiddleware>();
        }
    }
}