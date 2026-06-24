import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { TodoService } from './todo.service';

describe('TodoService', () => {
  let service: TodoService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(TodoService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('getAll loads todos for the signed-in user', () => {
    service.getAll().subscribe((list) => {
      expect(list.items.length).toBe(1);
      expect(list.items[0].title).toBe('Buy milk');
    });

    const req = httpMock.expectOne('/api/todo/all');
    expect(req.request.method).toBe('GET');
    expect(req.request.withCredentials).toBe(true);
    req.flush({
      items: [{ id: '1', title: 'Buy milk', description: '', isCompleted: false }],
    });
  });

  it('create posts a new todo', () => {
    service
      .create({ title: 'New task', description: 'Details', isCompleted: false })
      .subscribe((item) => {
        expect(item.id).toBe('1');
        expect(item.title).toBe('New task');
      });

    const req = httpMock.expectOne('/api/todo');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({
      title: 'New task',
      description: 'Details',
      isCompleted: false,
    });
    req.flush(
      { id: '1', title: 'New task', description: 'Details', isCompleted: false },
      { status: 201, statusText: 'Created' },
    );
  });

  it('update posts an existing todo', () => {
    service
      .update({ id: '1', title: 'Updated', description: 'Changed', isCompleted: true })
      .subscribe();

    const req = httpMock.expectOne('/api/todo');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({
      id: '1',
      title: 'Updated',
      description: 'Changed',
      isCompleted: true,
    });
    req.flush(null, { status: 204, statusText: 'No Content' });
  });

  it('delete removes a todo by id', () => {
    service.delete('1').subscribe();

    const req = httpMock.expectOne('/api/todo/1');
    expect(req.request.method).toBe('DELETE');
    expect(req.request.withCredentials).toBe(true);
    req.flush(null, { status: 204, statusText: 'No Content' });
  });
});
