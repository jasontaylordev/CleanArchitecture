import { useState } from 'react';
import { useNavigate, useLocation, Link } from 'react-router-dom';
import { useAuth } from './AuthContext';

export function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [invalid, setInvalid] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await login(email, password);
      const returnUrl = location.state?.returnUrl || '/';
      navigate(returnUrl, { replace: true });
    } catch {
      setInvalid(true);
    }
  };

  const handleChange = (setter) => (e) => {
    setInvalid(false);
    setter(e.target.value);
  };

  return (
    <article>
      <h2>Log in</h2>
      <form onSubmit={handleSubmit}>
        <label htmlFor="email">Email</label>
        <input type="email" id="email" autoComplete="username"
          value={email} onChange={handleChange(setEmail)}
          aria-invalid={invalid || undefined}
          aria-describedby={invalid ? 'login-error' : undefined} />
        <label htmlFor="password">Password</label>
        <input type="password" id="password" autoComplete="current-password"
          value={password} onChange={handleChange(setPassword)}
          aria-invalid={invalid || undefined}
          aria-describedby={invalid ? 'login-error' : undefined} />
        {invalid && <small id="login-error">Invalid email or password.</small>}
        <button type="submit">Log in</button>
        <p style={{ marginTop: '1rem' }}>Don't have an account? <Link to="/register">Register</Link></p>
      </form>
    </article>
  );
}
