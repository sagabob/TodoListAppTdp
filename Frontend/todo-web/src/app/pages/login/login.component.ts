import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.component.html',
})
export class LoginComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  email = 'demo@test.com';
  password = 'Password123!';
  readonly message = signal('');
  readonly isSubmitting = signal(false);
  readonly isError = signal(false);

  login(): void {
    if (!this.email.trim() || !this.password) {
      this.isError.set(true);
      this.message.set('Enter your email and password.');
      return;
    }

    this.isSubmitting.set(true);
    this.message.set('');
    this.isError.set(false);

    this.auth
      .login({ email: this.email, password: this.password })
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: () => {
          void this.router.navigate(['/home']);
        },
        error: () => {
          this.isError.set(true);
          this.message.set('Invalid email or password.');
        },
      });
  }
}
