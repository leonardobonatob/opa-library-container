#!/usr/bin/env python3

import aws_cdk as cdk

from opa.opa_stack import OpaStack


app = cdk.App()
OpaStack(app, "OpaStack")

app.synth()
