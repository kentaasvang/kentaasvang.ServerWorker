import os
import shutil
import tempfile
import unittest

from main import check_and_deploy_files

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

    def test_check_and_deploy_file(self):

        test_service = { 
            "name": "My Test Service",
            "publish": self.tmp_publish_dir,
            "public": self.tmp_public_dir
        }

        check_and_deploy_files(test_service)

        tmp_published_file_name = os.path.basename(self.tmp_published_file[1])
        moved_file = os.path.join(self.tmp_public_dir, tmp_published_file_name)

        self.assertTrue(os.path.exists(moved_file), "File does not exist")


if __name__ == "__main__":
    unittest.main(verbosity=2)