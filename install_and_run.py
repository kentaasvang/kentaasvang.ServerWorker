import os
from pathlib import Path

# check if already installed
systemd_file_path = Path("/etc/systemd/system/server-worker.service")
systemd_exist = systemd_file_path.exists() 
if (systemd_exist):
    print("systemd-file already exists. Exiting..")
    exit(0)

# add systemd-file
# TODO: User should be fixed automatically
cwd = Path().cwd()
main = cwd / "main.py"
systemd_file_content = f"""[Unit]
Description=My server worker
After=multi-user.target

[Service]
User=admin
Type=simple
Restart=always
WorkingDirectory={cwd}
ExecStart=/usr/bin/python3 {main} 

[Install]
WantedBy=multi-user.target
"""

systemd_file_path.touch()
with open(systemd_file_path, "w") as service_file:
    service_file.write(systemd_file_content)

# enable and start service
os.system("sudo systemctl daemon-reload")
os.system("sudo systemctl enable server-worker.service")
os.system("sudo systemctl start server-worker.service")