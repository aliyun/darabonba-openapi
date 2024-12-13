const { execSync } = require('child_process');

const nodeVersion = process.versions.node;
console.log(`Running with Node.js version: ${nodeVersion}`);

// 定义不同 Node 版本对应的 TypeScript 和 @types/node 版本
const depsMap = {
  '10': {
    typescript: 'typescript@^3.9.7',
    nodeTypes: '@types/node@^12.0.0'
  },
  '12': {
    typescript: 'typescript@^4.1.3',
    nodeTypes: '@types/node@^12.0.0'
  }
};

// 获取当前 Node major 版本
const majorVersion = nodeVersion.split('.')[0];

// 选择合适的版本
const { typescript, nodeTypes } = depsMap[majorVersion] || {};
if(!typescript) {
  process.exit(0);
}
console.log(`Installing ${typescript} and ${nodeTypes}...`);

try {
  execSync(`npm install --no-save ${typescript} ${nodeTypes}`, { stdio: 'inherit' });
} catch (error) {
  console.error('Error installing specific TypeScript and @types/node version:', error);
  process.exit(1);
}