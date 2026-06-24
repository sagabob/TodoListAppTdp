import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { GuardResult } from '@angular/router';
import { firstValueFrom, isObservable, of, throwError } from 'rxjs';
import { authGuard } from './auth.guard';
import { AuthService } from '../services/auth.service';

async function runGuard(): Promise<GuardResult> {
  const result = TestBed.runInInjectionContext(() => authGuard({} as never, {} as never));
  return isObservable(result) ? firstValueFrom(result) : result;
}

describe('authGuard', () => {
  let auth: { me: ReturnType<typeof vi.fn> };
  let router: { navigate: ReturnType<typeof vi.fn> };

  beforeEach(() => {
    auth = { me: vi.fn() };
    router = { navigate: vi.fn() };

    TestBed.configureTestingModule({
      providers: [
        { provide: AuthService, useValue: auth },
        { provide: Router, useValue: router },
      ],
    });
  });

  it('allows access when the user is logged in', async () => {
    auth.me.mockReturnValue(of({ email: 'demo@test.com', expiresAt: '2026-01-01T00:00:00Z' }));

    const result = await runGuard();

    expect(result).toBe(true);
    expect(router.navigate).not.toHaveBeenCalled();
  });

  it('redirects to login when the user is not logged in', async () => {
    auth.me.mockReturnValue(throwError(() => new Error('Unauthorized')));

    const result = await runGuard();

    expect(result).toBe(false);
    expect(router.navigate).toHaveBeenCalledWith(['/']);
  });
});
