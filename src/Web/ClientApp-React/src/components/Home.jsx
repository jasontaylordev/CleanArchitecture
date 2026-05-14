export function Home() {
  return (
    <div>
      <h1>Welcome</h1>
      <p>A full-stack application with a <a href="https://react.dev/">React</a> frontend and an <a href="https://get.asp.net/">ASP.NET Core</a> backend, built with:</p>
      <ul>
        <li><a href="https://get.asp.net/">ASP.NET Core</a> and <a href="https://learn.microsoft.com/en-us/dotnet/csharp/">C#</a> for cross-platform server-side code</li>
        <li><a href="https://react.dev/">React</a> and <a href="https://vite.dev/">Vite</a> for client-side code</li>
        <li><a href="https://picocss.com/">Pico CSS</a> for layout and styling</li>
        <li><a href="https://lucide.dev/">Lucide</a> for icons</li>
      </ul>
      <p>To help you get started:</p>
      <ul>
        <li><strong>Client-side navigation</strong>. Click <em>Counter</em> then <em>Back</em> to return here.</li>
        <li><strong>Vite dev server</strong>. In development mode, Vite runs in the background with hot module replacement, so the page updates instantly when you modify any file.</li>
        <li><strong>Efficient production builds</strong>. In production mode, development-time features are disabled, and your <code>dotnet publish</code> configuration produces minified, efficiently bundled JavaScript files.</li>
      </ul>
      <p>The <code>ClientApp</code> subdirectory is a Vite + React application. Open a command prompt there to run <code>npm</code> commands such as <code>npm run dev</code> or <code>npm install</code>.</p>
    </div>
  );
}
