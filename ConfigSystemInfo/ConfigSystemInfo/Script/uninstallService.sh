#!/bin/bash
sudo su - <<EOF
systemctl stop configsysteminfo.service
systemctl disable configsysteminfo.service
rm /etc/systemd/system/configsysteminfo.service
systemctl daemon-reload

echo 'uninstallserver success'
EOF
