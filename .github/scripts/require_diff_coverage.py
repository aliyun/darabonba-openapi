#!/usr/bin/env python3
"""Fail when changed, instrumented lines are covered below a threshold.

Compares the working tree with the pull-request base and counts only lines
the coverage report instrumented. A partially covered branch counts as missed.
Historical project coverage is ignored, and a diff with no instrumented lines
passes.
"""
from __future__ import print_function

import argparse
import os
import re
import subprocess
import sys
import xml.etree.ElementTree as ET
from collections import defaultdict


def git_output(args, cwd):
    return subprocess.check_output(["git"] + args, cwd=cwd, text=True)


def changed_lines(root, base):
    diff = git_output(["diff", "--unified=0", "--no-color", "--find-renames", base], root)
    files = {}
    path = None
    lines = set()

    def flush():
        if path is not None:
            files.setdefault(path, set()).update(lines)

    for raw in diff.splitlines():
        if raw.startswith("diff --git "):
            flush()
            path = None
            lines = set()
            continue
        if raw.startswith("+++ "):
            name = raw[4:]
            if name == "/dev/null":
                path = None
            else:
                path = name[2:] if name.startswith("b/") else name
            continue
        match = re.match(r"^@@ -\d+(?:,\d+)? \+(\d+)(?:,(\d+))? @@", raw)
        if match and path:
            start = int(match.group(1))
            count = int(match.group(2) or "1")
            if count:
                lines.update(range(start, start + count))
    flush()
    return files


def norm(path):
    return path.replace("\\", "/").lstrip("./")


def is_test_path(path):
    parts = norm(path).split("/")
    name = parts[-1]
    if name.endswith("_test.go") or name.endswith("_test.py") or ".spec." in name:
        return True
    if name.endswith("Test.java") or name.endswith("Test.php") or name.endswith("Test.cs"):
        return True
    if name.endswith("Tests.swift") or name.endswith("Test.swift"):
        return True
    return "test" in parts or "tests" in parts or "Tests" in parts


def strip_ns(element):
    for node in element.iter():
        if "}" in node.tag:
            node.tag = node.tag.split("}", 1)[1]


def mark(result, path, line, status):
    previous = result[path].get(line)
    if previous == "miss" or status == "miss":
        result[path][line] = "miss"
    else:
        result[path][line] = "hit"


def parse_go(text):
    result = defaultdict(dict)
    for raw in text.splitlines():
        if not raw.strip() or raw.startswith("mode:"):
            continue
        try:
            location, rest = raw.rsplit(" ", 1)
            location, _statements = location.rsplit(" ", 1)
            count = int(rest)
        except ValueError:
            continue
        filename, span = location.split(":", 1)
        start, end = span.split(",")
        start_line = int(start.split(".")[0])
        end_line = int(end.split(".")[0])
        status = "hit" if count > 0 else "miss"
        for number in range(start_line, end_line + 1):
            mark(result, filename, number, status)
    return result


def parse_lcov(text):
    result = defaultdict(dict)
    current = None
    for raw in text.splitlines():
        if raw.startswith("SF:"):
            current = raw[3:]
        elif raw.startswith("DA:") and current:
            number, hits = raw[3:].split(",")[:2]
            mark(result, current, int(number), "hit" if int(hits) > 0 else "miss")
        elif raw.startswith("BRDA:") and current:
            parts = raw[5:].split(",")
            if len(parts) >= 4 and parts[3] in ("0", "-"):
                result[current][int(parts[0])] = "miss"
    return result


def parse_jacoco(root):
    result = defaultdict(dict)
    for package in root.findall("package"):
        package_name = package.get("name") or ""
        for source in package.findall("sourcefile"):
            name = source.get("name")
            path = package_name + "/" + name if package_name else name
            for line in source.findall("line"):
                missed = int(line.get("mi") or 0)
                covered = int(line.get("ci") or 0)
                missed_branches = int(line.get("mb") or 0)
                if missed == 0 and covered == 0:
                    continue
                status = "miss" if missed or missed_branches else "hit"
                mark(result, path, int(line.get("nr")), status)
    return result


def parse_cobertura(root):
    result = defaultdict(dict)
    for cls in root.findall(".//class"):
        filename = cls.get("filename") or cls.get("name")
        for line in cls.findall("lines/line"):
            hits = int(line.get("hits") or 0)
            status = "hit" if hits else "miss"
            condition = line.get("condition-coverage") or ""
            if condition and not condition.startswith("100%"):
                status = "miss"
            mark(result, filename, int(line.get("number")), status)
    return result


