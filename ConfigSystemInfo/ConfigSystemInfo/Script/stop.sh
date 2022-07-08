#!/bin/bash
sudo su - <<EOF
systemctl stop configsysteminfo.service
systemctl disable configsysteminfo.service
echo 'Server stop Success'
EOF
