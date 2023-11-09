# Steps

```bash
cd opa-library
dotnet restore
dotnet build
rsync bin/Debug/opa_library.1.0.0.nupkg ../fargate/opa-lib/
```

```bash
cd lambdaFunction/src/LambdaPoc
dotnet restore
dotnet lambda package
```

```bash
cd cdk
cdk deploy
```

# Actual way to testing Opa due to it's requests to get policy document

## Description of the problem
The Opa class makes http requests to the Api Gateway to get the policy.rego data.
This request needs an url that changes everytime the ApiGateway is re-deployed.
In consequence of it, in this version of the code the following steps are need in order to properly test the  POC.

## How to test
1. Fist, comment everything in the cdk/opa/opa_stack.py below the Api Gateway definition (from line 63).
2. Execute the `cdk deploy` command at /cdk for the first deployment of the Api Gateway.
4. While waiting for deployment, execute `dotnet nuget locals -c all` command to clean the old cached packages
5. Copy the Api Gateway endpoint from the cdk Outputs replacing the value at the `opa-library/Opa.cs > var uri = "{oldUri}"`
6. Add the /policy to the end of the endpoint to be like `http://{apiEndpoint}.amazonaws.com/prod/policy`
7. Execute the `dotnet build` command at /opa-library for updating the library
8. Follow the next steps according on what you want to test
### Lambda Function
- After completing the previous steps (How to test), for testing the lambda function:
1. Execute the `dotnet lambda package` command at /lambdaFunction/src/LambdaPoc to package the lambda function
2. Uncomment the lambda function definition at cdk/opa/opa_stack.py
3. Execute the `cdk deploy` command at /cdk
4. Test it!

### Fargate
- After completing the previos steps (How to test), for testing the fargate:
1. Copy the `opa_library.1.0.0.nupkg` package generated at opa-library/bin/Debug
2. Delete the package at fargate/opa-lib/ and paste the new one
3. Uncomment the vpc and fargate definition at cdk/opa/opa_stack.py
4. Execute the `cdk deploy` command at /cdk
5. Test it! The fargate service endpoint will be shown one of the cdk Outputs

## Testing inputs
All the inputs must follow the Opa pattern, for more informations visit https://www.openpolicyagent.org/ and for examples, visit its playground
- The lambda function and the http requests body for the fargate receives a json input formatted like { 'input': {...}, 'data': {...} }
- The policy.rego is defined at the Api Gateway definition at cdk/opa/opa_stack.py, in the variable `response`

## Authors
The project/solution was designed by Leonardo Bonato Bizaro and Raphael Calciolari, when working on a project aimed at application development . You can contact me using the following email: leonardobonatob@gmail.com