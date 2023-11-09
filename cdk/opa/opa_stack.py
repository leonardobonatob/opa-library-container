from aws_cdk import (
    Stack,
)
from constructs import Construct

from .network import Network
from .policyapi import PolicyApi
from .lambdafn import LambdaFn, PowerTuner
from .fargate import Application
from .lambdaintegration import LambdaIntegration
from .fargate_server import Opa_Server

class OpaStack(Stack):

    def __init__(self, scope: Construct, construct_id: str, **kwargs) -> None:
        super().__init__(scope, construct_id, **kwargs)

        network = Network(self, "network")

        # lambda_api = LambdaIntegration(self, "lambdaapi")

        # policy_api = PolicyApi(self, "policyapi", vpc_endpoint=network.api_gateway_endpoint, lambdaintegration=lambda_api.function)

        # lambdafn = LambdaFn(self, "lambda", vpc=network.vpc, opa_policy_endpoint=policy_api.api.url)

        # # usage: modify the cdk.context.json file
        # create_power_tuner = self.node.try_get_context("createPowerTuner") or False
        # if create_power_tuner:
        #     PowerTuner(self, "powertuner", lambda_fn=lambdafn.function)

        # Application(self, "fargate", vpc=network.vpc, opa_policy_endpoint=policy_api.api.url)

        fargateserver = Opa_Server(self, "fargateOpaServer", vpc=network.vpc)
        
        Application(self, "fargate", vpc=network.vpc, opa_policy_endpoint=fargateserver.url  ) #policy_api.api.url)