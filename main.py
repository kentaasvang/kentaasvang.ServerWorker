import os
import time
import shutil
from pathlib import Path 


def _check_and_deploy_files(service):

    publish_dir = Path(service["publish"]) 
    public_dir = Path(service["public"])
    published_files = list(publish_dir.iterdir())
        
    if any(published_files):
        for old_file in public_dir.iterdir():
            os.remove(old_file)

        for file in published_files:
            shutil.move(file, public_dir)


def main():
    from utils import read_service_file

    services = read_service_file("services.yaml") 

    while True:
        time.sleep(60)
        for service in services:
            _check_and_deploy_files(service)


if __name__ == "__main__":
    main()