from aws_cdk import (
    Duration,
    aws_ec2 as ec2,
    aws_lambda as lambda_,
    aws_sam as sam,
)

from constructs import Construct

ASSET = "../lambdaFunction/src/LambdaPoc/bin/Release/netcoreapp3.1/LambdaPoc.zip"
POWER_TUNNING_APP = ('arn:aws:serverlessrepo:us-east-1:451282441545:'
                     'applications/aws-lambda-power-tuning')


class LambdaFn(Construct):
    def __init__(self, scope: Construct, id: str, *,
                 vpc: ec2.IVpc,
                 opa_policy_endpoint: str) -> None:
        super().__init__(scope, id)

        opa_layer = lambda_.LayerVersion(
            self, "OpaLayer",
            code=lambda_.Code.from_asset("opa.zip"),
            compatible_architectures=[lambda_.Architecture.ARM_64]
        )

        self.function = lambda_.Function(
            self, "OpaFunction",
            layers=[opa_layer],
            runtime=lambda_.Runtime.DOTNET_CORE_3_1,
            architecture=lambda_.Architecture.ARM_64,
            handler="LambdaPoc::LambdaPoc.Function::FunctionHandler",
            code=lambda_.Code.from_asset(ASSET),
            timeout=Duration.seconds(30),
<<<<<<< HEAD
            memory_size=3000,
=======
            memory_size=3008,
>>>>>>> 48b0ce7cca72fad7a924bf42e828eb6a6898134b
            vpc=vpc,
            vpc_subnets=ec2.SubnetType.PRIVATE_ISOLATED,
            environment={
                "OPA_POLICY_ENDPOINT": opa_policy_endpoint,
            }
        )


class PowerTuner(Construct):
    def __init__(self, scope: Construct, id: str, *,
                 lambda_fn: lambda_.IFunction) -> None:
        super().__init__(scope, id)

        sam.CfnApplication(
            self, 'powerTuner',
            location=sam.CfnApplication.ApplicationLocationProperty(
                application_id=POWER_TUNNING_APP,
                semantic_version='4.2.0',
            ),
            parameters={
                "lambdaResource": f"{lambda_fn.function_arn}*",
            },
        )
