#!/bin/bash
source cli_global.sh
source cli_config.sh

echo "detach-role-policy: --role-name ${ROLE_NAME} --policy-arn ${POLICY_ARN}"
aws iam detach-role-policy \
--role-name ${ROLE_NAME} \
--policy-arn ${POLICY_ARN}

echo "delete-role --role-name ${ROLE_NAME}"
aws iam delete-role \
--role-name ${ROLE_NAME}

echo "delete-policy --policy-arn arn:aws:iam::${ACCOUNT_ID}:policy/${POLICY_NAME}"
aws iam delete-policy \
--policy-arn arn:aws:iam::${ACCOUNT_ID}:policy/${POLICY_NAME}

sed -i '.bkup' "s,${USER_POOL_ID},USER_POOL_ID,g" policies/lambda-user-pool-triggers-role.json
rm policies/*.bkup

> cli_config.sh