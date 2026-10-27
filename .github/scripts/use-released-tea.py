"""Use released Tea after preparing unpublished companion dependencies."""

import json
import os
from pathlib import Path
import sys


tea = (Path(os.environ["GITHUB_WORKSPACE"]) / ".swift-ci/tea-swift").resolve()
temporary = Path(os.environ["RUNNER_TEMP"])
manifests = [Path(sys.argv[1]).resolve() / "Package.swift"]
manifests.extend(temporary.glob("swift-pr-dependencies-*/*/Package.swift"))
previous = ".package(path: " + json.dumps(str(tea)) + ")"
released = '.package(url: "https://github.com/aliyun/tea-swift.git", from: "1.1.0")'
replacements = 0
for manifest in manifests:
    source = manifest.read_text()
    count = source.count(previous)
    if count:
        manifest.write_text(source.replace(previous, released))
        replacements += count
if not replacements:
    raise RuntimeError("No temporary Tea dependencies were found")
print(f"Using released Tea 1.1.0 in {replacements} dependency declarations")
