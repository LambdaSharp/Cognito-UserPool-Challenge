#!/bin/bash

# create the policy
POLICY_ARN=$(aws iam create-policy \
--policy-name ${POLICY_NAME} \
--policy-document file://policies/user-pool-sns-policy.json \
--query "Policy.Arn" \
--output text)
echo "POLICY_ARN=${POLICY_ARN}" | tee -a cli_config.sh

# replace the user_pool id
sed -i '.bkup' "s,USER_POOL_ID,${USER_POOL_ID},g" policies/lambda-user-pool-triggers-role.json

# create the role
aws iam create-role \
--role-name  ${ROLE_NAME} \
--assume-role-policy-document file://policies/lambda-user-pool-triggers-role.json

# get the role arn that was just created, it didn't like `query` in the previous command
ROLE_ARN=$(aws iam get-role \
--role-name ${ROLE_NAME} \
--query "Role.Arn" \
--output text)
echo "ROLE_ARN=${ROLE_ARN}" | tee -a cli_config.sh

# attach the policy to the role
aws iam attach-role-policy \
--role-name ${ROLE_NAME} \
--policy-arn ${POLICY_ARN} > /dev/null
