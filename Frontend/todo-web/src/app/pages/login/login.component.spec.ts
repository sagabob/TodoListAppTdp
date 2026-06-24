import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';
import { LoginComponent } from './login.component';

describe('LoginComponent', () => {
  let auth: { login: ReturnType<typeof vi.fn> };
  let router: { navigate: ReturnType<typeof vi.fn> };

  beforeEach(async () => {
    auth = { login: vi.fn() };
    router = { navigate: vi.fn() };

    await TestBed.configureTestingModule({
      imports: [LoginComponent],
      providers: [
        { provide: AuthService, useValue: auth },
        { provide: Router, useValue: router },
      ],
    }).compileComponents();
  });

  it('shows a validation message when email or password is missing', () => {
    const fixture = TestBed.createComponent(LoginComponent);
    const component = fixture.componentInstance;

    component.email = '  ';
    component.password = '';
    component.login();

    expect(component.message()).toBe('Enter your email and password.');
    expect(component.isError()).toBe(true);
    expect(auth.login).not.toHaveBeenCalled();
  });

  it('navigates to home after a successful login', async () => {
    auth.login.mockReturnValue(of({ email: 'demo@test.com', expiresAt: '2026-01-01T00:00:00Z' }));

    const fixture = TestBed.createComponent(LoginComponent);
    fixture.componentInstance.login();
    await fixture.whenStable();

    expect(auth.login).toHaveBeenCalledWith({
      email: 'demo@test.com',
      password: 'Password123!',
    });
    expect(router.navigate).toHaveBeenCalledWith(['/home']);
    expect(fixture.componentInstance.isSubmitting()).toBe(false);
  });

  it('shows an error message when login fails', async () => {
    auth.login.mockReturnValue(throwError(() => new Error('Unauthorized')));

    const fixture = TestBed.createComponent(LoginComponent);
    fixture.componentInstance.login();
    await fixture.whenStable();

    expect(fixture.componentInstance.message()).toBe('Invalid email or password.');
    expect(fixture.componentInstance.isError()).toBe(true);
    expect(router.navigate).not.toHaveBeenCalled();
  });
});
