import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { TodoItem, TodoListResponse } from '../models/todo.models';

@Injectable({ providedIn: 'root' })
export class TodoService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/todo';

  getAll(): Observable<TodoListResponse> {
    return this.http.get<TodoListResponse>(`${this.apiUrl}/all`, {
      withCredentials: true,
    });
  }

  create(item: Omit<TodoItem, 'id'>): Observable<TodoItem> {
    return this.http
      .post<TodoItem>(this.apiUrl, item, {
        withCredentials: true,
        observe: 'response',
      })
      .pipe(map((response) => response.body!));
  }

  update(item: TodoItem & { id: string }): Observable<void> {
    return this.http
      .post(this.apiUrl, item, {
        withCredentials: true,
        observe: 'response',
        responseType: 'text',
      })
      .pipe(map(() => undefined));
  }

  delete(id: string): Observable<void> {
    return this.http
      .delete(`${this.apiUrl}/${id}`, {
        withCredentials: true,
        observe: 'response',
        responseType: 'text',
      })
      .pipe(map(() => undefined));
  }
}
