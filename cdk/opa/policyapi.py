from weakref import proxy
from aws_cdk import (
    aws_iam as iam,
    aws_apigateway as apigateway,
    aws_ec2 as ec2,
    aws_lambda as lambda_
)

from constructs import Construct


class PolicyApi(Construct):
    def __init__(self, scope: Construct, id: str, *, vpc_endpoint: ec2.IVpcEndpoint, lambdaintegration: lambda_.Function) -> None:
        super().__init__(scope, id)

        # Policy Document for apigateway resource policies

        apiResourcePolicy = iam.PolicyDocument(
            statements=[
                iam.PolicyStatement(
                    effect=iam.Effect.ALLOW,
                    actions=["execute-api:Invoke"],
                    principals=[iam.AnyPrincipal()],
                    resources=["*"]
                )
            ]
        )

        # API Gateway for services simulation

        self.api = apigateway.RestApi(
            self, "opa-services-api",
            endpoint_configuration=apigateway.EndpointConfiguration(
                types=[
                    apigateway.EndpointType.PRIVATE,
                ],
                vpc_endpoints=[
                    vpc_endpoint,
                ]
            ),
            policy=apiResourcePolicy
        )
        self.api.root.add_method("GET")  # Enables GET method for the root endpoint

        # GET /policy
        policyUri = "policy"
        policyServer = self.api.root.add_resource(policyUri)
        policyServer.add_method(
            "GET",
            apigateway.LambdaIntegration(lambdaintegration, proxy=True)
        )
