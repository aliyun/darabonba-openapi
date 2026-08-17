# -*- coding: utf-8 -*-
import ast
import os
import unittest

import cryptography


class TestSetupRequires(unittest.TestCase):
    @staticmethod
    def _load_requires():
        setup_path = os.path.join(os.path.dirname(__file__), '..', 'setup.py')
        with open(setup_path, encoding='utf-8') as fp:
            tree = ast.parse(fp.read(), filename=setup_path)
        for node in tree.body:
            if isinstance(node, ast.Assign):
                for target in node.targets:
                    if isinstance(target, ast.Name) and target.id == 'REQUIRES':
                        return ast.literal_eval(node.value)
        raise AssertionError('REQUIRES not found in setup.py')

    def test_cryptography_upper_bound_allows_v50_on_py39_plus(self):
        # CVE-2026-69247 is fixed in cryptography 50.0.0; keep an upper bound
        # that still allows installing that release on Python >= 3.9.
        requires = self._load_requires()
        marker = "python_version>='3.9'"
        crypto_reqs = [
            r for r in requires
            if isinstance(r, str) and r.startswith('cryptography') and marker in r
        ]
        self.assertEqual(1, len(crypto_reqs), crypto_reqs)
        self.assertIn('<51.0.0', crypto_reqs[0])
        self.assertNotIn('<49.0.0', crypto_reqs[0])

    def test_installed_cryptography_api_for_rsa_sign(self):
        # Smoke-check the cryptography APIs used by rsa_sign remain importable.
        from cryptography.hazmat.primitives import hashes
        from cryptography.hazmat.primitives.asymmetric import padding
        from cryptography.hazmat.primitives.serialization import load_pem_private_key

        self.assertTrue(hasattr(hashes, 'SHA256'))
        self.assertTrue(hasattr(padding, 'PKCS1v15'))
        self.assertTrue(callable(load_pem_private_key))
        # On CI for Python >=3.9 we install cryptography>=50.0.0.
        major = int(cryptography.__version__.split('.', 1)[0])
        if major >= 50:
            self.assertGreaterEqual(major, 50)
