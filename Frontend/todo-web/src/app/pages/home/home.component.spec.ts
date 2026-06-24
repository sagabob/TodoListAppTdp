import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { TodoItem } from '../../core/models/todo.models';
import { AuthService } from '../../core/services/auth.service';
import { TodoService } from '../../core/services/todo.service';
import { HomeComponent } from './home.component';

const todo: TodoItem = {
  id: '1',
  title: 'Buy milk',
  description: 'Full cream',
  isCompleted: false,
};

describe('HomeComponent', () => {
  let auth: { me: ReturnType<typeof vi.fn>; logout: ReturnType<typeof vi.fn> };
  let todoService: {
    getAll: ReturnType<typeof vi.fn>;
    create: ReturnType<typeof vi.fn>;
    update: ReturnType<typeof vi.fn>;
    delete: ReturnType<typeof vi.fn>;
  };
  let router: { navigate: ReturnType<typeof vi.fn> };

  beforeEach(async () => {
    auth = {
      me: vi.fn(() => of({ email: 'demo@test.com', expiresAt: '2026-01-01T00:00:00Z' })),
      logout: vi.fn(() => of(undefined)),
    };
    todoService = {
      getAll: vi.fn(() => of({ items: [todo] })),
      create: vi.fn(() => of(todo)),
      update: vi.fn(() => of(undefined)),
      delete: vi.fn(() => of(undefined)),
    };
    router = { navigate: vi.fn() };

    await TestBed.configureTestingModule({
      imports: [HomeComponent],
      providers: [
        { provide: AuthService, useValue: auth },
        { provide: TodoService, useValue: todoService },
        { provide: Router, useValue: router },
      ],
    }).compileComponents();
  });

  it('loads the signed-in user and their todos', async () => {
    const fixture = TestBed.createComponent(HomeComponent);
    fixture.detectChanges();
    await fixture.whenStable();

    expect(auth.me).toHaveBeenCalled();
    expect(todoService.getAll).toHaveBeenCalled();
    expect(fixture.componentInstance.email()).toBe('demo@test.com');
    expect(fixture.componentInstance.todos()).toEqual([todo]);
    expect(fixture.componentInstance.isLoading()).toBe(false);
  });

  it('creates a todo and reloads the list', async () => {
    const fixture = TestBed.createComponent(HomeComponent);
    fixture.detectChanges();
    await fixture.whenStable();

    const component = fixture.componentInstance;
    component.newTitle = 'New task';
    component.newDescription = 'Details';
    component.addTodo();
    await fixture.whenStable();

    expect(todoService.create).toHaveBeenCalledWith({
      title: 'New task',
      description: 'Details',
      isCompleted: false,
    });
    expect(todoService.getAll).toHaveBeenCalledTimes(2);
    expect(component.newTitle).toBe('');
    expect(component.newDescription).toBe('');
  });

  it('deletes a todo and reloads the list', async () => {
    const fixture = TestBed.createComponent(HomeComponent);
    fixture.detectChanges();
    await fixture.whenStable();

    fixture.componentInstance.deleteTodo(todo);
    await fixture.whenStable();

    expect(todoService.delete).toHaveBeenCalledWith('1');
    expect(todoService.getAll).toHaveBeenCalledTimes(2);
  });

  it('redirects to login when the session is invalid', async () => {
    auth.me.mockReturnValue(throwError(() => new Error('Unauthorized')));

    const fixture = TestBed.createComponent(HomeComponent);
    fixture.detectChanges();
    await fixture.whenStable();

    expect(router.navigate).toHaveBeenCalledWith(['/']);
  });
});
