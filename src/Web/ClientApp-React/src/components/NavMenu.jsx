import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from './api-authorization/AuthContext';
import { ThemeToggle } from './ThemeToggle';

function AuthLinks() {
  const { isAuthenticated, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async (e) => {
    e.preventDefault();
    await logout();
    navigate('/login');
  };

  if (isAuthenticated) {
    return <li><a href="#" onClick={handleLogout}>Log out</a></li>;
  }
  return (
    <>
      <li><Link to="/login">Log in</Link></li>
      <li><Link to="/register">Register</Link></li>
    </>
  );
}

export function NavMenu() {
  return (
    <header>
      <nav>
        <ul>
          <li><Link to="/">Clean Architecture</Link></li>
        </ul>
        <ul>
          <li><Link to="/">Home</Link></li>
          <li><Link to="/counter">Counter</Link></li>
          <li><Link to="/weather">Weather</Link></li>
          <li><Link to="/todo">Tasks</Link></li>
        </ul>
        <ul>
          <AuthLinks />
          <li aria-hidden="true" className="nav-separator"></li>
          <li><ThemeToggle /></li>
        </ul>
      </nav>
    </header>
  );
}
