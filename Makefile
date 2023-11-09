.PHONY: clean_cache opa_library lambda_function cdk

all: clean_cache opa_library lambda_function cdk

clean_cache:
	rm -Rf ~/.nuget/packages/opa_library

opa_library:
	cd opa-library && dotnet build && rsync bin/Debug/opa_library.1.0.0.nupkg ../fargate/opa-lib/

lambda_function:
	cd lambdaFunction/src/LambdaPoc && dotnet lambda package 

cdk:
	cd cdk && cdk deploy

cdk_destroy:
	cd cdk && cdk destroy
