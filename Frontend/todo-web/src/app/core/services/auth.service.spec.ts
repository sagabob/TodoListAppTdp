import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('login posts credentials with credentials cookie', () => {
    service.login({ email: 'demo@test.com', password: 'Password123!' }).subscribe((session) => {
      expect(session.email).toBe('demo@test.com');
    });

    const req = httpMock.expectOne('/api/auth/login');
    expect(req.request.method).toBe('POST');
    expect(req.request.withCredentials).toBe(true);
    expect(req.request.body).toEqual({ email: 'demo@test.com', password: 'Password123!' });
    req.flush({ email: 'demo@test.com', expiresAt: '2026-01-01T00:00:00Z' });
  });

  it('me gets the current session', () => {
    service.me().subscribe((session) => {
      expect(session.email).toBe('demo@test.com');
    });

    const req = httpMock.expectOne('/api/auth/me');
    expect(req.request.method).toBe('GET');
    expect(req.request.withCredentials).toBe(true);
    req.flush({ email: 'demo@test.com', expiresAt: '2026-01-01T00:00:00Z' });
  });

  it('logout posts to logout endpoint', () => {
    service.logout().subscribe();

    const req = httpMock.expectOne('/api/auth/logout');
    expect(req.request.method).toBe('POST');
    expect(req.request.withCredentials).toBe(true);
    req.flush(null);
  });
});
