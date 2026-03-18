import { Component, computed } from '@angular/core';
import { ThemeService, Theme } from '../theme.service';

@Component({
  standalone: false,
  selector: 'app-theme-toggle',
  templateUrl: './theme-toggle.component.html'
})
export class ThemeToggleComponent {
  constructor(public themeService: ThemeService) {}

  icon = computed(() => {
    switch (this.themeService.theme()) {
      case 'light': return 'sun';
      case 'dark':  return 'moon';
      default:      return 'laptop';
    }
  });

  label = computed(() => {
    switch (this.themeService.theme()) {
      case 'light': return 'Light';
      case 'dark':  return 'Dark';
      default:      return 'Auto';
    }
  });

  cycle(): void {
    const next: Record<Theme, Theme> = { auto: 'light', light: 'dark', dark: 'auto' };
    this.themeService.setTheme(next[this.themeService.theme()]);
  }
}
