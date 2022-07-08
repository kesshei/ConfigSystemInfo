#!/bin/bash
sudo su - <<EOF
systemctl daemon-reload
systemctl start configsysteminfo.service
systemctl enable configsysteminfo.service
echo 'Server start Success'
EOF
