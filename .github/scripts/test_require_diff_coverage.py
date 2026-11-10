import contextlib
import importlib.util
import io
import os
import sys
import tempfile
import unittest
from unittest import mock

spec = importlib.util.spec_from_file_location(
    'coverage_gate', os.path.join(os.path.dirname(__file__), 'require_diff_coverage.py'))
gate = importlib.util.module_from_spec(spec)
spec.loader.exec_module(gate)


class SourceScopeTests(unittest.TestCase):
    def run_gate(self, changed, report_path, report_lines, source_root):
        with tempfile.TemporaryDirectory() as root:
            report = os.path.join(root, 'tests', 'coverage.info')
            os.makedirs(os.path.dirname(report))
            with open(report, 'w') as handle:
                handle.write('SF:' + report_path + '\n')
                for line, hits in report_lines:
                    handle.write('DA:%s,%s\n' % (line, hits))
                handle.write('end_of_record\n')
            args = ['coverage_gate', '--base', 'base', '--report', report,
                    '--format', 'lcov', '--source-root', source_root]
            with mock.patch.object(sys, 'argv', args), \
                    mock.patch.object(gate, 'git_output', return_value=root), \
                    mock.patch.object(gate, 'changed_lines', return_value=changed), \
                    contextlib.redirect_stdout(io.StringIO()):
                return gate.main()

    def test_no_changes_in_language_passes(self):
        self.assertEqual(0, self.run_gate({'ts/src/client.ts': {1}}, 'Client.java', [(1, 1)], 'java'))

    def test_report_in_test_directory_still_covers_library(self):
        self.assertEqual(0, self.run_gate(
            {'csharp/core/Client.cs': {1}, 'csharp/OpenApiClientUnitTests/TestClient.cs': {2}},
            'Client.cs', [(1, 1)], 'csharp'))

    def test_unmatched_changed_source_fails(self):
        self.assertEqual(1, self.run_gate({'ts/src/client.ts': {1}}, 'other.ts', [(1, 1)], 'ts'))

    def test_uncovered_changed_line_fails(self):
        self.assertEqual(1, self.run_gate({'ts/src/client.ts': {1}}, 'client.ts', [(1, 0)], 'ts'))

    def test_comment_change_without_instrumentation_passes(self):
        self.assertEqual(0, self.run_gate({'ts/src/client.ts': {2}}, 'client.ts', [(1, 1)], 'ts'))

    def test_same_basename_in_unrelated_directory_does_not_match(self):
        self.assertEqual(1, self.run_gate(
            {'ts/src/client.ts': {1}}, 'src/websocketUtils/client.ts', [(1, 1)], 'ts'))

    def test_changelog_and_package_metadata_pass(self):
        self.assertEqual(0, self.run_gate(
            {'ts/ChangeLog.md': {1}, 'ts/package.json': {3}},
            'src/client.ts', [(1, 1)], 'ts'))

    def test_go_module_path_matches_source_relative_path(self):
        self.assertEqual(0, self.run_gate(
            {'golang/client/client.go': {1}}, 'github.com/example/sdk/client/client.go', [(1, 1)], 'golang'))
