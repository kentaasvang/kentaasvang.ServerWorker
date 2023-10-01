import os
import logging
import time
import shutil
from pathlib import Path 

logging.basicConfig(level=logging.INFO)


def _check_and_deploy_files(service):

    publish_dir = Path(service["publish"]) 
    public_dir = Path(service["public"])
    published_files = list(publish_dir.iterdir())
        
    if any(published_files):

        for old_file in public_dir.iterdir():
            logging.info(f"Removing file '{old_file}'")
            os.remove(old_file)

        for file in published_files:
            logging.info(f"Moving '{file}' info '{public_dir}'")
            shutil.move(file, public_dir)


def main():
    from utils import read_service_file
    from pathlib import Path

    cwd = Path().cwd()
    service_yaml_path = cwd / "services.yaml"
    logging.info(f"Reading '{service_yaml_path}'")

    services = read_service_file(service_yaml_path) 

    while True:
        delay_in_secs = 60
        logging.info(f"Waiting for {delay_in_secs} seconds")
        time.sleep(delay_in_secs)

        for service in services:
            logging.info(f"Deploying files for {service['name']}")
            _check_and_deploy_files(service)


if __name__ == "__main__":
    main()