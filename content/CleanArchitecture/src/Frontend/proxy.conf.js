const backendUrls = ['/api', '/auth'];

const result = {};

for (const url of backendUrls) {
  result[url] = {
    target: 'http://localhost:5000',
    secure: false,
    changeOrigin: true,
  };
}

module.exports = result;
