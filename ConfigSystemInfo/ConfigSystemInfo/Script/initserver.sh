#!/bin/bash
ServerPath='/etc/systemd/system'
FileName='configsysteminfo.service'

chmod +x start.sh
chmod +x stop.sh
chmod +x uninstallService.sh

cp $FileName $ServerPath/$FileName
echo 'copy success'
./start.sh
echo 'Server Init Success'
