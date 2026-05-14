import { Injectable, signal } from '@angular/core';

export type Theme = 'auto' | 'light' | 'dark';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly STORAGE_KEY = 'picoColorScheme';

  theme = signal<Theme>('auto');

  constructor() {
    const stored = localStorage.getItem(this.STORAGE_KEY) as Theme | null;
    const initial: Theme = stored ?? 'auto';
    this.theme.set(initial);
    this.applyTheme(initial);
  }

  setTheme(value: Theme): void {
    this.theme.set(value);
    localStorage.setItem(this.STORAGE_KEY, value);
    this.applyTheme(value);
  }

  private applyTheme(theme: Theme): void {
    if (theme === 'auto') {
      document.documentElement.removeAttribute('data-theme');
    } else {
      document.documentElement.setAttribute('data-theme', theme);
    }
  }
}
