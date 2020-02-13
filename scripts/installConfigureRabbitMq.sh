#!/bin/bash

sudo apt-get install -y rabbitmq-server
sudo rabbitmq-plugins enable rabbitmq_management

sudo rabbitmqctl add_user admin admin
sudo rabbitmqctl set_user_tags admin administrator
sudo rabbitmqctl set_permissions -p / admin ".*" ".*" ".*"

echo 'RabbitMQ users:'
sudo rabbitmqctl list_users

echo 'RabbitMQ config:'
cat /etc/rabbitmq/rabbitmq-env.conf

echo 'Enabled plugings:'
cat /etc/rabbitmq/enabled_plugins
