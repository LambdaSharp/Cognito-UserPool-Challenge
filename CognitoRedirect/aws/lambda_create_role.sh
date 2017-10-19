#!/bin/bash
LAMBDA_NAME="lambdasharp_cognito_redirect"
POLICY_NAME="lambdasharp_cognito_redirect_policy"
ROLE_NAME="lambdasharp_cognito_redirect_role"

# create the policy
POLICY_ARN=$(aws iam create-policy \
--policy-name ${POLICY_NAME} \
--policy-document file://lambda-cognito-redirect-role-policy.json \
--query "Policy.Arn" \
--output text)
echo "POLICY_ARN=${POLICY_ARN}"

# create the role
aws iam create-role \
--role-name  ${ROLE_NAME} \
--assume-role-policy-document file://lambda-cognito-redirect-role-trust.json

# get the role arn that was just created, it didn't like `query` in the previous command
ROLE_ARN=$(aws iam get-role \
--role-name ${ROLE_NAME} \
--query "Role.Arn" \
--output text)
echo "ROLE_ARN=${ROLE_ARN}"

# attach the policy to the role
aws iam attach-role-policy \
--role-name ${ROLE_NAME} \
--policy-arn ${POLICY_ARN}
