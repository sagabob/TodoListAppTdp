export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthSessionResponse {
  email: string;
  expiresAt: string;
}
