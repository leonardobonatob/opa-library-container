using System;
using System.Net.Http;
using System.Diagnostics;
using System.Threading.Tasks;

using TmpFile.CRUD;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace opa_library
{
    /// <summary>
    /// Open Policy Agent class for evaluating expressions on Linux enviroment.
    /// </summary>
    public class Opa
    {
        public static async Task<bool> Eval(Object input){
            try{
                var opaPolicyEndpoint = Environment.GetEnvironmentVariable("OPA_POLICY_ENDPOINT");
                if(opaPolicyEndpoint == null) {
                    Console.WriteLine("env OPA_POLICY_ENDPOINT not defined");
                    return false;
                }

                // Converting System.Text.Json.JsonElement (input from lambda) into Newtonsoft.Json.Linq.JObject
                // The compiler does not check the type of the dynamic type variable at compile time
                dynamic inputJObject = JsonConvert.DeserializeObject(input.ToString());
                
                // Creation of dataFile
                var dataFile = TmpFileCRUD.CreateTmpFile(".json");
                TmpFileCRUD.UpdateTmpFile(dataFile, inputJObject["data"].ToString());

                // Creation of inputFile
                var inputFile = TmpFileCRUD.CreateTmpFile(".json");
                TmpFileCRUD.UpdateTmpFile(inputFile, inputJObject["input"].ToString());

                // Request of policy data
                HttpClient client = new HttpClient();
                string uri = opaPolicyEndpoint + "policy";
                string responseBody = await client.GetStringAsync(uri);

                // Creation of policyFile
                var policyFile = TmpFileCRUD.CreateTmpFile(".rego");
                TmpFileCRUD.UpdateTmpFile(policyFile, responseBody);

                // Find of package and OPA querying
                string package = GetRegoPackage(responseBody);
                var OPAresult = RunOpaProcess(dataFile, inputFile, policyFile, package);

                // Deletion of Files
                TmpFileCRUD.DeleteTmpFile(dataFile);
                TmpFileCRUD.DeleteTmpFile(inputFile);
                TmpFileCRUD.DeleteTmpFile(policyFile);

                return((bool)(JObject.Parse(OPAresult)["allow"]));
            }
            catch (Exception e){
                Console.WriteLine($"\n\nError on Opa.Eval(string value):\n{e}\n\n");
                return false;
            }
        }

        private static string RunOpaProcess(string dataPath, string inputPath, string policyPath, string package)
        {
            var command = $"eval -i {inputPath} -d {policyPath} \"data.{package}\" -d {dataPath} --format pretty";

            try{
                Process process = new Process{
                    StartInfo = {
                        FileName = "./opa",
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
                return $"\nError identified on calling RunOpaProcess method: {e}\n";
            }
        }

        private static string GetRegoPackage(string regoPolicyContent){
            try{
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
            catch(Exception e){
                Console.WriteLine($"\n\nError on Opa.GetRegoPackage(string regoPolicyContent):\n{e}\n\n");
                return "ERROR";
            }
        }
    }
}
