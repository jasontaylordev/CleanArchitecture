const { createProxyMiddleware } = require('http-proxy-middleware');
const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
    env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:5503';

const context = [
    "/weatherforecast",
    "/_configuration",
    "/.well-known",
    "/Identity",
    "/connect",
    "/ApplyDatabaseMigrations",
    "/_framework",
    "/api"
];

const onError = (err, req, resp, target) => {
    console.error(`${err.message}`);
}

function onProxyReq(proxyReq, req, res) {
    // add custom header to request
    proxyReq.setHeader('x-added', 'foobar');
    // or log the req
}

module.exports = function (app) {
    const appProxy = createProxyMiddleware(context, {
        proxyTimeout: 10000,
        target: target,
        // Handle errors to prevent the proxy middleware from crashing when
        // the ASP NET Core webserver is unavailable
        onError: onError,
        secure: false,
        // Uncomment this line to add support for proxying websockets
        //ws: true, 
        headers: {
            Connection: 'Keep-Alive'
        },
        onProxyReq: onProxyReq
    });

    app.use(appProxy);
};
