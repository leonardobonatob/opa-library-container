:: clean_cache:
dotnet nuget locals -c all

:: opa_library:
cd opa-library && dotnet build
@REM mkdir ../fargate/opa-lib
@REM copy -Force bin/Debug/opa_library.1.0.0.nupkg ../fargate/opa-lib/

:: lambda_function:
cd ../lambdaFunction/src/LambdaPoc && dotnet lambda package 

:: cdk:
cd ../../../cdk && cdk deploy

:: cdk_destroy:
	:: cd cdk && cdk destroy