def parse_clover(root):
    result = defaultdict(dict)
    for source in root.findall(".//file"):
        filename = source.get("name") or source.get("path")
        for line in source.findall("line"):
            number = line.get("num")
            if not number:
                continue
            status = "hit" if int(line.get("count") or 0) else "miss"
            true_count = line.get("truecount")
            false_count = line.get("falsecount")
            if line.get("type") == "cond" and true_count is not None and false_count is not None:
                if int(true_count) == 0 or int(false_count) == 0:
                    status = "miss"
            mark(result, filename, int(number), status)
    return result


def source_prefix(root, report, changed):
    """Nearest directory of the report that contains a changed source file.

    A language checkout can contain two trees with the same relative path.
    Limiting the diff to the directory that owns the report keeps those trees
    apart. Reports written outside the repository are left unscoped.
    """
    directory = os.path.dirname(os.path.abspath(report))
    while True:
        relative = os.path.relpath(directory, root)
        if relative.startswith(".."):
            return None
        prefix = "" if relative == "." else relative
        if any(path == prefix or path.startswith(prefix + "/") for path in changed):
            return prefix
        parent = os.path.dirname(directory)
        if parent == directory:
            return None
        directory = parent


def best_match(report_path, changed_paths, source_root=None):
    report_parts = [part for part in norm(report_path).split("/") if part]
    best = []
    best_len = 0
    for candidate in changed_paths:
        relative = norm(candidate)
        if source_root and relative.startswith(norm(source_root) + "/"):
            relative = relative[len(norm(source_root)) + 1:]
        parts = [part for part in relative.split("/") if part]
        matched = 0
        while matched < len(parts) and matched < len(report_parts) and parts[-1 - matched] == report_parts[-1 - matched]:
            matched += 1
        if matched == 0:
            continue
        if source_root and matched != min(len(parts), len(report_parts)):
            continue
        if matched > best_len:
            best = [candidate]
            best_len = matched
        elif matched == best_len:
            best.append(candidate)
    if len(best) == 1:
        return best[0]
    return None


def load_report(path, report_format):
    with open(path, "r", encoding="utf-8", errors="replace") as handle:
        text = handle.read()
    if report_format == "go":
        return parse_go(text)
    if report_format == "lcov":
        return parse_lcov(text)
    root = ET.fromstring(text)
    strip_ns(root)
    return {
        "jacoco": parse_jacoco,
        "cobertura": parse_cobertura,
        "clover": parse_clover,
    }[report_format](root)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--base", required=True)
    parser.add_argument("--report", required=True)
    parser.add_argument("--source-root", help="Repository-relative source tree covered by this report")
    parser.add_argument("--format", required=True, choices=["go", "cobertura", "jacoco", "clover", "lcov"])
    parser.add_argument("--minimum", type=float, default=90.0)
    args = parser.parse_args()

    root = git_output(["rev-parse", "--show-toplevel"], os.getcwd()).strip()
    report = args.report if os.path.isabs(args.report) else os.path.abspath(args.report)
    if not os.path.isfile(report):
        print("coverage report not found: " + report)
        return 1

    changed = {
        path: lines
        for path, lines in changed_lines(root, args.base).items()
        if not is_test_path(path)
    }
    prefix = norm(args.source_root) if args.source_root else source_prefix(root, report, changed)
    if prefix:
        changed = {
            path: lines
            for path, lines in changed.items()
            if path == prefix or path.startswith(prefix + "/")
        }
    covered = load_report(report, args.format)
    total = 0
    hit = 0
    missing = []
    matched = False
    for report_path, lines in covered.items():
        source = best_match(report_path, changed, args.source_root)
        if source is None:
            continue
        matched = True
        for number, status in lines.items():
            if number not in changed[source]:
                continue
            total += 1
            if status == "hit":
                hit += 1
            else:
                missing.append("%s:%s" % (source, number))

    if total == 0:
        if changed and not matched:
            print("coverage report did not match any changed file")
            return 1
        print("diff coverage: no instrumented lines changed")
        return 0
    percent = 100.0 * hit / total
    print("diff coverage: %.2f%% (%d/%d)" % (percent, hit, total))
    if missing:
        print("missing:")
        for item in missing[:80]:
            print("  " + item)
        if len(missing) > 80:
            print("  ... %d more" % (len(missing) - 80))
    if percent + 1e-9 < args.minimum:
        print("diff coverage %.2f%% is below %.0f%%" % (percent, args.minimum))
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
