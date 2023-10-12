import os
import logging
import time
import shutil
from typing import List
from pathlib import Path 

logging.basicConfig(level=logging.INFO)

def _valid(file: Path) -> bool:
    return file.name.startswith(".") == False

def _get_validated_files(path: Path) -> List[Path]:
    return [f for f in path.iterdir() if _valid(f)]

def _check_and_deploy_files(service):

    publish_dir = Path(service["publish"]) 
    public_dir = Path(service["public"])
    published_files = _get_validated_files(publish_dir)
    public_files = _get_validated_files(public_dir) 
        
    if any(published_files):

        for old_file in public_files:
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

        try:
            for service in services:
                logging.info(f"Deploying files for {service['name']}")
                _check_and_deploy_files(service)
        except Exception as e:
            logging.error(e)


if __name__ == "__main__":
    main()