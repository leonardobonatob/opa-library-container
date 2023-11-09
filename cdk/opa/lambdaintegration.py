from aws_cdk import (
    Duration,
<<<<<<< HEAD
    aws_ec2 as ec2,
    aws_lambda as lambda_
=======
    aws_lambda as lambda_,
>>>>>>> 48b0ce7cca72fad7a924bf42e828eb6a6898134b
)

from constructs import Construct

ASSET = "../LambdaIntegration"


class LambdaIntegration(Construct):
    def __init__(self, scope: "Construct", id: str) -> None:
        super().__init__(scope, id)

        self.function = lambda_.Function(
            self, "OpaPolicyIntegration",
            runtime=lambda_.Runtime.PYTHON_3_9,
            architecture=lambda_.Architecture.ARM_64,
            handler="lambdapolicy.lambda_handler",
            code=lambda_.Code.from_asset(ASSET),
            timeout=Duration.seconds(30),
<<<<<<< HEAD
            memory_size=3000,
            vpc=vpc,
            vpc_subnets=ec2.SubnetType.PRIVATE_ISOLATED
=======
            memory_size=3008,
>>>>>>> 48b0ce7cca72fad7a924bf42e828eb6a6898134b
        )
