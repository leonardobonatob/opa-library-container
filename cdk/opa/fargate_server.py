from aws_cdk import (
    aws_ec2 as ec2,
    aws_ecs as ecs,
    aws_ecs_patterns as ecs_patterns,
)

from constructs import Construct


class Opa_Server(Construct):
    def __init__(self, scope: Construct, id: str, *,
                 vpc: ec2.IVpc,
                ) -> None:
        super().__init__(scope, id)

        cluster = ecs.Cluster(
            self, "FargateOpaServerCluster",
            vpc=vpc
        )

        load_balanced_fargate_service = ecs_patterns.ApplicationLoadBalancedFargateService(
            self, "FargateOpaServerService",
            cluster=cluster,            # Required
            cpu=256,                    # Default is 256
            desired_count=1,            # Default is 1
            task_image_options=ecs_patterns.ApplicationLoadBalancedTaskImageOptions(
                image=ecs.ContainerImage.from_asset("../opa-server"),
                container_port=8181,
                enable_logging=True),
            memory_limit_mib=1024,     # Default is 512
            public_load_balancer=True,
            min_healthy_percent=0,
        )

        load_balanced_fargate_service.target_group.configure_health_check(
            # Hardcoded path, must be equals to the healthcheck path of the application
            path="/healthcheck",
        )
