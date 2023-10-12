import os
import tempfile
import unittest
import shutil

# disable for tests
import logging
logging.disable()

from main import _check_and_deploy_files
from utils import read_service_file

class TestServerWorker(unittest.TestCase):

    def setUp(self):
        self.tmp_publish_dir = tempfile.mkdtemp(prefix="publish_") 
        self.tmp_public_dir = tempfile.mkdtemp(prefix="public_") 
        self.tmp_published_file = tempfile.mkstemp(dir=self.tmp_publish_dir, prefix="published_")

    def tearDown(self):
        if os.path.exists(self.tmp_publish_dir):
            shutil.rmtree(self.tmp_publish_dir)

        if os.path.exists(self.tmp_public_dir):
            shutil.rmtree(self.tmp_public_dir)

    def test_items_are_moved_from_publish_dir(self):

        self.tmp_published_folder = tempfile.mkdtemp(dir=self.tmp_publish_dir, prefix="published_dir")

        test_service = { 
            "name": "My Test Service",
            "publish": self.tmp_publish_dir,
            "public": self.tmp_public_dir
        }

        _check_and_deploy_files(test_service)

        tmp_published_file_name = os.path.basename(self.tmp_published_file[1])
        moved_file = os.path.join(self.tmp_public_dir, tmp_published_file_name)

        tmp_published_folder_name = os.path.basename(self.tmp_published_folder)
        moved_folder = os.path.join(self.tmp_public_dir, tmp_published_folder_name)

        self.assertTrue(os.path.exists(moved_file), "File does not exist")
        self.assertTrue(os.path.exists(moved_folder), "Folder does not exist")

    def test_check_old_files_are_removed_from_public_folder(self):
        
        self.tmp_remove_file = tempfile.mkstemp(dir=self.tmp_public_dir, prefix="remove_")
        self.tmp_remove_folder = tempfile.mkdtemp(dir=self.tmp_public_dir, prefix="remove_dir_")

        test_service = { 
            "name": "My Test Service",
            "publish": self.tmp_publish_dir,
            "public": self.tmp_public_dir
        }

        _check_and_deploy_files(test_service)

        tmp_removed_file_name = os.path.basename(self.tmp_remove_file[1])
        removed_file = os.path.join(self.tmp_public_dir, tmp_removed_file_name)

        tmp_removed_folder_name = os.path.basename(self.tmp_remove_folder)
        removed_folder = os.path.join(self.tmp_public_dir, tmp_removed_folder_name)

        self.assertFalse(os.path.exists(removed_file), "File should be removed")
        self.assertFalse(os.path.exists(removed_folder), "Folder should be removed")

    def test_check_dotfiles_are_ignored(self):
        
        self.tmp_remove_file = tempfile.mkstemp(dir=self.tmp_publish_dir, prefix=".")
        self.tmp_existing_dotfile = tempfile.mkstemp(dir=self.tmp_public_dir, prefix=".")

        test_service = { 
            "name": "My Test Service",
            "publish": self.tmp_publish_dir,
            "public": self.tmp_public_dir
        }

        _check_and_deploy_files(test_service)

        tmp_removed_file_name = os.path.basename(self.tmp_remove_file[1])
        removed_file = os.path.join(self.tmp_publish_dir, tmp_removed_file_name)

        tmp_existing_dotfile_name = os.path.basename(self.tmp_existing_dotfile[1])
        existing_dot_file = os.path.join(self.tmp_public_dir, tmp_existing_dotfile_name)

        self.assertTrue(os.path.exists(removed_file), "Dotfiles should be ignored")
        self.assertTrue(os.path.exists(existing_dot_file), "Dotfiles should be ignored")

    def test_check_dotfolders_are_ignored(self):
        
        self.tmp_remove_folder = tempfile.mkdtemp(dir=self.tmp_publish_dir, prefix=".")
        self.tmp_existing_folder = tempfile.mkdtemp(dir=self.tmp_public_dir, prefix=".")

        test_service = { 
            "name": "My Test Service",
            "publish": self.tmp_publish_dir,
            "public": self.tmp_public_dir
        }

        _check_and_deploy_files(test_service)

        tmp_removed_file_name = os.path.basename(self.tmp_remove_folder)
        removed_file = os.path.join(self.tmp_publish_dir, tmp_removed_file_name)

        tmp_existing_folder_name = os.path.basename(self.tmp_existing_folder)
        existing_dot_folder = os.path.join(self.tmp_public_dir, tmp_existing_folder_name)

        self.assertTrue(os.path.exists(removed_file), "Dotfolders should be ignored")
        self.assertTrue(os.path.exists(existing_dot_folder), "Dotfolders should be ignored")

class TestLoadServiceYaml(unittest.TestCase):

    def setUp(self):
        self.tmp_config_file = tempfile.mkstemp(prefix="config_", suffix=".yaml") 

        with open(self.tmp_config_file[1], "w") as f:
            f.write("""services:
  - name: "My first service" 
    publish_dir: "my publish dir"
    public_dir: "my public dir"

  - name: "My second service" 
    publish_dir: "my second publish dir"
    public_dir: "my second public dir"
""")

    def tearDown(self):
        if os.path.exists(self.tmp_config_file[1]):
            os.remove(self.tmp_config_file[1])

    def test_load_service_file(self):
        actual = read_service_file(self.tmp_config_file[1])

        expected = [
            {
                "name": "My first service",
                "publish_dir": "my publish dir",
                "public_dir": "my public dir"
            },
            {
                "name": "My second service",
                "publish_dir": "my second publish dir",
                "public_dir": "my second public dir"
            }
        ]

        self.assertListEqual(expected, actual)


if __name__ == "__main__":
    unittest.main(verbosity=1)