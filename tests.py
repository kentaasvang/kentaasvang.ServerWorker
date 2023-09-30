import os
import shutil
import tempfile
import unittest

from main import _check_and_deploy_files
from utils import read_service_file

class TestCheckAndDeploy(unittest.TestCase):

    def setUp(self):
        self.tmp_publish_dir = tempfile.mkdtemp(prefix="publish_") 
        self.tmp_public_dir = tempfile.mkdtemp(prefix="public_") 
        self.tmp_published_file = tempfile.mkstemp(dir=self.tmp_publish_dir, prefix="published_")

    def tearDown(self):
        if os.path.exists(self.tmp_publish_dir):
            shutil.rmtree(self.tmp_publish_dir)

        if os.path.exists(self.tmp_public_dir):
            shutil.rmtree(self.tmp_public_dir)

    def test_check_and_deploy_file(self):

        test_service = { 
            "name": "My Test Service",
            "publish": self.tmp_publish_dir,
            "public": self.tmp_public_dir
        }

        _check_and_deploy_files(test_service)

        tmp_published_file_name = os.path.basename(self.tmp_published_file[1])
        moved_file = os.path.join(self.tmp_public_dir, tmp_published_file_name)

        self.assertTrue(os.path.exists(moved_file), "File does not exist")


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