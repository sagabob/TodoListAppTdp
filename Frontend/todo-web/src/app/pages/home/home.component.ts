import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { TodoItem } from '../../core/models/todo.models';
import { AuthService } from '../../core/services/auth.service';
import { TodoService } from '../../core/services/todo.service';

@Component({
  selector: 'app-home',
  imports: [FormsModule],
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly todoService = inject(TodoService);
  private readonly router = inject(Router);

  readonly email = signal<string | null>(null);
  readonly todos = signal<TodoItem[]>([]);
  readonly isLoading = signal(true);
  readonly isSaving = signal(false);
  readonly editingId = signal<string | null>(null);
  readonly errorMessage = signal('');

  newTitle = '';
  newDescription = '';
  editTitle = '';
  editDescription = '';

  ngOnInit(): void {
    this.auth.me().subscribe({
      next: (session) => {
        this.email.set(session.email);
        this.loadTodos();
      },
      error: () => {
        void this.router.navigate(['/']);
      },
    });
  }

  loadTodos(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.todoService
      .getAll()
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (list) => this.todos.set(list.items),
        error: () => {
          this.todos.set([]);
          this.errorMessage.set('Could not load todos.');
        },
      });
  }

  addTodo(): void {
    const title = this.newTitle.trim();
    if (!title || this.isSaving()) return;

    this.isSaving.set(true);
    this.errorMessage.set('');

    this.todoService
      .create({
        title,
        description: this.newDescription.trim(),
        isCompleted: false,
      })
      .pipe(finalize(() => this.isSaving.set(false)))
      .subscribe({
        next: () => {
          this.newTitle = '';
          this.newDescription = '';
          this.loadTodos();
        },
        error: () => this.errorMessage.set('Could not add task.'),
      });
  }

  startEdit(item: TodoItem): void {
    if (!item.id) return;

    this.editingId.set(item.id);
    this.editTitle = item.title;
    this.editDescription = item.description;
  }

  cancelEdit(): void {
    this.editingId.set(null);
    this.editTitle = '';
    this.editDescription = '';
  }

  saveEdit(item: TodoItem): void {
    const title = this.editTitle.trim();
    if (!title || !item.id || this.isSaving()) return;

    this.isSaving.set(true);
    this.errorMessage.set('');

    this.todoService
      .update({
        id: item.id,
        title,
        description: this.editDescription.trim(),
        isCompleted: item.isCompleted,
      })
      .pipe(finalize(() => this.isSaving.set(false)))
      .subscribe({
        next: () => {
          this.cancelEdit();
          this.loadTodos();
        },
        error: () => this.errorMessage.set('Could not save changes.'),
      });
  }

  toggleTodo(item: TodoItem): void {
    if (!item.id || this.editingId() === item.id) return;

    this.todoService
      .update({
        id: item.id,
        title: item.title,
        description: item.description,
        isCompleted: !item.isCompleted,
      })
      .subscribe({
        next: () => this.loadTodos(),
        error: () => this.errorMessage.set('Could not update task.'),
      });
  }

  deleteTodo(item: TodoItem): void {
    if (!item.id) return;

    if (this.editingId() === item.id) {
      this.cancelEdit();
    }

    this.todoService.delete(item.id).subscribe({
      next: () => this.loadTodos(),
      error: () => this.errorMessage.set('Could not delete task.'),
    });
  }

  logout(): void {
    this.auth.logout().subscribe({
      next: () => {
        void this.router.navigate(['/']);
      },
      error: () => {
        void this.router.navigate(['/']);
      },
    });
  }
}
