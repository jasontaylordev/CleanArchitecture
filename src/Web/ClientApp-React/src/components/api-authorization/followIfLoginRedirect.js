const baseUrl = document.getElementsByTagName('base')[0].href;
const loginUrl = `${baseUrl}Identity/Account/Login`;

export default function followIfLoginRedirect(response) {
  if (response.redirected && response.url.startsWith(loginUrl)) {
    window.location.href = `${loginUrl}?ReturnUrl=${window.location.pathname}`;
  }
}
