import os
import logging
import time
import shutil
from typing import List
from pathlib import Path 

logging.basicConfig(level=logging.INFO)

def _get_validated_files(path: Path, ignored_files=None) -> List[Path]:
    if not ignored_files:
        return list(path.iterdir())

    return [f for f in path.iterdir() if os.path.basename(f) not in ignored_files]

def _check_and_deploy_files(service: dict):

    ignored_files: List[str] | None = service.get("ignore")
    publish_dir = Path(service["publish"]) 
    public_dir = Path(service["public"])

    published_items = _get_validated_files(publish_dir, ignored_files=ignored_files)
    public_items = _get_validated_files(public_dir, ignored_files=ignored_files) 
        
    if any(published_items):

        logging.info(f"Deploying files for {service['name']}")

        for old_item in public_items:
            if old_item.is_dir():
                logging.info(f"Removing folder '{old_item}'")
                shutil.rmtree(old_item)
            
            if old_item.is_file():
                logging.info(f"Removing file '{old_item}'")
                os.remove(old_item)

        for file in published_items:
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
                _check_and_deploy_files(service)
        except Exception as e:
            logging.error(e)


if __name__ == "__main__":
    main()