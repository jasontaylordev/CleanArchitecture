import { Sun, Moon, Laptop } from 'lucide-react';
import { useTheme } from './ThemeContext';

const icons = {
  auto:  <Laptop size={22} strokeWidth={2} />,
  light: <Sun    size={22} strokeWidth={2} />,
  dark:  <Moon   size={22} strokeWidth={2} />,
};

const next = { auto: 'light', light: 'dark', dark: 'auto' };

export function ThemeToggle() {
  const { theme, setTheme } = useTheme();

  return (
    <button
      className="theme-toggle-btn"
      onClick={() => setTheme(next[theme])}
      aria-label={theme}
    >
      {icons[theme]}
    </button>
  );
}
