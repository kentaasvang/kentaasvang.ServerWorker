import os
from pathlib import Path

# check if already installed
systemd_file_path = Path("/etc/systemd/system/server-worker.service")
systemd_exist = systemd_file_path.exists() 
if not systemd_exist:
    print("systemd-file don't exist")
    exit(0)

os.system("sudo systemctl stop server-worker.service")
os.system("sudo systemctl disable server-worker.service")
os.system("sudo systemctl daemon-reload")

# remove systemd-file
os.remove(systemd_file_path)