import re

with open("deploy_to_production.ps1", "r", encoding="utf-8") as f:
    content = f.read()

content = content.replace("/tmp/deploy_temp.tar.gz", "/root/deploy_temp.tar.gz")
content = content.replace("scp -o StrictHostKeyChecking=no -i $SSH_KEY deploy_temp.tar.gz ${SERVER}:/tmp/", "scp -o StrictHostKeyChecking=no -i $SSH_KEY deploy_temp.tar.gz ${SERVER}:/root/")

with open("deploy_to_production.ps1", "w", encoding="utf-8") as f:
    f.write(content)

with open("deploy_retry.ps1", "r", encoding="utf-8") as f:
    content_retry = f.read()

content_retry = content_retry.replace("/tmp/deploy_temp.tar.gz", "/root/deploy_temp.tar.gz")
content_retry = content_retry.replace("scp -o StrictHostKeyChecking=no -o ConnectTimeout=30 -i $SSH_KEY deploy_temp.tar.gz ${SERVER}:/tmp/", "scp -o StrictHostKeyChecking=no -o ConnectTimeout=30 -i $SSH_KEY deploy_temp.tar.gz ${SERVER}:/root/")
content_retry = content_retry.replace("scp -o StrictHostKeyChecking=no -o ConnectTimeout=30 -i $SSH_KEY coffee.db ${SERVER}:/tmp/", "scp -o StrictHostKeyChecking=no -o ConnectTimeout=30 -i $SSH_KEY coffee.db ${SERVER}:/root/")
content_retry = content_retry.replace("/tmp/coffee.db", "/root/coffee.db")

with open("deploy_retry.ps1", "w", encoding="utf-8") as f:
    f.write(content_retry)

print("Updated deployment scripts to use /root/ instead of /tmp/")
