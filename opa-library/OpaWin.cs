using System;
using System.IO;
using System.Diagnostics;
using TmpFile.CRUD;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace opa_library
{
    /// <summary>
    /// Open Policy Agent class for evaluating expressions on Windows enviroment.
    /// </summary>
    public static class OpaWin
    {
        static HttpClient client = new HttpClient();

        private static async Task<string> RunAsync()
        {
            string policyresponse = null;
            var uri = "https://n7yjlu2sy9.execute-api.us-east-1.amazonaws.com/prod/policy";

            HttpResponseMessage response = await client.GetAsync(uri);
             if (response.IsSuccessStatusCode)
            {
              policyresponse = await client.GetStringAsync(uri);
            }
            else{ Console.WriteLine("\n GET Policy - Permission denied \n"); }

             return policyresponse;      
        }


        public static bool Eval(){
            try{
                // Simulation of lambdaInput converted into JObject
                JObject inputJObject = lambdaEventInput();

                // Creation of inputFile
                var inputFile = TmpFileCRUD.CreateTmpFile(".json");
                TmpFileCRUD.UpdateTmpFile(inputFile, inputJObject["input"].ToString());

                // Creation of dataFile
                var dataFile = TmpFileCRUD.CreateTmpFile(".json");
                TmpFileCRUD.UpdateTmpFile(dataFile, inputJObject["data"].ToString());
                
                // Creation of policyFile
                string policyresponse = RunAsync().GetAwaiter().GetResult();
                
                var policyFile = TmpFileCRUD.CreateTmpFile(".rego");
                TmpFileCRUD.UpdateTmpFile(policyFile, policyresponse);

                // Find of package and OPA querying
                string package = GetRegoPackage(policyresponse);
                var OPAresult = RunOpaProcess(dataFile, inputFile, policyFile, package);

                // Deletion of Files
                TmpFileCRUD.DeleteTmpFile(inputFile);
                TmpFileCRUD.DeleteTmpFile(dataFile);
                TmpFileCRUD.DeleteTmpFile(policyFile);
                
                return((bool)(JObject.Parse(OPAresult)["allow"]));
            }
            catch (Exception e){
                Console.WriteLine($"\n\nOpa.Eval() error:\n{e}\n\n");
                return false;
            }
        }

        public static bool EvalPolicy(Object input){
            try{
                // Simulation of lambdaInput converted into JObject
                JObject inputJObject = lambdaEventInput();
                
                // Creation of dataFile
                var dataFile = TmpFileCRUD.CreateTmpFile(".json");
                TmpFileCRUD.UpdateTmpFile(dataFile, inputJObject["data"].ToString());

                // Creation of inputFile
                var inputFile = TmpFileCRUD.CreateTmpFile(".json");
                TmpFileCRUD.UpdateTmpFile(inputFile, inputJObject["input"].ToString());

                // Creation of policyFile
                var policyFile = TmpFileCRUD.CreateTmpFile(".rego");
                TmpFileCRUD.UpdateTmpFile(policyFile, inputJObject["policy"].ToString());

                // Find of package and OPA querying
                string package = GetRegoPackage(inputJObject["policy"].ToString());
                var OPAresult = RunOpaProcess(dataFile, inputFile, policyFile, package);

                // Deletion of Files
                TmpFileCRUD.DeleteTmpFile(dataFile);
                TmpFileCRUD.DeleteTmpFile(inputFile);
                TmpFileCRUD.DeleteTmpFile(policyFile);

                return((bool)(JObject.Parse(OPAresult)["allow"]));
            }
            catch (Exception e){
                Console.WriteLine($"\n\nOpa.Eval(string value) error:\n{e}\n\n");
                return false;
            }
        }

        // Simula a input da Lambda
        private static JObject lambdaEventInput(){
            try{
                var sr = new StreamReader("event.json");
                var fileContent = sr.ReadToEnd();
                sr.Close();
                return JObject.Parse(fileContent);
            }
            catch (Exception e){
                Console.WriteLine($"\nError on calling lambdaEventInput():\n{e}\n\n");
                return null;
            }
        }

        private static string RunOpaProcess(string dataPath, string inputPath, string policyPath, string package){
            var command = $"eval -i {inputPath.Replace("C:", "")} -d {policyPath.Replace("C:", "")} \"data.{package}\" -d {dataPath.Replace("C:", "")} --format raw";

            try{
                Process process = new Process{
                    StartInfo = {
                        FileName = "opa.exe",
                        Arguments = command,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                return process.StandardOutput.ReadToEnd();
            }
            catch (Exception e){
                return $"{e}";
            }
        }

        private static string GetRegoPackage(string regoPolicyContent){
            string package = string.Empty;
            int index = regoPolicyContent.IndexOf("package ") + 8; // size of 'package ' is 8

            var currentChar = regoPolicyContent[index];
            while (currentChar != '\n'){
                package += currentChar;
                index++;
                currentChar = regoPolicyContent[index];
            }

            return package;
        }        
    }
}
