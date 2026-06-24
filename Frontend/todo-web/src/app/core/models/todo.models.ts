export interface TodoItem {
  id?: string;
  title: string;
  description: string;
  isCompleted: boolean;
}

export interface TodoListResponse {
  items: TodoItem[];
}
